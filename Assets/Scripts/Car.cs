using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Transform[] wheelsMeshes;
    public WheelCollider[] wheelColliders;

    public float rotationAngle;
    public float rotationSpeed;
    public float wheelRotateSpeed;
    public float skidMarkDelay;
    public Transform[] skidMarkPivots;
    public Transform[] grasses;
    public GameObject skidMark;
    public float skidScale;
    private float targetRotation;
    private WorldGenerator generator;
    public float detectLength;
    private Rigidbody rb;
    public Transform backpoint;
    public float forceSize;
    bool skidMarkRoutine;
    private float lastRotation;
    public float minRotationDifference;
    public GameObject ragDoll;
    public AudioSource scoreAudio;

    void Start()
    {
        wheelColliders = GetComponentsInChildren<WheelCollider>();
        generator = GameObject.FindObjectOfType<WorldGenerator>();
        rb = GetComponent<Rigidbody>();
        wheelsMeshes = new Transform[4];
        wheelsMeshes[0] = transform.Find("car/CarMeshes/WheelsMeshes/WheelMeshFR");
        wheelsMeshes[1] = transform.Find("car/CarMeshes/WheelsMeshes/WheelMeshFL");
        wheelsMeshes[2] = transform.Find("car/CarMeshes/WheelsMeshes/WheelMeshRR");
        wheelsMeshes[3] = transform.Find("car/CarMeshes/WheelsMeshes/WheelMeshRL");
        StartCoroutine(SkidMark());
    }
    private void FixedUpdate()
    {
        UpdateEffects();
    }

    void UpdateEffects()
    {

        bool addForce = true;

        for(int i = 0; i < skidMarkPivots.Length; ++i)
        {
            Transform wheelMesh = wheelColliders[i+2].GetComponent<Transform>();
            if(Physics.Raycast(wheelMesh.position, Vector3.down, detectLength))
            {
                if (!grasses[i].gameObject.activeSelf)
                {
                    grasses[i].gameObject.SetActive(true);
                }

                float effectHeight = wheelMesh.position.y - 0.4f * detectLength;
                Vector3 targetPosition = new Vector3(grasses[i].position.x, grasses[i].position.y, wheelMesh.position.z);
                grasses[i].position = targetPosition;
                skidMarkPivots[i].position = targetPosition;
                addForce = false;

            }
            else if (grasses[i].gameObject.activeSelf)
            {
                grasses[i].gameObject.SetActive(false);
            }
        }

        if (addForce)
        {
            rb.AddForceAtPosition(backpoint.position, Vector3.down * forceSize);
            skidMarkRoutine = false;
        }
        else
        {
            bool rotate = Mathf.Abs(lastRotation - transform.localEulerAngles.y) > minRotationDifference;
            if (targetRotation != 0 && 
                 rotate && 
                !skidMarkRoutine)
            {
                skidMarkRoutine = true;
            }
            else
            {
                skidMarkRoutine = false;
            }   
        }



        lastRotation = transform.localEulerAngles.y;
    }

    void LateUpdate()
    {
        for(int i = 0; i < wheelColliders.Length; ++i)
        {
            Quaternion quat;
            Vector3 vec;
            wheelColliders[i].GetWorldPose(out vec, out quat);
            wheelsMeshes[i].position = vec;
            wheelsMeshes[i].Rotate(Vector3.right * Time.deltaTime * wheelRotateSpeed);
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

    }


    IEnumerator SkidMark()
    {
        while (true)
        {
            yield return new WaitForSeconds(skidMarkDelay);
            if (skidMarkRoutine)
            {
                for (int i = 0; i < skidMarkPivots.Length; ++i)
                {
                    GameObject newSkidMark = Instantiate(skidMark, skidMarkPivots[i].position,
                                                         skidMarkPivots[i].rotation);
                    newSkidMark.transform.parent = generator.GetWorldPiece();
                    newSkidMark.transform.localScale = new Vector3(1, 1, 4) * skidScale;

                }
            }
            
        }
    }


    public void FallApart()
    {
        scoreAudio.Play();
        Instantiate(ragDoll, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }
}
