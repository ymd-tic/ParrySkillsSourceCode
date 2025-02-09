using UnityEngine;
using UnityEngine.UI;

public class EnemyGolemCtrl : EnemyBase
{
    private enum AIState // 状態パターン
    {
        Idle,       // 待機
        Chase,      // 追跡
        Atack,      // 攻撃
        KnockBack   // ノックバック
    }

    public enum AtackState // 攻撃パターン
    {
        Melee1,     // 近接1
        Melee2,     // 近接2
        Jump        // ジャンプ
    }

    //-----SerializeField------------------------------------------------------------
    [Header("クールタイム")]
    [SerializeField] private CoolTime atackTime;   // 攻撃クールタイム
    [Header("ゲージ")]
    [SerializeField] private Slider hpGage; // HPゲージ

    //-----privateField--------------------------------------------------------------
    private AIState aiState = AIState.Idle;
    private AtackState atackState = AtackState.Melee1;
    private bool findPlayer = false;    // プレイヤーを見つけたかフラグ



    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    #region システム

    protected override void Start()
    {
        base.Start();
        hpGage.value = hpValue.cur / hpValue.max;

        // クールタイムの初期値設定
        atackTime.goal = Generic.RandomErrorRange(atackTime.def, 2f);
    }

    protected override void Update()
    {
        if (AnimationEnd("Die"))
        {
            var sceneManager = GameObject.Find("SceneManager").GetComponent<MySceneManager>();
            sceneManager.GameFinish(MySceneManager.GameEndStatus.CLEAR);

            Destroy(this.gameObject);
        }

        // 死んだら何もしない
        if (isDie) { return; }

        switch (aiState)
        {
            case AIState.Idle:
                Idle();
                break;

            case AIState.Chase:
                Chase();
                break;

            case AIState.Atack:
                Atack();
                break;

            case AIState.KnockBack:
                KnockBack();
                break;
        }
    }

    #endregion

    #region 状態ステート

    private void Idle()
    {
        // プレイヤーとの距離が追跡範囲内なら
        if(DistanceFromPlayer() <= range.far)
        {
            if(DistanceFromPlayer() > range.atack)
            {
                ChangeAIState(AIState.Chase);

                if(!findPlayer)
                {
                    findPlayer = true;
                }
                return;
            }
        }

        if(!findPlayer) { return; }

        // 攻撃クールタイムが終わったら攻撃ステートに移る
        atackTime.cur += Time.deltaTime;
        if (atackTime.cur > atackTime.goal)
        {
            ChangeAIState(AIState.Atack);
            canDamageAnim = false;
            atackTime.cur = 0;
            return;
        }
    }

    private void Chase()
    {
        agent.destination = playerPos.position;

        // プレイヤーとの距離が攻撃範囲内なら
        if (DistanceFromPlayer() <= range.atack)
        {
            ChangeAIState(AIState.Idle);
            return;
        }
    }

    private void Atack()
    {
        // 攻撃分岐
        switch (atackState)
        {
            case AtackState.Melee1:
                Melee1();
                break;

            case AtackState.Melee2:
                Melee2();
                break;

            case AtackState.Jump:
                Jump();
                break;
        }

        // 攻撃モーションの終了
        if (AnimationEnd("Atack"))
        {
            // プレイヤーとの距離が攻撃範囲外
            if (DistanceFromPlayer() > range.atack)
            {
                ChangeAIState(AIState.Chase);
            }
            else
            {
                ChangeAIState(AIState.Idle);
            }

            animator.SetInteger("AtackValue", 0);
        }

        #region 攻撃ステート

        void Melee1()
        {

        }

        void Melee2()
        {

        }

        void Jump()
        {

        }
        #endregion

    }

    private void KnockBack()
    {
        if (AnimationEnd("KnockBack"))
        {
            rigidbody.isKinematic = true;
            canDamageAnim = true;

            // プレイヤーとの距離が追跡範囲内なら
            if (DistanceFromPlayer() <= range.far)
            {   // かつ攻撃範囲内なら
                if (DistanceFromPlayer() <= range.atack)
                {
                    ChangeAIState(AIState.Idle);
                    return;
                }

                ChangeAIState(AIState.Chase);
            }
        }
    }
    #endregion

    #region エネミーの制御

    public override async void TakeDamage(int _damage)
    {
        if(isDie) { return; }

        // 受けたダメージを反映
        damageText.text = $"{Mathf.Abs(_damage)}";
        // UIのポップアップ位置
        Vector3 popTextPos = new Vector3(enemyPos.position.x, 2.5f, enemyPos.position.z);
        // ダメージUI生成
        Instantiate(damageTextObj, popTextPos, Quaternion.identity);

        // HPを減らす
        await new Generic.CalcuRation().ValueFluctuation(_damage, hpGage, hpValue).AsTask(this);

        if (hpValue.cur <= hpValue.min)
        {
            Die();
        }
    }

    public override void TakeParry()
    {
        base.TakeParry();

        rigidbody.isKinematic = false;
        ChangeAIState(AIState.KnockBack);
        transform.LookAt(playerPos);
    }

    /// <summary>
    /// 状態ステートを変える
    /// ステート毎のアニメーションを再生
    /// </summary>
    /// <param name="_nextState">次のステート</param>
    private void ChangeAIState(AIState _nextState)
    {
        if (isDie) { return; }

        // ステート更新
        aiState = _nextState;

        // アニメーション更新
        animator.SetTrigger($"{_nextState}");   

        foreach (var animState in animator.parameters)
        {
            if(animState.type != AnimatorControllerParameterType.Trigger) {  continue; }

            if (animState.name != $"{_nextState}")
            {
                animator.ResetTrigger($"{animState.name}");
            }
        }

        // 次のステートに移る時に1回だけ呼ばれる処理
        switch (_nextState)
        {
            case AIState.Idle:
                agent.speed = speed.zero;
                agent.destination = enemyPos.position;
                break;

            case AIState.Chase:
                agent.speed = speed.fast;
                agent.destination = playerPos.position;
                break;

            case AIState.Atack:
                // 攻撃をランダムで選択
                AtackState atack = EnumGeneric.GetRandom<AtackState>();
                SetAtackState(atack);

                // 攻撃クールタイムをランダムで設定
                atackTime.goal = Generic.RandomErrorRange(atackTime.def, 2.0f);

                transform.LookAt(playerPos.position);
                agent.speed = speed.zero;
                agent.destination = enemyPos.position;
                break;

            case AIState.KnockBack:
                agent.speed = speed.zero;
                agent.destination = enemyPos.position;
                break;
        }

        //Debug.Log($"{_nextState}ステートに更新");
    }

    /// <summary>
    /// 攻撃ステートを設定
    /// </summary>
    /// <param name="_atack">攻撃</param>
    private void SetAtackState(AtackState _atack)
    {
        atackState = _atack;

        animator.SetInteger("AtackValue",(int)_atack + 1);
        // +1としているのはAnimatorの各遷移条件が1から始まるため

        // 攻撃ステートに移る時に1回だけ呼ばれる処理
        switch (_atack)
        {
            case AtackState.Melee1:
                atackPower = Generic.RandomErrorRange(-10.0f, 2.0f);
                break;

            case AtackState.Melee2:
                atackPower = Generic.RandomErrorRange(-15.0f, 2.0f);
                break;

            case AtackState.Jump:
                atackPower = Generic.RandomErrorRange(-20.0f, 3.0f);
                break;
        }
    }

    #endregion
}
