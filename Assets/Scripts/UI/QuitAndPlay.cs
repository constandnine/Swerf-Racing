using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitAndPlay : MonoBehaviour
{
    [Header("")]

    [SerializeField] private int sceneIndex;


    public void OnSceneSwitch(GameObject loadingscreen)
    {
        loadingscreen.SetActive(true);


        StartCoroutine(Loading());
    }


    public void OnQuit()
    {
        Application.Quit();
    }


    private IEnumerator Loading()
    {
        yield return new WaitForSeconds(2);


        SceneManager.LoadScene(sceneIndex);
    }
}
