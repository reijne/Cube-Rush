using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionRotate : MonoBehaviour
{
    // Start is called before the first frame update
    public float sensitivity;
    public Rigidbody rb;
    private bool isRotate = false;
    private Vector3 mouseRef;
    private Vector3 offset;
    private Vector3 rotation;
   
    private void Start() {
      rotation = Vector3.zero;
    }
    void Update() {
      if (isRotate) {
        offset = (Input.mousePosition - mouseRef);
        rotation.y = -(offset.x) * sensitivity;
        rotation.x = (offset.y) * sensitivity;
        rb.AddTorque(rotation);
        mouseRef = Input.mousePosition;
      }
    }

    private void OnMouseDown() {
      mouseRef = Input.mousePosition;
      isRotate = true;
    }

    private void OnMouseUp() {
      isRotate = false;
    }
}
