using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitManager : MonoBehaviour
{

    //-----SerializeField------------------------------------------------------------


    //-----privateField--------------------------------------------------------------
    private PlayerCtrl playerCtrl;
    private Dictionary<int, bool> atackCollider { get; } = new Dictionary<int, bool>();

    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    #region システム

    private void Start()
    {
        playerCtrl = GetComponent<PlayerCtrl>();
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (!this.enabled) { return; }
        if (!_other.gameObject.CompareTag("EnemyAtack")) { return; }
        StartCoroutine(ExeDamage(_other));
    }

    private void OnControllerColliderHit(ControllerColliderHit _hit)
    {
        if (!this.enabled) { return; }

        if (_hit.collider.isTrigger)
        {
            OnTriggerEnter(_hit.collider);
        }
    }
    #endregion

    /// <summary>
    /// パリィ・ヒット判定の実行順用
    /// </summary>
    IEnumerator ExeDamage(Collider _other)
    {
        yield return new WaitForSecondsRealtime(0.02f);

        if (!ParrySystem.parrySuccess)
        {
            EnemyBase enemy = _other.GetComponent<EnemyAtack>().enemy;

            // 接触した敵のIDを取得
            int enemyId = enemy.GetInstanceID();

            // すでにダメージを受けている場合は処理をスキップ
            if (atackCollider.ContainsKey(enemyId)) { yield break; }

            atackCollider[enemyId] = true;
            playerCtrl.TakeDamage(enemy.atackPower);

            yield return new WaitForSecondsRealtime(1f);
            atackCollider.Clear();
        }
    }
}


