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
    [SerializeField] private AudioClip nomalBGM;    //�ʏ��BGM
    [SerializeField] private AudioClip stageBGM;   // �퓬BGM


    //-----privateField--------------------------------------------------------------
    private Dictionary<string , AudioClip> sceneBGM = new Dictionary<string, AudioClip>();

    //-----publicField---------------------------------------------------------------
    [NonSerialized] public AudioSource audioSource;


    //-----staticField---------------------------------------------------------------
    static private AudioManager instance; // �V���O���g��


    //-----protectedField------------------------------------------------------------

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // �V�[�����Ƃ�BGM��ݒ�
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
            // ���łɐݒ肵�Ă���BGM�ƈႤ�Ȃ����ւ���
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
    /// ���݂̃V�[���ɐݒ肷��BGM��Ԃ�
    /// </summary>
    /// <returns>BGM</returns>
    private AudioClip SetBGM()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        return sceneBGM[sceneName];
    }
}
