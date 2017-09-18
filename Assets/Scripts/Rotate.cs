using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    public Transform[] rotTransforms;
    private Collider coll;
    float rotSpeed = 10;

    void Start()
    {
        coll = GetComponent<Collider>();
        transform.rotation = GameObject.Find("Landscape").transform.rotation;
    }


    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (coll.Raycast(ray, out hit, 100.0F))
            {
                OnMouseDrag();
            }
        }

    }

    void OnMouseDrag()
    {
        float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

        transform.RotateAround(Vector3.up, -rotX);
        transform.RotateAround(Vector3.right, rotY);

        MirrorRotationToTransforms();

    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;

        MirrorRotationToTransforms();
    }

    void MirrorRotationToTransforms()
    {
        foreach (Transform child in rotTransforms)
        {
            child.transform.rotation = transform.rotation;
        }
    }
}
