using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioCtrl : MonoBehaviour
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
    [SerializeField] private AudioData runSE;       // 走る音
    [SerializeField] private AudioData atackSE;     // 攻撃音
    [SerializeField] private AudioData rollingSE;   // 回避音
    [SerializeField] private AudioData damageVoice; // ダメージボイス


    //-----privateField--------------------------------------------------------------


    //-----publicField---------------------------------------------------------------


    //-----staticField---------------------------------------------------------------



    //-----ComponentField------------------------------------------------------------


    /// <summary>
    /// 走る音の再生
    /// </summary>
    private void SoundRunSE()
    {
        SetSE(runSE);
    }

    /// <summary>
    /// 攻撃音の再生
    /// </summary>
    private void SoundAtackSE()
    {
        SetSE(atackSE);
    }

    /// <summary>
    /// 回避音の再生
    /// </summary>
    private void SoundRollingSE()
    {
        SetSE(rollingSE);
    }

    /// <summary>
    /// ダメージボイスの再生
    /// </summary>
    private void SoundDamageVoice()
    {
        SetSE(damageVoice);
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
