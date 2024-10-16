using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float moveSpeed = 30.0f;
    public float rotateSpeed = 30.0f;
    public bool lamp;

    private WorldGenerator generator;
    private Car car;
    Transform carTransform;

    void Start()
    {
        car = GameObject.FindObjectOfType<Car>();
        generator = GameObject.FindObjectOfType<WorldGenerator>();
        if (car != null)
        {
            carTransform = car.gameObject.transform;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        if(car != null)
        {
            CheckRotation();
        }
    }

    void CheckRotation()
    {
        Vector3 dir = (lamp) ? Vector3.right : Vector3.forward;
        float carRotation = carTransform.localEulerAngles.y;
        if(carRotation > car.rotationAngle * 2f)
        {
            carRotation = (360 - carRotation) * -1f;
        }

        transform.Rotate(dir * -rotateSpeed * (carRotation / (float)car.rotationAngle) * (36f / generator.dimensions.x) * Time.deltaTime);
    }
}
