/*
    TiTH:
    Active ragdoll (sort of) logic
    Ahn Yubin @ MMXXI
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRagdollLogic : MonoBehaviour
{
    // To hold both ConfigurableJoint AND its initial rotation
    private class JointInfo
    {
        public ConfigurableJoint joint;
        public Quaternion initRotation;
        public JointInfo(ConfigurableJoint joint)
        {
            this.joint = joint;
            this.initRotation = joint.transform.localRotation;
        }
    }

    // Physics configs for ''stable'' ragdoll
    public float phyDriverForce = 0.25f;
    public int phySolverIterations = 10;
    public int phySolverVelocityIterations = 10;
    public int phyMaxAngularVelocity = 24;
    public GameObject boneHead;
    public List<GameObject> pinnedBones = new List<GameObject>(); // Bone that will be 'pinpointed' -- that will follow the original bone's transform
    public GameObject charRagdoll; // Simulated ragdoll Gameobject
    public GameObject charSource; // Manually animated character Gameobject
    private GameObject boneHip; // Hip bone
    private GameObject boneCOM, boneCOMProjected; // COM/Root bone (for later)
    private Dictionary<GameObject, GameObject> TBLRagdollToSource, TBLSourceToRagdoll; // Table to match/convert ragdoll's bone gameobject to source
    private Dictionary<GameObject, JointInfo> boneDrivers; // Dictionary of 'driver' bones/body parts and its joint: only this parts will 'mimic' the matching part of source


    // Start is called before the first frame update
    void Start()
    {
        if (!charRagdoll) // it's null -- try to automatically grab one
            charRagdoll = transform.Find("Ragdoll").gameObject;
        if (!charSource)
            charSource = transform.Find("Animated").gameObject;
        
        if (!charRagdoll) // still didn't found the ragdoll gameobject? we got a real problem
            Debug.LogError("OH NO: Couldn't find the Ragdoll Gameobject!!!");
        if (!charSource) // still didn't found the source gameobject? we got a real problem
            Debug.LogError("OH NO: Couldn't find the Animated source Gameobject!!!");
        if (charRagdoll && charSource)
            Debug.Log("Found GameObjects: Ragdoll=>`"+charRagdoll.name+"`, Source=>`"+charSource.name+"`");

        boneDrivers = new Dictionary<GameObject, JointInfo>();
        TBLRagdollToSource = new Dictionary<GameObject, GameObject>();
        TBLSourceToRagdoll = new Dictionary<GameObject, GameObject>();
        IndexBones();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float force = 0.5f;
        
        // For each driver bones we set the target rotation to that of the source
        foreach (var driver in boneDrivers) // driver = KeyValuePair
        {
            GameObject  bone = driver.Key,
                        src = TBLRagdollToSource[bone];
            Rigidbody rb = bone.GetComponent<Rigidbody>();
            JointInfo joint = driver.Value;

            Vector3 delta = src.transform.position - bone.transform.position;
            rb.AddForce(delta * phyDriverForce);
            joint.joint.SetTargetRotationLocal(src.transform.localRotation, joint.initRotation);
        }
        
        // For each pinned bones we copy the transform of the source
        foreach (var pinned in pinnedBones) // driver = KeyValuePair
        {
            GameObject src = TBLRagdollToSource[pinned];
            pinned.transform.position = src.transform.position;
            pinned.transform.rotation = src.transform.rotation;
        }

        // Try to make the head bone upright
        //boneHead
    }

    // Scan the bones and see if they're in a proper format while also updating necessary varaibles
    public void IndexBones()
    {
        // Get the hip bone & root bone
        //DumpAllChilds(charRagdoll, "Ragdoll/");
        boneHip = charRagdoll.transform.Find("Character1_Reference/Character1_Hips").gameObject;
        boneCOM = charRagdoll.transform.Find("Character1_Reference/Character1_COM_root/Character1_COM_com").gameObject;
        boneCOMProjected = charRagdoll.transform.Find("Character1_Reference/Character1_COM_root/Character1_COM_com/Character1_COM_proj").gameObject;
        if (!boneHip || !boneCOM || !boneCOMProjected)
        {
            Debug.LogError("OH NO: Hip/COM Bones does not exist OR in not proper format (ragdoll/Character1_Reference/Character1_Hips=`"+boneHip+"`, ragdoll/Character1_Reference/Character1_COM_root/Character1_COM_com=`"+boneCOM+"`, ragdoll/Character1_Reference/Character1_COM_root/Character1_COM_com/Character1_COM_proj=`"+boneCOMProjected+"`)");
            return;
        }

        // Do a DFS and for each child bone and check for any anomalities
        Debug.Log("Indexing bones...");
        TBLRagdollToSource.Clear();
        TBLSourceToRagdoll.Clear();
        IndexBonesSlave(boneHip);
        Debug.Log("Indexing bones done. Enjoy your bloody calcium lad");

        // Debug dump
        string cat = "";
        foreach (var driver in boneDrivers) // driver = KeyValuePair
        {
            cat += " * `ragdoll->"+driver.Key.name+"`\n";
        }
        Debug.Log("DEBUG> "+boneDrivers.Count+" driver bones:\n"+cat);
    }

    // Performs DFS on the child gameobjects and prints their name
    private void DumpAllChilds(GameObject root, string ap)
    {
        Debug.Log("Search: `"+ap+root.name+"`");
        foreach (Transform childtf in root.transform)
        {
            GameObject child = childtf.gameObject;
            DumpAllChilds(child, ap+child.name+"/");
        }
    }


    // Performs DFS on the child gameobjects in given bone, checking for any matching bone in the animated source and rigidbodies whatnot
    private void IndexBonesSlave(GameObject bone)
    {
        foreach (Transform childtf in bone.transform) // what the hell how does this even work??
        {
            GameObject childbone = childtf.gameObject;
            string childname = childbone.name;
            
            // Find matching bone in the animated source
            // Debug.Log("Search for `"+childname+"`...");
            GameObject siblingbone = SearchObject(charSource, childname, charSource.name+"/");
            if (!siblingbone)
            {
                Debug.LogWarning("No matching bone found for bone `ragdoll/Character1_Reference/Character1_Hips/["+childname+"]`!");
            }
            else // add to the table
            {
                TBLRagdollToSource.Add(childbone, siblingbone);
                TBLSourceToRagdoll.Add(siblingbone, childbone);
            }

            // Check if this bone is the driver
            Rigidbody rb = childbone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                var joint = childbone.GetComponent<ConfigurableJoint>();
                if (!joint)
                    Debug.LogError("OH NO: Driver bone `"+childname+"` has no ConfigurableJoint assigned!");
                else 
                    boneDrivers.Add(childbone, new JointInfo(joint));

                // Up the physics precision to minimize glitching
                rb.solverIterations = phySolverIterations;
                rb.solverVelocityIterations = phySolverVelocityIterations;
                rb.maxAngularVelocity = phyMaxAngularVelocity;
            }

            // Go deeper
            IndexBonesSlave(childbone);
        }
    }

    private GameObject SearchObject(GameObject root, string name, string ap="")
    {
        if (name == root.name)
            return root;
        //Debug.Log("Searching: `"+ap+root.name+"`");
        foreach (Transform childtf in root.transform)
        {
            var result = SearchObject(childtf.gameObject, name, ap+root.name+"/");
            if (result)
                return result;
        }
        return null;
    }

    private GameObject PleaseFind(GameObject gameObject, string n)
    {
        var tf = gameObject.transform.Find(n);
        if (!tf)
            Debug.LogError("Can't find `"+gameObject.name+"/"+n+"`!");
        return tf.gameObject;
    }
}
