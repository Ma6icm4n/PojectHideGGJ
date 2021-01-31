using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill : MonoBehaviour
{

    GameObject kid;
    Vector3 translateKid;
    public float distance;
    private void Start()
    {
       kid = GameObject.Find("/Kid");
        
    }

    void Update()
    {
        translateKid = kid.transform.position;

        if(Vector3.Distance(translateKid, transform.position) < distance)
        {
            Debug.Log("FFIIIIIIIIIIIIINNNNNNNNNNN");
            Application.Quit();
        } 
    }
    void OnCollisionEnter(Collision target)
    {
        if (target.transform.gameObject.name == "Kid")
        {
            
            Application.Quit();

        }

    }
}
