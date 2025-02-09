using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialCtrl : MonoBehaviour
{
    [Serializable]
    private class TutorialPanel
    {
        public GameObject panel;

        [NonSerialized]
        public RectTransform rect;
    }

    //-----SerializeField------------------------------------------------------------
    [Header("�����p�l��")]
    [SerializeField] private TutorialPanel panelLeft;       // �����p�l����
    [SerializeField] private TutorialPanel panelRight;      // �����p�l���E

    [Header("�ΏۃL�[")]
    [SerializeField] private TMP_Text tutoriaKeyText;  // �p�l���̑���Key�e�L�X�g
    [SerializeField] private Button tutoriaKeyBtn;  // �p�l���̑���KeyBtn

    [Header("�L�����o�X�O���[�v")]
    [SerializeField] private List<CanvasGroup> canvasGroups;

    [Header("���j���[")]
    [SerializeField] private MenuCtrl menuCtrl;

    //-----privateField--------------------------------------------------------------
    private bool isOpenTutorial = false;    // �p�l���J�t���O (true => �J���Ă��� false => ���Ă���)
    private float speed = 0.5f; // �p�l���J���x


    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------


    private void Start()
    {
        panelLeft.rect = panelLeft.panel.GetComponent<RectTransform>();
        panelRight.rect = panelRight.panel.GetComponent<RectTransform>();
    }

    /// <summary>
    /// �Ώ�Key��������������p�l���J��
    /// </summary>
    /// <param name="_context">�Ώ�Key</param>
    public void OnTutorial(InputAction.CallbackContext _context)
    {
        if (menuCtrl.isOpenMenu) { return; }

        if (_context.started)
        {
            ExecuteEvents.Execute(tutoriaKeyBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            return; 
        }
        else if(_context.canceled)
        {
            ExecuteEvents.Execute(tutoriaKeyBtn.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
            return ;
        }

        if (!isOpenTutorial) // �p�l�������Ă�����
        {
            isOpenTutorial = true;
            menuCtrl.enabled = false;
            tutoriaKeyText.SetText("����");

            // �p�l������ʓ��Ɉړ�
            PanelMove(panelLeft, new Vector2(89, -50));
            PanelMove(panelRight,new Vector2(-95, -50));

            // ����UI���t�F�[�h�C��
            foreach (CanvasGroup group in canvasGroups)
            {
                group.blocksRaycasts = false;
                group.DOFade(0,speed)
                        .SetUpdate(true)
                        .SetLink(gameObject);
            }
        }
        else
        {
            isOpenTutorial = false;
            menuCtrl.enabled = true;
            tutoriaKeyText.SetText("�Q�[������");

            // �p�l������ʊO�Ɉړ�
            PanelMove(panelLeft,new Vector2 (-1130, -50));
            PanelMove(panelRight, new Vector2(600, -50));

            // ����UI���t�F�[�h�A�E�g
            foreach (CanvasGroup group in canvasGroups)
            {
                group.blocksRaycasts = true;
                group.DOFade(1, speed)
                        .SetUpdate(true)
                        .SetLink(gameObject);
            }
        }
    }

    /// <summary>
    /// �p�l���̈ړ�
    /// </summary>
    /// <param name="_panel">�p�l��</param>
    /// <param name="_vec2">�ڕW���W</param>
    private void PanelMove(TutorialPanel _panel, Vector2 _vec2)
    {
        _panel.rect.DOAnchorPos(_vec2, speed)
                        .SetEase(Ease.OutQuart)
                        .SetUpdate(true)
                        .SetLink(gameObject);
    }
}
