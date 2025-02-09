using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudioCtrl : MonoBehaviour
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
    [SerializeField] private AudioData damageVoice;     // �_���[�W�{�C�X
    [SerializeField] private AudioData dieVoice;        // �|���ꂽ�{�C�X

    //-----privateField--------------------------------------------------------------


    //-----publicField---------------------------------------------------------------


    //-----staticField---------------------------------------------------------------



    //-----ComponentField------------------------------------------------------------

    /// <summary>
    /// �_���[�W�{�C�X
    /// </summary>
    private void SoundDamageVoice()
    {
        SetSE(damageVoice);
    }

    /// <summary>
    /// ����{�C�X
    /// </summary>
    private void SoundDieVoice()
    {
        SetSE(dieVoice);
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
