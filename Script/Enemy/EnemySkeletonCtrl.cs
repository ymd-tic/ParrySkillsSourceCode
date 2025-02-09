using System.Collections;
using UnityEngine;

public class EnemySkeletonCtrl : EnemyBase
{
    private enum AIState // 状態パターン
    {
        Idle,       // 待機
        Wait,       // 待つ
        Patrol,     // 巡回
        Chase,      // 追跡
        Atack,      // 攻撃
        Damage,     // ダメージ
        KnockBack,  // ノックバック
        Distance    // 距離をとる
    }

    private enum AtackState // 攻撃パターン
    {
        Melee1,     // 近接1
    }
    //-----SerializeField------------------------------------------------------------
    [Header("クールタイム")]
    [SerializeField] private CoolTime atackTime;    // 攻撃クールタイム



    //-----privateField--------------------------------------------------------------
    private EnemyAreaBase enemyArea;    // スポーンしたエリア
    private Coroutine coroutine;    // コルーチン
    private AIState aiState = AIState.Patrol;
    private AtackState atackState = AtackState.Melee1;



    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------




    #region システム

    protected override void Start()
    {
        base.Start();

        // 目的地をエリア内に設定
        enemyArea = this.transform.parent.GetComponent<EnemyAreaBase>();
        agent.destination = enemyArea.GetRandomPosInSphere();
    }

    protected override void Update()
    {
        base.Update();

        // 死んだら何もしない
        if(isDie) { return; }    

        switch (aiState)
        {
            case AIState.Idle:
                Idle();
                break;

            case AIState.Wait:
                Wait();
                break;

            case AIState.Patrol:
                Patrol();
                break;

            case AIState.Chase:
                Chase();
                break;

            case AIState.Atack:
                Atack();
                break;

            case AIState.Damage:
                Damage();
                break;

            case AIState.KnockBack:
                KnockBack();
                break;

            case AIState.Distance:
                Distance();
                break;
        }
        //Debug.Log(aiState);

    }

    #endregion


    #region 状態ステート

    private void Idle()
    {
        // プレイヤーとの距離が追跡範囲外なら
        if (DistanceFromPlayer() > range.far)
        {
            ChangeAIState(AIState.Patrol);
        }
        // プレイヤーとの距離が攻撃範囲外なら
        else if (DistanceFromPlayer() > range.atack)
        {
            ChangeAIState(AIState.Chase);
        }
        else if(DistanceFromPlayer() > range.near)
        {
            // 攻撃クールタイムが終わったら攻撃ステートに移る
            atackTime.cur += Time.deltaTime;
            if (atackTime.cur > atackTime.goal)
            {
                ChangeAIState(AIState.Atack);
                atackTime.cur = 0;
            }
        }
        else if(DistanceFromPlayer() <= range.near)
        {
            ChangeAIState(AIState.Distance);
        }
    }

