using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class EnemyWizardCtrl : EnemyBase
{
    private enum AIState // 状態パターン
    {
        Idle,       // 待機
        Wait,       // 待つ
        Patrol,     // 巡回
        Atack,      // 攻撃
        Damage,     // ダメージ
        Distance,   // 距離をとる
        Warp        // ワープ
    }

    private enum AtackState // 攻撃パターン
    {
        Magic1,     // 魔法1
        Magic2,     // 魔法2
        Magic3,     // 魔法3
        Magic4      // 魔法4
    }

    //-----SerializeField------------------------------------------------------------
    [Header("クールタイム")]
    [SerializeField] private CoolTime atackTime;    // 攻撃クールタイム
    [SerializeField] private CoolTime distanceTime; // 距離をとるクールタイム
    [SerializeField] private CoolTime warpTime;     // ワープクールタイム
    [Header("ゲージ")]
    [SerializeField] private Slider hpGage; // HPゲージ
    [Header("遠距離攻撃")]
    [SerializeField] private GameObject fireBall;   // ファイアボール

    [Header("エフェクト")]
    [SerializeField] private GameObject warpEffect; // ワープエフェクト
    [SerializeField] private GameObject warpShadowEffect; // 影エフェクト

    //-----privateField--------------------------------------------------------------
    private EnemyAreaBase enemyArea;    // スポーンしたエリア
    private Coroutine coroutine;    // コルーチン
    private AIState aiState = AIState.Patrol;
    private AtackState atackState = AtackState.Magic1;
    private bool isWarp = false;   // ワープ中かどうか
    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    # region システム
    protected override void Start()
    {
        base.Start();

        hpGage.value = hpValue.cur / hpValue.max;

        // 目的地をエリア内に設定
        enemyArea = this.transform.parent.GetComponent<EnemyAreaBase>();
        agent.destination = enemyArea.GetRandomPosInSphere();

        // クールタイムの初期値設定
        atackTime.goal = Generic.RandomErrorRange(atackTime.def, 2f);
        distanceTime.goal = Generic.RandomErrorRange(distanceTime.def, 2f);
        warpTime.goal = Generic.RandomErrorRange(warpTime.def, 2f);
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
            case AIState.Wait:
                Wait();
                break;
            case AIState.Patrol:
                Patrol();
                break;
            case AIState.Atack:
                Atack();
                break;
            case AIState.Damage:
                Damage();
                break;
            case AIState.Distance:
                Distance();
                break;
            case AIState.Warp:
                Warp(); 
                break;
        }
    }

    #endregion

    #region 状態ステート
    private void Idle()
    {
        transform.LookAt(playerPos);

        if(isWarp) { return; }

        // 攻撃クールタイムが終わったら攻撃ステートに移る
        atackTime.cur += Time.deltaTime;
        if(atackTime.cur > atackTime.goal)
        {
            ChangeAIState(AIState.Atack);
            atackTime.cur = 0;
            return;
        }

        // 距離をとるクールタイムが終わったら離れるステートに移る
        distanceTime.cur += Time.deltaTime;
        if (distanceTime.cur > distanceTime.goal)
        {
            ChangeAIState(AIState.Distance);
            distanceTime.cur = 0;
        }
    }

    private void Wait()
    {
        // 動いているコルーチンがある場合はスキップ
        if(coroutine == null)
        {
            // 待機時間が終わったら次の地点を決める
            coroutine = StartCoroutine(SetNextPatrolPoint());
        }

        // プレイヤーが近くに来たら待機を解除
        if (DistanceFromPlayer() <= range.far)
        {
            ChangeAIState(AIState.Idle);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }

    private void Patrol()
    {
        // 目標地点に着いたら次の地点を決める
        if(agent.remainingDistance < 2f)
        {
            ChangeAIState(AIState.Wait);
        }

        // プレイヤーが近くに来たら待機を解除
        if (DistanceFromPlayer() <= range.far)
        {
            ChangeAIState(AIState.Idle);
        }
    }

    private void Atack()
    {
        // 攻撃モーションの終了
        if(AnimationEnd("Atack"))
        {
            //Debug.Log("攻撃終了");

            ChangeAIState(AIState.Idle);
            canDamageAnim = true;
        }
        #region 攻撃ステート


        #endregion
    }

    private void Damage()
    {
        // ワープクールタイムが終わったら待機ステートに移る
        warpTime.cur += Time.deltaTime;
        if (warpTime.cur > warpTime.goal)
        {
            ChangeAIState(AIState.Warp);
            warpTime.cur = 0;
            rigidbody.isKinematic = true;
            canDamageAnim = true;
            return;
        }

        // 攻撃クールタイムが終わったら攻撃ステートに移る
        atackTime.cur += Time.deltaTime;
        if (atackTime.cur > atackTime.goal)
        {
            ChangeAIState(AIState.Atack);
            atackTime.cur = 0;
            rigidbody.isKinematic = true;
            return;
        }

        // アニメーションが終了したら
        if (AnimationEnd("Damage"))
        {
            rigidbody.isKinematic = true;
            canDamageAnim = true;

            ChangeAIState(AIState.Idle);
        }
    }

    private void Distance()
    {
        transform.LookAt(playerPos);

        // プレイヤーが近くに来たら
        if (DistanceFromPlayer() <= range.atack)
        {
            // プレイヤーから離れる方向に移動
            Vector3 directionAwayFromPlayer = (transform.position - playerPos.position).normalized;
            Vector3 retreatPosition = transform.position + directionAwayFromPlayer * range.atack;
            agent.destination = retreatPosition;
        }

        // プレイヤーが遠くに行ったら
        if (DistanceFromPlayer() >= range.far)
        {
            // プレイヤーに近づく方向に移動
            agent.destination = playerPos.position;
        }

        // 攻撃クールタイムが終わったら攻撃ステートに移る
        atackTime.cur += Time.deltaTime;
        if (atackTime.cur > atackTime.goal)
        {
            ChangeAIState(AIState.Atack);
            atackTime.cur = 0;
            return;
        }

        // ワープクールタイムが終わったら待機ステートに移る
        warpTime.cur += Time.deltaTime;
        if (warpTime.cur > warpTime.goal)
        {
            ChangeAIState(AIState.Warp);
        }
    }

    private void Warp()
    {

    }

    #endregion

    #region エネミーの制御

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="_damage">ダメージ</param>
    public override async void TakeDamage(int _damage)
    {
        if (isDie) { return; }

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
            return;
        }

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

        ChangeAIState(AIState.Warp);
    }

    /// <summary>
    /// 状態ステートを設定
    /// ステート毎のアニメーションを再生
    /// </summary>
    /// <param name="_nextState">次のステート</param>
    private void ChangeAIState(AIState _nextState)
    {
        if (isDie) { return; }

        aiState = _nextState;   // ステート更新


        // アニメーション更新
        foreach (var animState in animator.parameters)
        {
            // トリガー以外はスキップ
            if (animState.type != AnimatorControllerParameterType.Trigger) { continue; }

            if(animState.name == $"{_nextState}")
            {
                animator.SetTrigger($"{_nextState}");   // 更新
            }
            else　      
            {
                animator.ResetTrigger($"{animState.name}"); // 他をリセット
            }
        }

        // クールタイムをランダムに再設定
        distanceTime.goal = Generic.RandomErrorRange(distanceTime.def, 2f); // 距離を測るまでの時間
        atackTime.goal = Generic.RandomErrorRange(atackTime.def, 1f);       // 攻撃までの時間
        warpTime.goal = Generic.RandomErrorRange(warpTime.def, 2f);         // ワープまでの時間

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

            case AIState.Atack:
                // プレイヤーとの距離によって攻撃パターンを変える
                AtackState atack;
                if(DistanceFromPlayer() <= range.atack)
                {
                    atack = (AtackState)Random.Range(0, 1); // 近接攻撃
                }
                else
                {
                    atack = (AtackState)Random.Range(2, 4); // 遠距離攻撃
                }
                SetAtackState(atack);
                canDamageAnim = false;

                agent.speed = speed.zero;
                agent.destination = enemyPos.position;
                break;

            case AIState.Damage:
                agent.speed = speed.zero;
                agent.destination = enemyPos.position;
                break;

            case AIState.Distance:
                agent.speed = speed.slow;
                agent.destination = enemyPos.position;
                break;

            case AIState.Warp:
                StartCoroutine(WarpCoroutine());
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

        animator.SetInteger("AtackValue", (int)_atack + 1);
        // +1としているのはAnimatorの遷移条件が1から始まるため

        // 攻撃ステートに移る時に1回だけ呼ばれる処理
        switch (_atack)
        {
            case AtackState.Magic1:
                atackPower = Generic.RandomErrorRange(-10.0f, 2.0f);
                //Debug.Log("魔法1");
                break;

            case AtackState.Magic2:
                atackPower = Generic.RandomErrorRange(-15.0f, 2.0f);
                //Debug.Log("魔法2");
                break;

            case AtackState.Magic3:
                atackPower = Generic.RandomErrorRange(-20.0f, 3.0f);
                //Debug.Log("魔法3");
                break;

            case AtackState.Magic4:
                atackPower = Generic.RandomErrorRange(-15.0f, 4.0f);
                //Debug.Log("魔法4");
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
        float waitTime = 2.5f; // 待機時間
        yield return new WaitForSeconds(waitTime);
        coroutine = null;
        ChangeAIState(AIState.Patrol);
    }

    /// <summary>
    /// ワープ移動
    /// </summary>
    /// <returns></returns>
    IEnumerator WarpCoroutine()
    {
        float waitTime = 1.5f; // 待機時間
        isWarp = true;

        // 消えるエフェクト生成
        Vector3 efectPos = this.transform.position;
        Instantiate(warpEffect, efectPos, Quaternion.identity);

        // ワープ
        Vector3 warpPos = enemyArea.GetRandomPosInSphere(); // 範囲内のランダムな座標を取得
        enemyPos.position = warpPos;    // ワープ地点に座標移動

        // 影エフェクトの生成と移動
        var shadow = Instantiate(warpShadowEffect, efectPos, Quaternion.identity);
        warpPos.y = 1f;   // ワープ地点の高さを上げる
        shadow.transform.position = new(shadow.transform.position.x,warpPos.y,shadow.transform.position.z);
        shadow.transform.DOMove(warpPos, waitTime)    // ワープ地点まで残像を移動
                                .SetLink(gameObject);

        // 姿を非表示
        this.transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSeconds(waitTime);

        isWarp = false;
        warpTime.cur = 0;

        // 影エフェクト削除
        Destroy(shadow);

        // エフェクト生成
        efectPos = this.transform.position;
        Instantiate(warpEffect, efectPos, Quaternion.identity);

        // 姿を表示
        this.transform.GetChild(0).gameObject.SetActive(true);

        ChangeAIState(AIState.Idle);
    }

    #endregion

    #region アニメーションEvent

    /// <summary>
    /// 遠距離魔法攻撃
    /// </summary>
    public void FireBall()
    {
        GameObject obj; // ファイアボール
        float shotSpeed = 1000; // 発射速度

        // 生成座標
        Vector3 pos = this.transform.position;
        Quaternion rot = this.transform.rotation;
        pos.y += 1f;

        // ファイアボールを生成
        obj = Instantiate(fireBall, pos, rot);
        // 正面方向に飛ばす
        obj.GetComponent<Rigidbody>().AddForce(transform.forward * shotSpeed);
        // ダメージを設定
        obj.GetComponent<EnemyAtack>().enemy = this;
    }
    #endregion
}

