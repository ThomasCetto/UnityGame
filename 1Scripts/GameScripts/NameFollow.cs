using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameFollow : MonoBehaviour
{
    private Transform fpsCamera;

    private void Start()
    {
        fpsCamera = Camera.main.transform;
    }
    void LateUpdate()
    {
        if(fpsCamera != null) //quando il player muore la cam viene distrutta perciò non verrà più trovata
            transform.LookAt(transform.position + fpsCamera.rotation * Vector3.forward, fpsCamera.rotation * Vector3.up);
    }
}
