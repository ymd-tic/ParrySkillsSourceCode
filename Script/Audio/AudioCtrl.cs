using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class AudioCtrl : MonoBehaviour
{
    //-----SerializeField------------------------------------------------------------
    [Header("スライダー")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    [Header("オーディオミキサー")]
    [SerializeField] private AudioMixer audioMixer;

    //-----privateField--------------------------------------------------------------



    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------


    //-----protectedField------------------------------------------------------------

    private void Start()
    {
        //ミキサーのvolumeにスライダーのvolumeを入れてます。

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
