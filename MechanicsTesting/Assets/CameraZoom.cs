using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {
    //## - set in inspector

    public float zoomSensitivity = 1;//##
    public float minDist = 0.5f;//##
    public float maxDist = 30;//##
    public Transform anchor;//##
    public Vector3 dest;

    public void Awake()
    {
        dest = transform.localPosition;
    }

	public void Update()
    {
        dest.z = Mathf.Clamp(transform.localPosition.z + (Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity * Vector3.Distance(transform.position, anchor.position)), -maxDist, -minDist);
        transform.localPosition = dest;
    }
}
