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
    [Header("メニュー")]
    [SerializeField] private GameObject menuPanel;    // パネル

    [Header("テキスト")]
    [SerializeField] private TMP_Text menuKeyText;  // パネルの操作Keyテキスト
    [SerializeField] private Button menuKeyBtn;  // パネルの操作Keyテキスト

    [Header("キャンバスグループ")]
    [SerializeField] private List<CanvasGroup> canvasGroups; 
    //-----privateField--------------------------------------------------------------
    private float speed = 0.5f; // パネル開閉速度
    private RectTransform rectPos;

    //-----publicField---------------------------------------------------------------
    [NonSerialized] public bool isOpenMenu = false;    // パネル開閉フラグ (true => 開いている false => 閉じている)



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    private void Start()
    {
        rectPos = menuPanel.GetComponent<RectTransform>();
    }

    /// <summary>
    /// 対象Keyを押したらパネル開閉
    /// </summary>
    /// <param name="_context">対象Key</param>
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

        if (!isOpenMenu) // パネルが閉じていたら
        {
            isOpenMenu = true;
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0f;

            // パネル以外のUIを制御不可にする
            foreach (var group in canvasGroups)
            {
                group.blocksRaycasts = false;
            }

            menuKeyText.SetText("閉じる");

            // パネルを移動
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


            menuKeyText.SetText("設定");

            // パネルを移動
            rectPos.DOAnchorPos(new Vector3(0, -600), speed)
                        .SetEase(Ease.OutQuart)
                        .SetUpdate(true)
                        .SetLink(gameObject)
                        .OnComplete(() =>
                        {
                            // パネル以外のUIを制御可能にする
                            foreach (var group in canvasGroups)
                            {
                                group.blocksRaycasts = true;
                            }
                        });
        }
    }

    /// <summary>
    /// シーンによってカーソルの状態を変える
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
