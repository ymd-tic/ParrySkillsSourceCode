using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioListenerCtrl : MonoBehaviour
{
    //-----SerializeField------------------------------------------------------------
    [Header("�v���C���[")]
    [SerializeField] private GameObject player;

    [Header("�J����")]
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
