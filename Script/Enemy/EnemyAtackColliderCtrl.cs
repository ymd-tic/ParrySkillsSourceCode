using UnityEngine;
using System.Collections.Generic;

public class EnemyAtackColliderCtrl : MonoBehaviour
{
    [System.Serializable]
    public class ColliderList // コライダーのリスト
    {
        [Header("コライダー")]
        public List<Collider> colliders = new List<Collider>();

    }

    //-----SerializeField------------------------------------------------------------


    [Header("各攻撃の判定")]
    [SerializeField] new List<ColliderList> collider = new List<ColliderList>();


    //-----privateField--------------------------------------------------------------
    Animator animator;



    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    #region システム

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    #endregion


    #region アニメーションEvent

    /// <summary>
    /// コライダーを有効にする
    /// </summary>
    public void SetColliderOn()
    {
        // 現在の攻撃を取得
        int atackValue = animator.GetInteger("AtackValue");

        // コライダーを有効にする
        foreach (var col in collider[atackValue-1].colliders)
        {
            col.enabled = true;
        }
    }

    /// <summary>
    /// コライダーを無効にする
    /// </summary>
    public void SetColliderOff()
    {
        // コライダーを無効にする
        foreach (var col in collider)
        {
            foreach (var col2 in col.colliders)
            {
                col2.enabled = false;
            }
        }
    }
    #endregion

}
