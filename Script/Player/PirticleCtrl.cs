using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PirticleCtrl : MonoBehaviour
{

    //-----SerializeField------------------------------------------------------------


    //-----privateField--------------------------------------------------------------


    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    private void Start()
    {
        StartCoroutine(DestroyEffect());
    }

    IEnumerator DestroyEffect()
    {
        yield return new WaitForSeconds(3.0f);
        
        Destroy(this.gameObject);
    }
}
