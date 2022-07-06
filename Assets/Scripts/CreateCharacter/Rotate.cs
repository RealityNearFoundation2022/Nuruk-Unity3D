using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    public float angle;
    void OnMouseDrag()
    {
        float x = Input.GetAxis("Mouse X");
        transform.RotateAround(transform.position, new Vector3(0, x, 0) * Time.deltaTime, angle);
    }
}
