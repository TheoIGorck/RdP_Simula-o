using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject _rover;
    public float MoveAmount;

    private void Start()
    {
        _rover = GameObject.Find("Rover(Clone)");
    }
    
    private void LateUpdate()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(_rover.transform.position.x, Camera.main.transform.position.y, _rover.transform.position.z - 10), MoveAmount);
    }
}