    private void Wait()
    {
        if (coroutine == null)
        {
            // 待機時間が終わったら次の地点を決める
            coroutine = StartCoroutine(SetNextPatrolPoint());
        }

        // プレイヤーが近くに来たら待機を解除
        if (DistanceFromPlayer() <= range.far)
        {
            ChangeAIState(AIState.Chase);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }

    private void Patrol()
    {
        // 目標地点に到着したら
        if (agent.remainingDistance < 2f)
        {
            ChangeAIState(AIState.Wait);
        }

        // プレイヤーとの距離が追跡範囲内なら
        if (DistanceFromPlayer() <= range.far)
        {
            ChangeAIState(AIState.Chase);
            return;
        }
    }

    private void Chase()
    {
        agent.destination = playerPos.position;

        // プレイヤーとの距離が追跡範囲外なら
        if (DistanceFromPlayer() > range.far)
        {
            ChangeAIState(AIState.Patrol);
        }
        // プレイヤーとの距離が攻撃範囲外なら
        else if (DistanceFromPlayer() > range.atack)
        {

        }
        else if (DistanceFromPlayer() > range.near)
        {
            ChangeAIState(AIState.Idle);
        }
        else if (DistanceFromPlayer() <= range.near)
        {
            ChangeAIState(AIState.Distance);
        }
    }

    private void Atack()
    {
        // 攻撃分岐
        switch(atackState)
        {
            case AtackState.Melee1:
                Melee1();
                break;
        }

        // 攻撃モーションの終了
        if (AnimationEnd("Atack"))
        {
            // プレイヤーとの距離が
            // 追跡範囲外
            if (DistanceFromPlayer() > range.far)
            {
                ChangeAIState(AIState.Patrol);
            }
            // 攻撃範囲外
            else if (DistanceFromPlayer() > range.atack)
            {
                ChangeAIState(AIState.Chase);
            }
            else if (DistanceFromPlayer() > range.near)
            {
                ChangeAIState(AIState.Idle);
            }
            else
            {
                ChangeAIState(AIState.Distance);
            }
            canDamageAnim = true;
        }

        #region 攻撃パターン

        void Melee1()
        {

        }

        #endregion

    }

private void Damage()
    {
        // 攻撃クールタイムが終わったら攻撃ステートに移る
        atackTime.cur += Time.deltaTime;
        if (atackTime.cur > atackTime.goal)
        {
            ChangeAIState(AIState.Atack);
            rigidbody.isKinematic = true;
            return;
        }

        if (AnimationEnd("Damage"))
        {
            rigidbody.isKinematic = true;
            canDamageAnim = true;

            // プレイヤーとの距離が
            // 追跡範囲外
            if (DistanceFromPlayer() > range.far)
            {
                ChangeAIState(AIState.Patrol);
            }
            // 攻撃範囲外
            else if (DistanceFromPlayer() > range.atack)
            {
                ChangeAIState(AIState.Chase);
            }
            else if (DistanceFromPlayer() > range.near)
            {
                ChangeAIState(AIState.Idle);
            }
            else
            {
                ChangeAIState(AIState.Distance);
            }
        }
    }

    private void KnockBack()
    {
        if(AnimationEnd("KnockBack"))
        {
            rigidbody.isKinematic = true;
            canDamageAnim = true;

            // プレイヤーとの距離が追跡範囲内なら
            if (DistanceFromPlayer() <= range.far)

            {   // かつ攻撃範囲内なら
                if (DistanceFromPlayer() <= range.atack)
                {
                    ChangeAIState(AIState.Idle);

                    if(DistanceFromPlayer() <= range.near)
                    {
                        ChangeAIState(AIState.Distance);
                    }

                    return;
                }

                ChangeAIState(AIState.Chase);
            }
            else
            {
                ChangeAIState(AIState.Patrol);
            }
        }
    }


    private void Distance()
    {
        transform.LookAt(playerPos);
        // プレイヤーから離れる方向に移動
        Vector3 directionAwayFromPlayer = (transform.position - playerPos.position).normalized;
        Vector3 retreatPosition = transform.position + directionAwayFromPlayer * range.near;

        agent.destination = retreatPosition;

        // 攻撃クールタイムが終わったら攻撃ステートに移る
        atackTime.cur += Time.deltaTime;
        if (atackTime.cur > atackTime.goal)
        {
            ChangeAIState(AIState.Atack);
            return;
        }

        // 一定の距離を取ったらIdle状態に遷移
        if (DistanceFromPlayer() > range.near)
        {
            ChangeAIState(AIState.Idle);
        }
    }
    #endregion


    #region エネミーの制御

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="_damage">ダメージ</param>
    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        if (canDamageAnim)
        {
            rigidbody.isKinematic = false;
            ChangeAIState(AIState.Damage);
            transform.LookAt(playerPos);
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
        if(isDie) {return;}

        aiState = _nextState;   // ステート更新

        foreach (var animState in animator.parameters)
        {
            // トリガー以外はスキップ
            if (animState.type != AnimatorControllerParameterType.Trigger) { continue; }

            if (animState.name == $"{_nextState}")
            {
                animator.SetTrigger($"{_nextState}");   // 更新
            }
            else
            {
                animator.ResetTrigger($"{animState.name}"); // 他をリセット
            }
        }

        // 次のステートに移る時に1回だけ呼ばれる処理
        switch (_nextState)
        {
            case AIState.Idle:
                agent.speed = speed.zero;
                agent.destination = enemyPos.position;
                break;

            case AIState.Wait:
                agent.speed = speed.zero;
                agent.destination = enemyPos.position;
                break;

            case AIState.Patrol:
                agent.speed = speed.slow;
                agent.destination = enemyArea.GetRandomPosInSphere();
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
                atackTime.cur = 0;
                atackTime.goal = Generic.RandomErrorRange(atackTime.def, 0.5f);

                canDamageAnim = false;
                transform.LookAt(playerPos.position);
                agent.speed = speed.zero;
                agent.destination = enemyPos.position;
                break;

            case AIState.Damage:
                agent.speed = speed.zero;
                agent.destination = enemyPos.position;
                break;

            case AIState.KnockBack:
                agent.speed = speed.zero;
                agent.destination = enemyPos.position;
                break;

            case AIState.Distance:
                agent.speed = speed.slow;
                agent.destination = enemyPos.position;
                break;
        }

        Debug.Log($"{_nextState}ステートに更新");
    }

    /// <summary>
    /// 攻撃ステートを設定
    /// </summary>
    /// <param name="_atack">攻撃</param>
    private void SetAtackState(AtackState _atack)
    {
        atackState = _atack;

        animator.SetInteger("AtackValue", (int)_atack + 1);
        // +1としているのはAnimatorの各遷移条件が1から始まるため

        // 攻撃ステートに移る時に1回だけ呼ばれる処理
        switch (_atack)
        {
            case AtackState.Melee1:
                atackPower = Generic.RandomErrorRange(-10.0f, 2.0f);
                break;
        }
    }

    #endregion


    #region コルーチン

    /// <summary>
    /// 巡回地点に着いたら一定時間待機してから次の地点を決める
    /// </summary>
    /// <returns></returns>
    IEnumerator SetNextPatrolPoint()
    {
        float waitTime = 5.5f; // 待機時間
        yield return new WaitForSeconds(waitTime);
        ChangeAIState(AIState.Patrol);
    }

    #endregion
}