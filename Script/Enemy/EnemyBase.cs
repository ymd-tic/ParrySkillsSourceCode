using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Serializable]
    protected class CoolTime // クールタイム
    {
        public float def = 0; // デフォルト時間
        [NonSerialized] public float cur = 0;   // 現在
        [NonSerialized] public float goal = 0;  // 目標
    }

    [Serializable]
    protected class Range // 行動パターン範囲
    {
        public float far = 0;   // 遠い
        public float near = 0;  // 近い
        public float atack = 0; // 攻撃距離
    }

    [Serializable]
    protected class Speed // 移動速度
    {
        public float fast = 0;  // 速い
        public float slow = 0;  // 遅い
        [NonSerialized]
        public readonly float zero = 0;  // 停止
    }

    //-----SerializeField------------------------------------------------------------
    [Header("ステータス")]          
    [SerializeField] private float maxHp = 100; // 最大HP
    [SerializeField] protected Speed speed;     // 移動速度
    [SerializeField] protected Range range;     // 行動パターン範囲

    [Header("エフェクト")]
    [SerializeField] private ParticleSystem parryEfect; // パリィエフェクト
    [SerializeField] protected GameObject damageTextObj;  // ダメージUI


    //-----privateField--------------------------------------------------------------
    private CapsuleCollider capsuleCollider;// コライダー
    private EnemyAudioCtrl audioCtrl;       // オーディオ

    //-----publicField---------------------------------------------------------------
    [NonSerialized]public float atackPower = 10;


    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------
    protected Transform playerPos;  // プレイヤー座標
    protected Transform enemyPos;   // 敵座標
    protected Generic.ParamateValue hpValue;  // HP
    protected bool canDamageAnim = true;   // ダメージアニメーションの再生フラグ
    protected bool isDie = false;          // 死亡フラグ
    protected NavMeshAgent agent;
    protected Animator animator;
    protected new Rigidbody rigidbody;
    protected TMP_Text damageText;            // ダメージUIテキスト


    #region システム

    protected virtual void Start()
    {
        playerPos = GameObject.FindWithTag("Player").transform;
        enemyPos = this.gameObject.transform;

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        rigidbody = GetComponent<Rigidbody>();
        audioCtrl = GetComponent<EnemyAudioCtrl>();
        damageText = damageTextObj.transform.GetChild(0).GetComponent<TMP_Text>();

        hpValue = new Generic.ParamateValue(maxHp, maxHp, 0);
    }


    protected virtual void Update()
    {
        if(AnimationEnd("Die"))
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// プレイヤーとの距離を返す
    /// </summary>
    /// <returns>距離</returns>
    protected float DistanceFromPlayer()
    {
        return Vector3.Distance(playerPos.position, enemyPos.position);
    }

    #endregion


    #region エネミーの制御

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="_damage">ダメージ</param>
    public virtual void TakeDamage(int _damage)
    {
        if (isDie) { return; }

        hpValue.cur += _damage;

        // ダメージUI生成
        damageText.text = $"{Mathf.Abs(_damage)}"; // ダメージをテキストに反映
        Vector3 popTextPos = new Vector3(enemyPos.position.x, 2.5f, enemyPos.position.z); // UIの出現位置調整
        Instantiate(damageTextObj, popTextPos, Quaternion.identity); // 生成

        if (hpValue.cur <= hpValue.min)
        {
            Die();
        }
    }

    /// <summary>
    /// パリィされた判定
    /// </summary>
    public virtual void TakeParry()
    {
        if(isDie) { return; }
    }

    /// <summary>
    /// HPが0になったら呼ばれる
    /// </summary>
    protected void Die()
    {
        isDie = true;
        animator.SetTrigger("Die");
        AreaManager.enemyList.Remove(this.gameObject);
        capsuleCollider.enabled = false;
        agent.destination = enemyPos.position;
        agent.speed = speed.zero;
    }

    #endregion



    #region アニメーションEvent

    /// <summary>
    /// パリィエフェクトを出す
    /// </summary>
    private void PlayParryEfect()
    {
        parryEfect.Play();
    }

    /// <summary>
    /// アニメーションの終了
    /// </summary>
    /// <param name="_animName">ステート名</param>
    /// <returns></returns>
    protected bool AnimationEnd([HideInInspector]string _animName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(_animName))
        {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsTag(_animName))
        {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
        }
        return false;
    }
    #endregion
}
