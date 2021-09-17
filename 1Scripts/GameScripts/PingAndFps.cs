using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PingAndFps : MonoBehaviour
{
    private float deltaTime;

    void Update()
    {
        GetComponent<Text>().text = "Ping: " + GetPingValue() + "ms   " + "FPS: " + GetFPS();
    }

    private int GetFPS()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        return (int)fps;
    }

    private int GetPingValue()
    {
        return PhotonNetwork.GetPing();
    }
}
