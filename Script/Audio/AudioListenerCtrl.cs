using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerCtrl : MonoBehaviour
{
    //-----SerializeField------------------------------------------------------------
    [Header("プレイヤー")]
    [SerializeField] private GameObject player;

    [Header("カメラ")]
    [SerializeField] private GameObject mainCamera;

    //-----privateField--------------------------------------------------------------
    private Vector3 position;
    private Quaternion rotation;


    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    private void Start()
    {
    }

    private void Update()
    {
        this.transform.position = player.transform.position;

        this.transform.rotation = mainCamera.transform.rotation;
    }
}
