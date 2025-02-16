using UnityEngine;

public class CarControl : MonoBehaviour
{
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public Transform[] wheelMeshes = new Transform[4];

    public float maxSteerAngle = 30f;
    public float motorForce = 500f;
    public float brakeForce = 1000f;

    private float steerAngle;
    private float motorTorque;

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }

    private void GetInput()
    {
        steerAngle = maxSteerAngle * Input.GetAxis("Horizontal");
        motorTorque = motorForce * Input.GetAxis("Vertical");
    }

    private void Steer()
    {
        wheelColliders[0].steerAngle = steerAngle;
        wheelColliders[1].steerAngle = steerAngle;
    }

    private void Accelerate()
    {
        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].motorTorque = motorTorque;
        }
    }

    private void UpdateWheelPoses()
    {
        for (int i = 0; i < 4; i++)
        {
            UpdateWheelPose(wheelColliders[i], wheelMeshes[i]);
        }
    }

    private void UpdateWheelPose(WheelCollider collider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        float rotateAngle = collider.rpm * 6 * Time.fixedDeltaTime;
        collider.GetWorldPose(out pos, out rot);
        wheelTransform.SetPositionAndRotation(pos, rot * Quaternion.Euler(0,0,90));

        // 추가: 바퀴 회전 애니메이션
        wheelTransform.Rotate(Vector3.up, rotateAngle);
    }
}
