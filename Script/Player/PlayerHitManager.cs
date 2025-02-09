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

    #region �V�X�e��

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
    /// �p���B�E�q�b�g����̎��s���p
    /// </summary>
    IEnumerator ExeDamage(Collider _other)
    {
        yield return new WaitForSecondsRealtime(0.02f);

        if (!ParrySystem.parrySuccess)
        {
            EnemyBase enemy = _other.GetComponent<EnemyAtack>().enemy;

            // �ڐG�����G��ID���擾
            int enemyId = enemy.GetInstanceID();

            // ���łɃ_���[�W���󂯂Ă���ꍇ�͏������X�L�b�v
            if (atackCollider.ContainsKey(enemyId)) { yield break; }

            atackCollider[enemyId] = true;
            playerCtrl.TakeDamage(enemy.atackPower);

            yield return new WaitForSecondsRealtime(1f);
            atackCollider.Clear();
        }
    }
}


