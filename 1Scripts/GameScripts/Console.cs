using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection; 

public class Console : MonoBehaviour
{
    float deltaTime;


    void Start()
    {
        
    }


    void Update()
    {
        //clearConsole();
        //printFrameRate();

    }

    private void printFrameRate(){
        /*deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        print("fps: " + Mathf.Ceil(fps));*/
    }

    private void clearConsole(){
        /*var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);*/
    }
}
