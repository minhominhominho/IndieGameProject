using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHipControlLogic : MonoBehaviour
{
    public GameObject hipController = null;
    public GameObject hipAnimated = null;
    private ConfigurableJoint controllerJoint = null;
    private Quaternion jointInitRotation;

    // Start is called before the first frame update
    void Start()
    {
        hipController = transform.Find("Ragdoll/Character1_Reference/Character1_COM_root/Character1_COM_com/HipController").gameObject;
        hipAnimated = transform.Find("Animated/Character1_Reference/Character1_Hips").gameObject;

        controllerJoint = hipController.GetComponent<ConfigurableJoint>();
        jointInitRotation = controllerJoint.transform.localRotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //controllerJoint.SetTargetRotationLocal(hipAnimated.transform.localRotation, jointInitRotation);
        //controllerJoint.targetPosition = hipAnimated.transform.localPosition;
        
    }
}
