using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNumberCoin : MonoBehaviour
{

    private int count = 0;
    void AddCoin()
    {
        GetComponent<UnityEngine.UI.Text>().text = "" + (++count);

        if(count >= 5)
        {
            GetComponent<UnityEngine.UI.Text>().text = "Victoire !";
        }
    }

    private void Start()
    {
        Reward.OnGetCoin += AddCoin;
    }
    
}
