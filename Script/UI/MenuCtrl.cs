using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuCtrl : MonoBehaviour
{
    //-----SerializeField------------------------------------------------------------
    [Header("���j���[")]
    [SerializeField] private GameObject menuPanel;    // �p�l��

    [Header("�e�L�X�g")]
    [SerializeField] private TMP_Text menuKeyText;  // �p�l���̑���Key�e�L�X�g
    [SerializeField] private Button menuKeyBtn;  // �p�l���̑���Key�e�L�X�g

    [Header("�L�����o�X�O���[�v")]
    [SerializeField] private List<CanvasGroup> canvasGroups; 
    //-----privateField--------------------------------------------------------------
    private float speed = 0.5f; // �p�l���J���x
    private RectTransform rectPos;

    //-----publicField---------------------------------------------------------------
    [NonSerialized] public bool isOpenMenu = false;    // �p�l���J�t���O (true => �J���Ă��� false => ���Ă���)



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    private void Start()
    {
        rectPos = menuPanel.GetComponent<RectTransform>();
    }

    /// <summary>
    /// �Ώ�Key����������p�l���J��
    /// </summary>
    /// <param name="_context">�Ώ�Key</param>
    public void OnMenu(InputAction.CallbackContext _context)
    {
        if (!enabled) return;

        if (_context.started)
        {
            ExecuteEvents.Execute(menuKeyBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            return;
        }
        else if (_context.canceled)
        {
            ExecuteEvents.Execute(menuKeyBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
            return;
        }

        if (!isOpenMenu) // �p�l�������Ă�����
        {
            isOpenMenu = true;
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0f;

            // �p�l���ȊO��UI�𐧌�s�ɂ���
            foreach (var group in canvasGroups)
            {
                group.blocksRaycasts = false;
            }

            menuKeyText.SetText("����");

            // �p�l�����ړ�
            rectPos.DOAnchorPos(new Vector2(0, 0), speed)
                        .SetEase(Ease.OutQuart)
                        .SetUpdate(true)
                        .SetLink(gameObject);
        }
        else
        {
            isOpenMenu = false;
            CursorState();
            Time.timeScale = 1f;


            menuKeyText.SetText("�ݒ�");

            // �p�l�����ړ�
            rectPos.DOAnchorPos(new Vector3(0, -600), speed)
                        .SetEase(Ease.OutQuart)
                        .SetUpdate(true)
                        .SetLink(gameObject)
                        .OnComplete(() =>
                        {
                            // �p�l���ȊO��UI�𐧌�\�ɂ���
                            foreach (var group in canvasGroups)
                            {
                                group.blocksRaycasts = true;
                            }
                        });
        }
    }

    /// <summary>
    /// �V�[���ɂ���ăJ�[�\���̏�Ԃ�ς���
    /// </summary>
    private void CursorState()
    {
        switch(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            case MySceneManager.SceneData.TITLE:
                Cursor.lockState = CursorLockMode.Confined;
                break;

            case MySceneManager.SceneData.SELECT:
                Cursor.lockState = CursorLockMode.Confined;
                break;
                
            case MySceneManager.SceneData.STAGE01:
                Cursor.lockState = CursorLockMode.Locked;
                break;

            case MySceneManager.SceneData.STAGE02:
                Cursor.lockState = CursorLockMode.Locked;
                break;
        }
    }

}
