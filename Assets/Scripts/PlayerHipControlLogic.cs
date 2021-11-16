using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHipControlLogic : MonoBehaviour
{
    public float balanceStrength = 1f;
    public Collider hipBounds;
    public GameObject hipController = null;
    public GameObject hipAnimated = null;
    private ConfigurableJoint controllerJoint = null;
    // Store initial joint properties
    private Vector3 jointXYZDriveSpring;
    private float jointAngleDriveSpring;
    private PlayerRagdollLogic ragdollLogic;

    // Start is called before the first frame update
    void Start()
    {
        ragdollLogic = gameObject.GetComponent<PlayerRagdollLogic>();

        hipController = transform.Find("Ragdoll/Character1_Reference/Character1_COM_root/Character1_COM_com/HipController").gameObject;
        hipAnimated = transform.Find("Animated/Character1_Reference/Character1_Hips").gameObject;

        controllerJoint = hipController.GetComponent<ConfigurableJoint>();
        jointXYZDriveSpring = new Vector3(controllerJoint.xDrive.positionSpring, controllerJoint.yDrive.positionSpring, controllerJoint.zDrive.positionSpring);
        jointAngleDriveSpring = controllerJoint.angularXDrive.positionSpring;
        //jointInitRotation = controllerJoint.transform.localRotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Update joint drive spring force
        JointDrive xdrive = controllerJoint.xDrive, ydrive = controllerJoint.yDrive, zdrive = controllerJoint.zDrive,
                    anglexdrive = controllerJoint.angularXDrive, angleyzdrive = controllerJoint.angularYZDrive;
        xdrive.positionSpring = jointXYZDriveSpring.x * balanceStrength;
        ydrive.positionSpring = jointXYZDriveSpring.y * balanceStrength;
        zdrive.positionSpring = jointXYZDriveSpring.z * balanceStrength;
        anglexdrive.positionSpring = jointAngleDriveSpring * balanceStrength;
        angleyzdrive.positionSpring = jointAngleDriveSpring * balanceStrength;

        // Re-assign the modified drive back to the joint
        controllerJoint.xDrive = xdrive;
        controllerJoint.yDrive = ydrive;
        controllerJoint.zDrive = zdrive;
        controllerJoint.angularXDrive = anglexdrive;
        controllerJoint.angularYZDrive = angleyzdrive;

        //controllerJoint.SetTargetRotationLocal(hipAnimated.transform.localRotation, jointInitRotation);
        //controllerJoint.targetPosition = hipAnimated.transform.localPosition;

        // Check if the hip is within the bounds of 'safe zone' or not
        if (hipBounds != null && !ragdollLogic.isRagdoll)
        {
            // basically a point is within the collider if the distance to the closest point on collider is (almost) zero
            if ((hipBounds.ClosestPoint(hipController.transform.position) - hipController.transform.position).sqrMagnitude > Mathf.Epsilon * Mathf.Epsilon)
            {
                // Fall down
                ragdollLogic.Falldown();
            }
            Debug.DrawLine(hipController.transform.position, hipBounds.ClosestPoint(hipController.transform.position), Color.green, 0);
            //Debug.Log("DIST: " + ((hipBounds.ClosestPoint(gameObject.transform.position) - gameObject.transform.position).sqrMagnitude));
        }
    }

    public void OnFallDown ()
    {
        balanceStrength = 0f;

        // Update joint drive spring force
        JointDrive xdrive = controllerJoint.xDrive, ydrive = controllerJoint.yDrive, zdrive = controllerJoint.zDrive,
                    anglexdrive = controllerJoint.angularXDrive, angleyzdrive = controllerJoint.angularYZDrive;
        xdrive.positionSpring = 0; xdrive.positionDamper = 0;
        ydrive.positionSpring = 0; ydrive.positionDamper = 0;
        zdrive.positionSpring = 0; zdrive.positionDamper = 0;

        anglexdrive.positionSpring = 0; anglexdrive.positionDamper = 0;
        angleyzdrive.positionSpring = 0; angleyzdrive.positionDamper = 0;

        // Re-assign the modified drive back to the joint
        controllerJoint.xDrive = xdrive;
        controllerJoint.yDrive = ydrive;
        controllerJoint.zDrive = zdrive;
        controllerJoint.angularXDrive = anglexdrive;
        controllerJoint.angularYZDrive = angleyzdrive;
    }
    /*
    void OnDrawGizmos ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hipBounds.ClosestPoint(gameObject.transform.position), 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(gameObject.transform.position, 0.1f);
    }
    */
}
