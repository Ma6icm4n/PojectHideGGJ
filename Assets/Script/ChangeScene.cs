using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    [SerializeField] private string sceneNameToLoad;

    public void changesScene()
    {
        if(SceneManager.GetActiveScene().name == "Scene_Main_Menu")
        {
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }
}
