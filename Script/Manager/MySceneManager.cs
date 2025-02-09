using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public class SceneData // �V�[�����f�[�^
    {
        public const string TITLE = "Title";
        public const string SELECT = "StageSelect";
        public const string STAGE01 = "Stage01";
        public const string STAGE02 = "Stage02";
    }
    public enum GameEndStatus // �X�e�[�W�I�����̏��
    {
        CLEAR,
        OVER
    }

    [Serializable]
    private class GameFinishData // �Q�[���I�����̃f�[�^
    {
        public GameObject panel; // �p�l��
        public AudioClip audioClip; // BGM
    }

    //-----SerializeField------------------------------------------------------------
    [Tooltip("�Q�[���I��")]
    [SerializeField] GameFinishData gameClear;   // �N���A
    [SerializeField] GameFinishData gameOver;    // �I�[�o�[


    //-----privateField--------------------------------------------------------------



    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------


    //-----protectedField------------------------------------------------------------


    private void Awake()
    {
        // ���݂̃V�[�����ɂ���ăJ�[�\���̏�Ԃ�ύX
        switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            case SceneData.TITLE:
                Cursor.lockState = CursorLockMode.Confined;
                break;

            case SceneData.SELECT:
                Cursor.lockState = CursorLockMode.Confined;
                break;

            case SceneData.STAGE01:
            case SceneData.STAGE02:
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }

    /// <summary>
    /// �V�[���̃��[�h
    /// </summary>
    /// <param name="_name">���[�h�V�[����</param>
    public void OnLoadScene(string _name)
    {

        // �Q�[���v���C�I��
        if(_name == "Finish")
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();//�Q�[���v���C�I��
#endif
        }

        // �J�ڐ悪�X�e�[�W�̏ꍇ�G���i�[���Ă��郊�X�g���N���A
        switch (_name)
        {
            case MySceneManager.SceneData.STAGE01:
            case MySceneManager.SceneData.STAGE02:
            AreaManager.enemyList.Clear();
                break;
        }

        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_name, LoadSceneMode.Single);
    }


    /// <summary>
    /// �X�e�[�W�I��
    /// </summary>
    /// <param name="_status">�I�����</param>
    public void GameFinish(GameEndStatus _status)
    {
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0f;
        AreaManager.enemyList.Clear();
        AudioManager manager = GameObject.FindObjectOfType<AudioManager>();
        AudioSource source = manager.audioSource;

        switch (_status)
        {
            case GameEndStatus.CLEAR:
                gameClear.panel.SetActive(true);
                source.clip = gameClear.audioClip;
                break;
            case GameEndStatus.OVER:
                gameOver.panel.SetActive(true);
                source.clip= gameOver.audioClip;
                break;
        }

        source.loop = false;
        source.Play();
    }
}
