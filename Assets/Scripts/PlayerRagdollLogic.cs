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
        public float driveSpringBase;
        public float drivePosSpringFactor = 1f;
        public float driveAngleSpringFactor = 1f;
        public JointInfo(ConfigurableJoint joint)
        {
            this.joint = joint;
            this.initRotation = joint.transform.localRotation;
            this.driveSpringBase = joint.angularXDrive.positionSpring;
        }
    }

    // To hold driver bone and it's 'driving force': how strong the source animation will affect the driver bone
    [SerializeField]
    public class DriverForceOverrideInfo
    {
        public GameObject bone = null;
        public float posForce = 1f;
        public float angleForce = 1f;
    }

    public bool isRagdoll = false; // Is player in ragdoll state?
    // Physics configs for ''stable'' ragdoll
    public float phyAnimationForce = 0f;
    public float phyDriverForce = 0.25f;
    public float phyDriverForceAngle = 0.75f;
    public float phyDriverDampAngle = 1f;
    public int phySolverIterations = 10;
    public int phySolverVelocityIterations = 10;
    public int phyMaxAngularVelocity = 24;
    // For making the ragdoll stand upright
    public GameObject boneHead;
    public float headDriverForce = 500f;
    public float headDriverForceAngle = 300f;
    // For controlling the foot
    public GameObject boneLeftFoot, boneRightFoot;
    public Vector3 leftFootTarget, rightFootTarget;
    public List<GameObject> pinnedBones = new List<GameObject>(); // Bone that will be 'pinpointed' -- that will follow the original bone's transform
    public List<DriverForceOverrideInfo> driverForceOverride = new List<DriverForceOverrideInfo>(); // Driver force overriding info
    public GameObject charRagdoll; // Simulated ragdoll Gameobject
    public GameObject charSource; // Manually animated character Gameobject
    private GameObject boneHip; // Hip bone
    private GameObject boneCOM, boneCOMProjected; // COM/Root bone (for later)
    private Dictionary<GameObject, GameObject> TBLRagdollToSource, TBLSourceToRagdoll; // Table to match/convert ragdoll's bone gameobject to source
    private Dictionary<GameObject, JointInfo> boneDrivers; // Dictionary of 'driver' bones/body parts and its joint: only this parts will 'mimic' the matching part of source
    private PlayerHipControlLogic hipControlLogic;

    public GameObject fallEffect;

    // Start is called before the first frame update
    void Start()
    {
        hipControlLogic = gameObject.GetComponent<PlayerHipControlLogic>();

        if (!charRagdoll) // it's null -- try to automatically grab one
            charRagdoll = transform.Find("Ragdoll").gameObject;
        if (!charSource)
            charSource = transform.Find("Animated").gameObject;

        if (!charRagdoll) // still didn't found the ragdoll gameobject? we got a real problem
            Debug.LogError("OH NO: Couldn't find the Ragdoll Gameobject!!!");
        if (!charSource) // still didn't found the source gameobject? we got a real problem
            Debug.LogError("OH NO: Couldn't find the Animated source Gameobject!!!");
        if (charRagdoll && charSource)
            Debug.Log("Found GameObjects: Ragdoll=>`" + charRagdoll.name + "`, Source=>`" + charSource.name + "`");

        boneDrivers = new Dictionary<GameObject, JointInfo>();
        TBLRagdollToSource = new Dictionary<GameObject, GameObject>();
        TBLSourceToRagdoll = new Dictionary<GameObject, GameObject>();
        IndexBones();

        // Override driver forces
        foreach (var overrideInfo in driverForceOverride) // driver = KeyValuePair
        {
            var jointinfo = boneDrivers[overrideInfo.bone];
            jointinfo.drivePosSpringFactor = overrideInfo.posForce;
            jointinfo.driveAngleSpringFactor = overrideInfo.angleForce;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float force = 0.5f;

        // For each driver bones we set the target rotation to that of the source
        float driverForceAngle = phyAnimationForce * phyDriverForceAngle,
                driverForce = phyAnimationForce * phyDriverForce,
                driverDampAngle = phyAnimationForce * phyDriverDampAngle;

        foreach (var driver in boneDrivers) // driver = KeyValuePair
        {
            JointInfo joint = driver.Value;
            if (joint == null)
                continue;
            //FollowAnimation(joint, driver.Key, TBLRagdollToSource[driver.Key].transform.position, TBLRagdollToSource[driver.Key].transform.localRotation, false, true, true, true, driverForce, driverForceAngle, driverDampAngle);
            FollowAnimation(joint, driver.Key, TBLRagdollToSource[driver.Key].transform.position, TBLRagdollToSource[driver.Key].transform.localRotation, false, false, false, false, driverForce, driverForceAngle, driverDampAngle);
        }

        if (!isRagdoll)
        {
            // Make the head stand upright by copying the rotation and z-position
            FollowAnimation(boneDrivers[boneHead], boneHead, TBLRagdollToSource[boneHead].transform.position, TBLRagdollToSource[boneHead].transform.localRotation, true, false, true, false, headDriverForce, headDriverForceAngle, phyDriverDampAngle);
            //

            // Move the feet
            FollowAnimation(boneDrivers[boneLeftFoot], boneLeftFoot, leftFootTarget, Quaternion.LookRotation(new Vector3(0, 0, 1)), true, true, true, true, phyDriverForce, phyDriverForceAngle, phyDriverDampAngle);
            FollowAnimation(boneDrivers[boneRightFoot], boneRightFoot, rightFootTarget, Quaternion.LookRotation(new Vector3(0, 0, 1)), true, true, true, true, phyDriverForce, phyDriverForceAngle, phyDriverDampAngle);
            boneLeftFoot.transform.position = leftFootTarget;
            boneRightFoot.transform.position = rightFootTarget;
            //boneLeftFoot.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1));//src.transform.rotation;
            //boneRightFoot.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1));//src.transform.rotation;
            //
            /*
            // For each pinned bones we copy the transform of the source
            foreach (var pinned in pinnedBones) // driver = KeyValuePair
            {
                try
                {
                    GameObject src = TBLRagdollToSource[pinned];
                    pinned.transform.position = src.transform.position;
                    pinned.transform.rotation = src.transform.rotation;
                }
                catch
                {
                    Debug.LogError("Pinned bone `" + pinned.name + "` has no corresponding bone on source!");
                }
            }*/
        }
        else
        {
            phyDriverForce = 0;
            phyDriverForceAngle = 0;
            phyDriverDampAngle = 0;
        }

        // Try to make the head bone upright
        //boneHead
    }

    void Update()
    {
        if (!isRagdoll)
        {
            //boneLeftFoot.transform.position = leftFootTarget;
            //boneRightFoot.transform.position = rightFootTarget;
        }
    }

    // Turns into ragdoll by disabling all supports
    public void Falldown()
    {
        Debug.LogWarning("Ragdoll fell down!");
        GameObject eff = Instantiate(fallEffect, boneHip.transform);
        eff.transform.localPosition = Vector3.zero;
        eff.SetActive(true);
        Destroy(eff, 2f);
        isRagdoll = true;
        hipControlLogic.OnFallDown();

        // Since we used a lot of force to move the ragdoll, we must 'dampen' the velocity to prevent ragdolls flying away everytime they fall down
        float maxvel = 3f, maxangvel = 3f;
        foreach (var driver in boneDrivers)
        {
            Rigidbody rb = driver.Key.GetComponent<Rigidbody>();
            if (rb.velocity.magnitude > maxvel)
                rb.velocity = rb.velocity.normalized * maxvel;
            if (rb.angularVelocity.magnitude > maxangvel)
                rb.angularVelocity = rb.angularVelocity.normalized * maxangvel;
        }
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
            Debug.LogError("OH NO: Hip/COM Bones does not exist OR in not proper format (ragdoll/Character1_Reference/Character1_Hips=`" + boneHip + "`, ragdoll/Character1_Reference/Character1_COM_root/Character1_COM_com=`" + boneCOM + "`, ragdoll/Character1_Reference/Character1_COM_root/Character1_COM_com/Character1_COM_proj=`" + boneCOMProjected + "`)");
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
            cat += " * `ragdoll->" + driver.Key.name + "`\n";
        }
        Debug.Log("DEBUG> " + boneDrivers.Count + " driver bones:\n" + cat);
    }

    // Performs DFS on the child gameobjects and prints their name
    private void DumpAllChilds(GameObject root, string ap)
    {
        Debug.Log("Search: `" + ap + root.name + "`");
        foreach (Transform childtf in root.transform)
        {
            GameObject child = childtf.gameObject;
            DumpAllChilds(child, ap + child.name + "/");
        }
    }


    // Performs DFS on the child gameobjects in given bone, checking for any matching bone in the animated source and rigidbodies whatnot
    private void IndexBonesSlave(GameObject bone)
    {
        string name = bone.name;

        // Find matching bone in the animated source
        // Debug.Log("Search for `"+childname+"`...");
        GameObject siblingbone = SearchObject(charSource, name, charSource.name + "/");
        if (!siblingbone)
        {
            Debug.LogWarning("No matching bone found for bone `ragdoll/Character1_Reference/[" + name + "]`!");
        }
        else // add to the table
        {
            TBLRagdollToSource.Add(bone, siblingbone);
            TBLSourceToRagdoll.Add(siblingbone, bone);
        }

        // Check if this bone is the driver
        Rigidbody rb = bone.GetComponent<Rigidbody>();
        if (rb != null)
        {
            var joint = bone.GetComponent<ConfigurableJoint>();
            if (!joint)
            {
                //Debug.LogError("OH NO: Driver bone `"+name+"` has no ConfigurableJoint assigned!");
                boneDrivers.Add(bone, null);
            }
            else
                boneDrivers.Add(bone, new JointInfo(joint));

            // Up the physics precision to minimize glitching
            rb.solverIterations = phySolverIterations;
            rb.solverVelocityIterations = phySolverVelocityIterations;
            rb.maxAngularVelocity = phyMaxAngularVelocity;
        }

        foreach (Transform childtf in bone.transform) // what the hell how does this even work??
        {
            GameObject childbone = childtf.gameObject;
            // Go deeper
            IndexBonesSlave(childbone);
        }
    }

    private GameObject SearchObject(GameObject root, string name, string ap = "")
    {
        if (name == root.name)
            return root;
        //Debug.Log("Searching: `"+ap+root.name+"`");
        foreach (Transform childtf in root.transform)
        {
            var result = SearchObject(childtf.gameObject, name, ap + root.name + "/");
            if (result)
                return result;
        }
        return null;
    }

    private GameObject PleaseFind(GameObject gameObject, string n)
    {
        var tf = gameObject.transform.Find(n);
        if (!tf)
            Debug.LogError("Can't find `" + gameObject.name + "/" + n + "`!");
        return tf.gameObject;
    }

    // Makes the ragdoll's bone "follow" the source animation's bone
    private void FollowAnimation(JointInfo joint, GameObject bone, Vector3 targetPos, Quaternion targetRot, bool rotation, bool posx, bool posy, bool posz, float driverForce, float driverForceAngle, float driverDampAngle)
    {
        if (!bone) // invalid bones?
        {
            Debug.LogError("INVALID BONE: " + bone.name);
            return;
        }

        //GameObject src = TBLRagdollToSource[bone];
        Rigidbody rb = bone.GetComponent<Rigidbody>();
        ConfigurableJoint cj = joint.joint;

        Vector3 delta = targetPos - bone.transform.position;
        if (!posx) delta.x = 0f;
        if (!posy) delta.y = 0f;
        if (!posz) delta.z = 0f;
        rb.AddForce(delta * joint.drivePosSpringFactor * driverForce);

        // Set joint spring factor
        var xdrive = cj.angularXDrive;
        var yzdrive = cj.angularYZDrive;
        if (rotation)
        {
            cj.SetTargetRotationLocal(targetRot, joint.initRotation);
            xdrive.positionSpring = joint.driveSpringBase * joint.driveAngleSpringFactor * driverForceAngle;
            xdrive.positionDamper = driverDampAngle;
            yzdrive.positionSpring = joint.driveSpringBase * joint.driveAngleSpringFactor * driverForceAngle;
            yzdrive.positionDamper = driverDampAngle;
        }
        else
        {
            xdrive.positionSpring = 0;
            xdrive.positionDamper = 0;
            yzdrive.positionSpring = 0;
            yzdrive.positionDamper = 0;
        }

        cj.angularXDrive = xdrive;
        cj.angularYZDrive = yzdrive;
        //Debug.Log(bone.name + "-> Joint drive: " + cj.angularXDrive);
    }
}
