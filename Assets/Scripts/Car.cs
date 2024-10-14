using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Transform[] wheelsMeshes;
    public WheelCollider[] wheelColliders;

    public float rotationAngle;
    public float rotationSpeed;
    public float targetRotation;

    void Start()
    {
        wheelColliders = GetComponentsInChildren<WheelCollider>();
        wheelsMeshes = new Transform[4];
        wheelsMeshes[0] = transform.Find("car/CarMeshes/WheelsMeshes/WheelMeshFR");
        wheelsMeshes[1] = transform.Find("car/CarMeshes/WheelsMeshes/WheelMeshFL");
        wheelsMeshes[2] = transform.Find("car/CarMeshes/WheelsMeshes/WheelMeshRR");
        wheelsMeshes[3] = transform.Find("car/CarMeshes/WheelsMeshes/WheelMeshRL");
    }

    void LateUpdate()
    {
        for(int i = 0; i < wheelColliders.Length; ++i)
        {
            Quaternion quat;
            Vector3 vec;
            wheelColliders[i].GetWorldPose(out vec, out quat);
            wheelsMeshes[i].position = vec;
            wheelsMeshes[i].Rotate(Vector3.right * Time.deltaTime * rotationSpeed);
        }
        float horizontalInput = Input.GetAxis("Horizontal");
        bool bLeftMousePressing = Input.GetMouseButton(0);
        if (horizontalInput!=0 || bLeftMousePressing)
        {
            if (horizontalInput == 0) 
                targetRotation = Input.mousePosition.x > 0.5f * Screen.width ? rotationAngle : -rotationAngle;
            else 
                targetRotation = horizontalInput * rotationAngle;
        }
        else
        {
            targetRotation = 0;
        }
        Vector3 rotation = new Vector3(transform.localEulerAngles.x, targetRotation,
                                        transform.localEulerAngles.z);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rotation),
                                                       rotationSpeed * Time.deltaTime);

        //Quaternion targetRot = Quaternion.Euler(0, targetRotation, 0);
        //GetComponent<Rigidbody>().MoveRotation(Quaternion.RotateTowards(transform.rotation,
        //                                        targetRot, rotationSpeed * Time.deltaTime));
    }
}
