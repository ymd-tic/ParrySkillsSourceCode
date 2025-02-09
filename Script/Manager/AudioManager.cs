using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //-----SerializeField------------------------------------------------------------
    [Header("BGM")]
    [SerializeField] private AudioClip nomalBGM;    //通常のBGM
    [SerializeField] private AudioClip stageBGM;   // 戦闘BGM


    //-----privateField--------------------------------------------------------------
    private Dictionary<string , AudioClip> sceneBGM = new Dictionary<string, AudioClip>();

    //-----publicField---------------------------------------------------------------
    [NonSerialized] public AudioSource audioSource;


    //-----staticField---------------------------------------------------------------
    static private AudioManager instance; // シングルトン


    //-----protectedField------------------------------------------------------------

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // シーンごとのBGMを設定
        sceneBGM[MySceneManager.SceneData.TITLE] = nomalBGM;
        sceneBGM[MySceneManager.SceneData.SELECT] = nomalBGM;
        sceneBGM[MySceneManager.SceneData.STAGE01] = stageBGM;
        sceneBGM[MySceneManager.SceneData.STAGE02] = stageBGM;

        if (instance == null)
        {
            instance = this;

            audioSource.clip = SetBGM();
            audioSource.Play();

            audioSource.loop = true;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // すでに設定してあるBGMと違うなら入れ替える
            if(instance.audioSource.clip != SetBGM())
            {
                instance.audioSource.clip = SetBGM();
                instance.audioSource.Play();
            }

            instance.audioSource.loop = true;

            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 現在のシーンに設定するBGMを返す
    /// </summary>
    /// <returns>BGM</returns>
    private AudioClip SetBGM()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        return sceneBGM[sceneName];
    }
}
