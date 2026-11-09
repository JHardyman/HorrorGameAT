using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTheCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float movementSpeed = 1.0f;
    float maxheight = 10.0f;
    float minheight = 0.0f;
    bool rising = true;

    // Update is called once per frame
    void Update()
    {
        if (rising)
        {
            transform.Translate(Vector3.up * movementSpeed * Time.deltaTime);
            if (transform.position.y>maxheight)
                rising  =false;
        }
        else
        {
            transform.Translate(Vector3.up * -movementSpeed * Time.deltaTime);
            if (transform.position.y< minheight)
                rising  =true;
        }
  
    }
}
