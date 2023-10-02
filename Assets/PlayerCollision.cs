using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    void OnCollisionEnter (Collision collisionInfo){
        //Debug.Log(collisionInfo.collider.name);
        if (collisionInfo.collider.tag =="flower"){
            Debug.Log("hit!!");
        }
    }
}

