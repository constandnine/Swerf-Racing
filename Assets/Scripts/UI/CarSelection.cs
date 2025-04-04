using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSelection : MonoBehaviour
{
    [SerializeField] private  GameObject[] cars;
    private int selectedCarIndex = 0;


    public void CarSelect(int carIndex)
    {
        selectedCarIndex = carIndex;
        PlayerPrefs.SetInt("SelectedCarIndex", selectedCarIndex);
        PlayerPrefs.Save();
    }
}
