using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePistol : MonoBehaviour
{
    [Header("Pistol")]
    [SerializeField] private Transform pistol;
    [SerializeField] private Vector3 pistolAngles;
    [SerializeField] private float maxY; //max e min coordinate y per la posizione
    [SerializeField] private float minY;
    [SerializeField] private float movingSpeed;
    [SerializeField] private float rotationSpeed;
    private bool goingUp=true;


    /*
    [Header("Head")]
    [SerializeField] private Transform head;
    [SerializeField] private Transform hat;
    [SerializeField] private Vector3 headAngles;
    [SerializeField] private Vector3 hatAngles;
    [SerializeField] private float maxZ;
    [SerializeField] private float minZ;
    [SerializeField] private float tiltSpeed;
    public bool goingRight = true;
    */

    // Update is called once per frame
    void Update()
    {
        PistolRotation();
        PistolMovement();
        


    }

    
    private void PistolRotation()
    {
        pistolAngles.y += rotationSpeed * Time.deltaTime;

        if (pistolAngles.y >= 360)
            pistolAngles.y -= 360;

        pistol.transform.localEulerAngles = new Vector3(pistolAngles.x, pistolAngles.y, pistolAngles.z);

    }

    private void PistolMovement()
    {
        if (goingUp && pistol.transform.localPosition.y > maxY)
            goingUp = false;
        if (!goingUp && pistol.transform.localPosition.y < minY) 
            goingUp = true;

        float temporarySpeed = movingSpeed * Time.deltaTime;

        if(goingUp)
            pistol.transform.localPosition = new Vector3(pistol.transform.localPosition.x, pistol.transform.localPosition.y + temporarySpeed, pistol.transform.localPosition.z);
        else
            pistol.transform.localPosition = new Vector3(pistol.transform.localPosition.x, pistol.transform.localPosition.y - temporarySpeed, pistol.transform.localPosition.z);
    }
}

/*private void HeadMovement()
    {

        headAngles.z = (goingRight)? headAngles.z -= tiltSpeed * Time.deltaTime : headAngles.z += tiltSpeed * Time.deltaTime;
        hatAngles.x = (goingRight) ? hatAngles.x += tiltSpeed * Time.deltaTime : hatAngles.x -= tiltSpeed * Time.deltaTime;


        head.transform.localEulerAngles = new Vector3(headAngles.x, headAngles.y, headAngles.z);
        hat.transform.localEulerAngles = new Vector3(hatAngles.x, hatAngles.y, hatAngles.z);

        if (head.transform.localEulerAngles.z < maxZ && head.transform.localEulerAngles.z > 180)
            goingRight = false;
        else if (head.transform.localEulerAngles.z > minZ && head.transform.localEulerAngles.z < 180)
            goingRight = true;
        
    }
    */
