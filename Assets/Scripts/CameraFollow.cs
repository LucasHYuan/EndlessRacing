using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform camTarget;
    public float height = 5f;
    public float distance = 6f;
    public float heightDamping = 0.5f;
    public float rotationDamping = 1f;

    void Start()
    {
        
    }
    void Update()
    {
        
    }

    void LateUpdate()
    {
        if(camTarget == null)
        {
            return;
        }
        float targetRotationAngle = camTarget.eulerAngles.y;
        float targetHeight = camTarget.position.y + height;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, 
                                              Time.deltaTime * rotationDamping);
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, heightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        transform.position = camTarget.position;
        transform.position -= currentRotation * Vector3.forward * distance;
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);



        transform.LookAt(camTarget);
    }
}
