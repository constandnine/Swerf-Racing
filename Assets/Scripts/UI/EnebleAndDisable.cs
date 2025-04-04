using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnebleAndDisable : MonoBehaviour
{
    public void Eneble(GameObject UiElement)
    {
        UiElement.SetActive(true);
    }


    public void Disable(GameObject UiElement)
    {
        UiElement.SetActive(false);
    }
}
