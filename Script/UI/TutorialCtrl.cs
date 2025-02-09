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
    [Header("説明パネル")]
    [SerializeField] private TutorialPanel panelLeft;       // 説明パネル左
    [SerializeField] private TutorialPanel panelRight;      // 説明パネル右

    [Header("対象キー")]
    [SerializeField] private TMP_Text tutoriaKeyText;  // パネルの操作Keyテキスト
    [SerializeField] private Button tutoriaKeyBtn;  // パネルの操作KeyBtn

    [Header("キャンバスグループ")]
    [SerializeField] private List<CanvasGroup> canvasGroups;

    [Header("メニュー")]
    [SerializeField] private MenuCtrl menuCtrl;

    //-----privateField--------------------------------------------------------------
    private bool isOpenTutorial = false;    // パネル開閉フラグ (true => 開いている false => 閉じている)
    private float speed = 0.5f; // パネル開閉速度


    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------


    private void Start()
    {
        panelLeft.rect = panelLeft.panel.GetComponent<RectTransform>();
        panelRight.rect = panelRight.panel.GetComponent<RectTransform>();
    }

    /// <summary>
    /// 対象Keyを押したら説明パネル開閉
    /// </summary>
    /// <param name="_context">対象Key</param>
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

        if (!isOpenTutorial) // パネルが閉じていたら
        {
            isOpenTutorial = true;
            menuCtrl.enabled = false;
            tutoriaKeyText.SetText("閉じる");

            // パネルを画面内に移動
            PanelMove(panelLeft, new Vector2(89, -50));
            PanelMove(panelRight,new Vector2(-95, -50));

            // 他のUIをフェードイン
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
            tutoriaKeyText.SetText("ゲーム説明");

            // パネルを画面外に移動
            PanelMove(panelLeft,new Vector2 (-1130, -50));
            PanelMove(panelRight, new Vector2(600, -50));

            // 他のUIをフェードアウト
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
    /// パネルの移動
    /// </summary>
    /// <param name="_panel">パネル</param>
    /// <param name="_vec2">目標座標</param>
    private void PanelMove(TutorialPanel _panel, Vector2 _vec2)
    {
        _panel.rect.DOAnchorPos(_vec2, speed)
                        .SetEase(Ease.OutQuart)
                        .SetUpdate(true)
                        .SetLink(gameObject);
    }
}
