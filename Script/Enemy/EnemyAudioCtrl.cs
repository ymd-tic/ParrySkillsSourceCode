using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudioCtrl : MonoBehaviour
{
    [Serializable]
    private class AudioData // 音データ
    {
        public float volume; // 音量
        public AudioClip[] clips; // 鳴らすSE
    }

    //-----SerializeField------------------------------------------------------------
    [Header("オーディオソース")]
    [SerializeField] private AudioSource audioSource;

    [Header("効果音")]
    [SerializeField] private AudioData damageVoice;     // ダメージボイス
    [SerializeField] private AudioData dieVoice;        // 倒されたボイス

    //-----privateField--------------------------------------------------------------


    //-----publicField---------------------------------------------------------------


    //-----staticField---------------------------------------------------------------



    //-----ComponentField------------------------------------------------------------

    /// <summary>
    /// ダメージボイス
    /// </summary>
    private void SoundDamageVoice()
    {
        SetSE(damageVoice);
    }

    /// <summary>
    /// やられボイス
    /// </summary>
    private void SoundDieVoice()
    {
        SetSE(dieVoice);
    }

    /// <summary>
    /// SEの設定
    /// </summary>
    /// <param name="_data">鳴らすSE</param>
    private void SetSE(AudioData _data)
    {
        // 鳴らす音
        int se = UnityEngine.Random.Range(0, _data.clips.Length);

        audioSource.clip = _data.clips[se];    // SE設定
        audioSource.volume = _data.volume;     // 音量設定
        audioSource.Play();
    }
}
