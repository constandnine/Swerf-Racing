using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitAndPlay : MonoBehaviour
{
    [Header("")]

    [SerializeField] private int sceneIndex;


    public void OnSceneSwitch()
    {
        SceneManager.LoadScene(sceneIndex);
    }


    public void OnQuit()
    {
        Application.Quit();
    }
}
