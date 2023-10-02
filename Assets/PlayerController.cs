using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    //Variables or something like it
    public Rigidbody rb;
    public float forwardForce = 200f;
    public float sidewaysForce =200f;

    // Start is called before the first frame update
    
    void Start() // cuando inicia el juego, se activa esto
    {
        //rb.AddForce(0,200,500);
    }

    // Se llama antes de Update al mismo ratio basado en el Delta Time, recomendable colocar las físicas aquí
    void FixedUpdate()
    {


    }

    // Update is called once per frame // Llamado una vez por frame
    void Update()
    {

        if(Input.GetKey("w")){
            rb.AddForce(0,0,forwardForce*Time.deltaTime, ForceMode.VelocityChange);
            
        }
        if(Input.GetKey("s")){
            rb.AddForce(0,0,-forwardForce*Time.deltaTime, ForceMode.VelocityChange);
        }
        if(Input.GetKey("a")){
            rb.AddForce(-sidewaysForce*Time.deltaTime,0,0, ForceMode.VelocityChange);
        }
        if(Input.GetKey("d")){
            rb.AddForce(sidewaysForce*Time.deltaTime,0,0, ForceMode.VelocityChange);
        }
    

    }
}
