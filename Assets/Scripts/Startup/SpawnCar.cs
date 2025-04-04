using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCar : MonoBehaviour
{
    [SerializeField] private GameObject[] cars;
    [SerializeField] private Transform startPosition;

    void Start()
    {
        int selectedCarIndex = PlayerPrefs.GetInt("SelectedCarIndex", 0);


        if (selectedCarIndex >= 0 && selectedCarIndex < cars.Length)
        {
            Instantiate(cars[selectedCarIndex], startPosition.position, startPosition.rotation);
        }

        else
        {
            Debug.LogWarning("Invalid car index: " + selectedCarIndex);
        }
    }

}
