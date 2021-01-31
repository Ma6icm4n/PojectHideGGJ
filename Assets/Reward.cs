using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{

    public float collisionTime;
    public delegate void getCoin();

    public static event getCoin OnGetCoin;


    void OnCollisionEnter(Collision target)
    {
        if (target.transform.gameObject.name == "Kid")
        {

            OnGetCoin();
            Destroy(gameObject);


        }
        
    }

}
