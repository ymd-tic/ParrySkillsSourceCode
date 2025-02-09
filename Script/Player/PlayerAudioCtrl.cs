using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioCtrl : MonoBehaviour
{
    [Serializable]
    private class AudioData // ���f�[�^
    {
        public float volume; // ����
        public AudioClip[] clips; // �炷SE
    }

    //-----SerializeField------------------------------------------------------------
    [Header("�I�[�f�B�I�\�[�X")]
    [SerializeField] private AudioSource audioSource;

    [Header("���ʉ�")]
    [SerializeField] private AudioData runSE;       // ���鉹
    [SerializeField] private AudioData atackSE;     // �U����
    [SerializeField] private AudioData rollingSE;   // �����
    [SerializeField] private AudioData damageVoice; // �_���[�W�{�C�X


    //-----privateField--------------------------------------------------------------


    //-----publicField---------------------------------------------------------------


    //-----staticField---------------------------------------------------------------



    //-----ComponentField------------------------------------------------------------


    /// <summary>
    /// ���鉹�̍Đ�
    /// </summary>
    private void SoundRunSE()
    {
        SetSE(runSE);
    }

    /// <summary>
    /// �U�����̍Đ�
    /// </summary>
    private void SoundAtackSE()
    {
        SetSE(atackSE);
    }

    /// <summary>
    /// ������̍Đ�
    /// </summary>
    private void SoundRollingSE()
    {
        SetSE(rollingSE);
    }

    /// <summary>
    /// �_���[�W�{�C�X�̍Đ�
    /// </summary>
    private void SoundDamageVoice()
    {
        SetSE(damageVoice);
    }

    /// <summary>
    /// SE�̐ݒ�
    /// </summary>
    /// <param name="_data">�炷SE</param>
    private void SetSE(AudioData _data)
    {
        // �炷��
        int se = UnityEngine.Random.Range(0, _data.clips.Length);

        audioSource.clip = _data.clips[se];    // SE�ݒ�
        audioSource.volume = _data.volume;     // ���ʐݒ�
        audioSource.Play();
    }
}
