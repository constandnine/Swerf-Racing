using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public GameObject player;

    public LayerMask car;
    List<GameObject> stand;

    public GameObject win, lose;



    private IEnumerator OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == car)
        {
            stand.Add(other.gameObject);

            if (other.GetComponentInParent<EnebleAndDisable>())
            {
                player = other.gameObject;
                win = GameObject.Find("Win");
                //lose = GameObject.Find("lose");
            }

            yield return new WaitForSeconds(1);

            SetDisplay();
        }
    }

    void SetDisplay()
    {
        if (stand[0] == player)
        {
            win.SetActive(true);
        }

        else
        {
            lose.SetActive(true);
        }
    }
}
