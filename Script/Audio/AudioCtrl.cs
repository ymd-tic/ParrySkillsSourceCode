using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class AudioCtrl : MonoBehaviour
{
    //-----SerializeField------------------------------------------------------------
    [Header("�X���C�_�[")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    [Header("�I�[�f�B�I�~�L�T�[")]
    [SerializeField] private AudioMixer audioMixer;

    //-----privateField--------------------------------------------------------------



    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------


    //-----protectedField------------------------------------------------------------

    private void Start()
    {
        //�~�L�T�[��volume�ɃX���C�_�[��volume�����Ă܂��B

        //BGM
        audioMixer.GetFloat("BGM", out float bgmVolume);
        bgmSlider.value = bgmVolume;
        //SE
        audioMixer.GetFloat("SE", out float seVolume);
        seSlider.value = seVolume;
    }

    public void SetValumeBGM(float _volume)
    {
        audioMixer.SetFloat("BGM", _volume);
    }

    public void SetValumeSE(float _volume)
    {
        audioMixer.SetFloat("SE", _volume);
    }
}
