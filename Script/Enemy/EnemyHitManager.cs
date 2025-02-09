using UnityEngine;
using System.Collections;
public class EnemyHitManager : MonoBehaviour
{

    //-----SerializeField------------------------------------------------------------



    //-----privateField--------------------------------------------------------------
    private EnemyBase enemy;
    private new Collider collider = null;
    private bool canDamage = true; // ダメージを受けるか判定


    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    #region システム

    void Start()
    {
        enemy = GetComponent<EnemyBase>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerAtack") && canDamage)
        {
            collider = other;
            StartCoroutine(ExeDamage());
        }
    }

    #endregion


    #region コルーチン

    /// <summary>
    /// パリィ・ヒット判定の実行順用
    /// </summary>
    IEnumerator ExeDamage()
    {
        yield return new WaitForSecondsRealtime(0.04f);

        if (!ParrySystem.parrySuccess)
        {
            PlayerCtrl playerCtrl = collider.transform.root.GetComponent<PlayerCtrl>();

            collider.transform.root.GetComponent<SkillCtrl>().AdrenalineGaugeCalculation(1.0f);
            enemy.TakeDamage(-playerCtrl.atackPower);
            StartCoroutine(CanDamage());
        }
    }

    /// <summary>
    /// 一定時間無敵
    /// </summary>
    /// <returns></returns>
    IEnumerator CanDamage()
    {
        float canDamageTime = 0.2f; // 無敵時間

        canDamage = false;
        yield return new WaitForSeconds(canDamageTime);
        canDamage = true;
    }

    #endregion
}
