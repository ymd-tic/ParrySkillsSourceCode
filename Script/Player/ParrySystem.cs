using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParrySystem : MonoBehaviour
{

    //-----SerializeField------------------------------------------------------------
    [Header("パリィ成功エフェクト")]
    [SerializeField] ParticleSystem[] parryEfects;

    [Header("スクリプト")]
    [SerializeField] private PlayerAudioCtrl audioCtrl;

    //-----privateField--------------------------------------------------------------

    private Dictionary<int, bool> atackCollider { get; } = new Dictionary<int, bool>();


    //-----publicField---------------------------------------------------------------
    public static bool parrySuccess = false;    // パリィ成功したかフラグ


    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    #region システム


    private void OnTriggerEnter(Collider other)
    {
        // ヒットしたタグがEnemyAtackか検知
        if (!other.CompareTag("EnemyAtack")) { return; }
        if (parrySuccess) {return; }

        SuccessParry(other);
    }

    #endregion


    #region パリィ制御

    /// <summary>
    /// パリィの成功
    /// </summary>
    /// <param name="_other">ヒットした当たり判定</param>
    private void SuccessParry(Collider _other)
    {
        parrySuccess = true;

        // 判定を一度だけにする
        EnemyBase enemy = _other.GetComponent<EnemyAtack>().enemy;
        int enemyId = enemy.GetInstanceID();
        if (atackCollider.ContainsKey(enemyId)) { return; }
        atackCollider[enemyId] = true;

        // アドレナリンゲージを増やす
        this.transform.parent.GetComponent<SkillCtrl>().AdrenalineGaugeCalculation(10f);

        // 敵をノックバックさせる
        enemy.TakeParry();

        // エフェクト生成
        Vector3 efectPos = this.transform.position;
        efectPos.y = 1.5f;
        foreach (var efect in parryEfects)
        {
            Instantiate(efect, efectPos, Quaternion.identity);
        }


        Time.timeScale = 0.5f;

        StartCoroutine(ReseetParryFlag());
    }

    #endregion

    /// <summary>
    /// パリィフラグのリセット
    /// </summary>
    /// <returns></returns>
    IEnumerator ReseetParryFlag()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        parrySuccess = false;
        Time.timeScale = 1.0f;
        yield return new WaitForSecondsRealtime(0.8f);
        atackCollider.Clear();
    }
}
