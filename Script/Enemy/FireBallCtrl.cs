using System.Collections;
using UnityEngine;

public class FireBallCtrl : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DestroyObject());
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
