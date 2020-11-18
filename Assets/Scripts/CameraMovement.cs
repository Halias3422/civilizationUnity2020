using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("a"))
            cam.transform.position = new Vector3(cam.transform.position.x - 0.1f, cam.transform.position.y, cam.transform.position.z);    
        if (Input.GetKey("d"))
            cam.transform.position = new Vector3(cam.transform.position.x + 0.1f, cam.transform.position.y, cam.transform.position.z);    
        if (Input.GetKey("w"))
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y + 0.1f, cam.transform.position.z);    
        if (Input.GetKey("s"))
            cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y - 0.1f, cam.transform.position.z);    
        if (Input.GetKey("q") && cam.orthographicSize > 0.5f)
            cam.orthographicSize -= 0.1f;
        if (Input.GetKey("e") && cam.orthographicSize < 200)
            cam.orthographicSize += 0.1f;
    }
}
