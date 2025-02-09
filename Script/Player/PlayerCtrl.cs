using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngineInternal;

public class PlayerCtrl : MonoBehaviour
{
    //-----SerializeField------------------------------------------------------------
    [Header("ステータス")]
    [SerializeField] private float maxHp = 10;      // 最大HP
    [SerializeField] private float maxStamina = 10; // 最大スタミナ
    [SerializeField] private float moveSpeed;       // 移動速度
    [SerializeField] private float rotSpeed;        // 方向転換速度
    [SerializeField] private float rollingSpeed;    // 回避速度
    [SerializeField] private float staminaHealSpeed;// スタミナ回復速度
    [SerializeField] private float staminaUseValue; // スタミナ消費量

    [Header("メインカメラ")]        
    [SerializeField] private Camera mainCamera;     // メインカメラ

    [Header("ゲージ")]
    [SerializeField] private Slider hpGage;         // HPゲージ
    [SerializeField] private Slider staminaGage;    // スタミナゲージ


    [Header("コライダー")]    
    [SerializeField] public BoxCollider parryCollider; // パリィ当たり判定

    [Header("エフェクト")] 
    [SerializeField] public ParticleSystem[] slashEfects = new ParticleSystem[4]; // 剣の軌跡


    //-----privateField--------------------------------------------------------------
    private Vector3 moveVector;     // 移動方向
    private Vector3 rollingVector;  // 回避方向
    private float horizontal; // X軸
    private float vertical;   // Z軸
    private float curAtackState = 0;    // 現在の攻撃段数
    private bool isCanCombo = false;    // コンボ可能フラグ (true => コンボ可能 false => コンボ不可)
    private bool isCanAtack = true;     // 攻撃可能フラグ   (true => 攻撃可能   false => 攻撃不可)
    private bool isCanRolling = false;  // 回避可能フラグ   (true => 回避可能   false => 回避不可)
    private bool isNowRolling = false;  // 回避状態フラグ   (true => 回避中     false => 回避していない)

    private CharacterController characterController;
    private SkillCtrl skillController;
    private Coroutine rollingCoroutine;
    private Coroutine parryCoroutine;
    private PlayerHitManager playerHitManager;
    private Generic.ParamateValue hpValue;
    private Generic.ParamateValue staminaValue;

    //-----publicField---------------------------------------------------------------
    [System.NonSerialized] public int atackPower = 1;   // 攻撃力
    [System.NonSerialized] public Animator animator;


    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    #region システム

    private void Awake()
    {
        hpValue = new Generic.ParamateValue(maxHp, maxHp, 0);
        staminaValue = new Generic.ParamateValue(maxStamina, maxStamina, 0);
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        skillController = GetComponent<SkillCtrl>();
        animator = GetComponent<Animator>();
        playerHitManager = GetComponent<PlayerHitManager>();
        hpGage.value = hpValue.cur / hpValue.max;
        staminaGage.value = staminaValue.cur / staminaValue.max;
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Damage")) return;

        // 回避中じゃなければスタミナ回復
        if (!isNowRolling)
        {
            staminaValue.cur = Mathf.Clamp(staminaValue.cur += staminaHealSpeed * Time.deltaTime, staminaValue.min, staminaValue.max);
            staminaGage.value = staminaValue.cur / staminaValue.max;
        }

        MovePosition(); // 移動
        MoveRolling();  // 回避
    }
    #endregion


    #region InputSystem関連

    /// <summary>
    /// 移動の呼び出し
    /// </summary>
    /// <param name="_context"></param>
    public void OnMove(InputAction.CallbackContext _context)
    {
        horizontal = _context.ReadValue<Vector2>().x;
        vertical = _context.ReadValue<Vector2>().y;

        float moveAmount = Mathf.Abs(horizontal) + Mathf.Abs(vertical); // 入力しているかを0～1までで格納
        animator.SetFloat("Run", moveAmount);
    }

    /// <summary>
    /// 通常攻撃の呼び出し
    /// </summary>
    /// <param name="_context"></param>
    public void OnAtack(InputAction.CallbackContext _context)
    {
        if (!_context.performed) { return; } // 瞬間的に押していなかったらreturn
        if (!isCanAtack) { return; }        // 攻撃が可能じゃなかったらreturn
        if (isNowRolling) { return; }          // 回避中だったらreturn
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Damage")) { return; }

        curAtackState = animator.GetFloat("Atack");

        if (isCanCombo)
        {
            if (curAtackState >= 4)
            {
                //animator.SetFloat("Atack", 0f);
                isCanAtack = false;
                StartCoroutine(ComboEnd());
                return;
            }

            isCanCombo = false;
            curAtackState += 1f;



            animator.SetFloat("Atack", curAtackState);
        }
        else
        {
            if (curAtackState == 0f)
            {
                curAtackState += 1f;
                animator.SetFloat("Atack", 1f);
            }
        }

        // 攻撃力設定
        atackPower = (int)curAtackState * 2 + 10;
        atackPower = (int)Generic.RandomErrorRange(atackPower, 2f);
        // バフを追加
        atackPower += skillController.GetAtackBuff();

        // 敵が近くにいたら敵の方を向く
        if (AreaManager.enemyList.Count != 0)
        {
            Vector3 nearEnemy = AreaManager.NearEnemy(this.gameObject);

            if (Vector3.Distance(transform.position, nearEnemy) <= 4)
            {
                transform.LookAt(nearEnemy);
            }
        }
    }

    /// <summary>
    /// 回避の呼び出し
    /// </summary>
    /// <param name="_context"></param>
    public void OnRolling(InputAction.CallbackContext _context)
    {
        if (!_context.performed) { return; }
        if (staminaValue.cur < staminaUseValue) { return; }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Damage")) { return; }

        if (isCanRolling)
        {
            isCanRolling = false;
            StopCoroutine(rollingCoroutine);
            SetRolling();
            animator.CrossFade("Rolling", 0.1f, 0, 0f);
            return;
        }

        if (!isNowRolling)
        {
            SetRolling();
            animator.SetTrigger("Rolling");
        }
    }

    #endregion


    #region プレイヤーの制御

    #region 移動・回転

    /// <summary>
    /// プレイヤーの移動
    /// </summary>
    void MovePosition()
    {
        // カメラの前方向
        Vector3 cameraForward = Vector3.Scale(mainCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

        // プレイヤーの前方向
        moveVector = cameraForward * vertical + mainCamera.transform.right * horizontal;

        if (animator.GetFloat("Atack") != 0 || isNowRolling) { return; }

        // キャラクターコントローラーで移動
        characterController.Move(moveSpeed * moveVector * Time.deltaTime);

        // プレイヤーが動いていたら
        if (moveVector != Vector3.zero)
        {
            MoveRotation(moveVector);
        }
    }

    /// <summary>
    /// プレイヤーを入力方向に徐々に回転させる
    /// </summary>
    /// <param name="_moveVector"></param> プレイヤーの進行方向
    void MoveRotation(Vector3 _moveVector)
    {
        // 回転する角度を計算
        Quaternion deg = Quaternion.LookRotation(_moveVector);

        // 回転に速度を付ける
        transform.rotation = Quaternion.Lerp(transform.rotation, deg, Time.deltaTime * rotSpeed);
    }

    #endregion


    #region 回避

    /// <summary>
    /// 回避時の初期設定
    /// </summary>
    void SetRolling()
    {
        rollingCoroutine = StartCoroutine(RollingCoroutine());
        ParryColliderOff();
        StartCoroutine(new Generic.CalcuRation().ValueFluctuation(-staminaUseValue, staminaGage, staminaValue));
        animator.SetFloat("Atack", 0);
        // 移動していなかったら正面、してたら移動方向
        if (vertical == 0 && horizontal == 0)
        {
            rollingVector = transform.forward;
        }
        else
        {
            rollingVector = moveVector;
        }
        transform.rotation = Quaternion.LookRotation(rollingVector, Vector3.up);

    }

    /// <summary>
    /// 回避時に前方に移動
    /// </summary>
    void MoveRolling()
    {
        if (!isNowRolling) { return; }

        characterController.Move(rollingVector * rollingSpeed * Time.deltaTime);
    }

    #endregion


    #region その他

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="_damage"></param>
    public async void TakeDamage(float _damage)
    {
        Debug.Log(_damage);

        animator.SetTrigger("Damage");
        animator.SetFloat("Atack", 0f);
        ParryColliderOff();

        // HPが減るまで待つ
        await new Generic.CalcuRation().ValueFluctuation(_damage,hpGage,hpValue).AsTask(this);

        // HPが0以下になったら
        if (hpValue.cur <= hpValue.min)
        {
            var sceneManager = GameObject.Find("SceneManager").GetComponent<MySceneManager>();
            sceneManager.GameFinish(MySceneManager.GameEndStatus.OVER);
        }
    }

    /// <summary>
    /// パリィコライダーをOff
    /// </summary>
    private void ParryColliderOff()
    {
        parryCollider.enabled = false;
        StopCoroutine(ParryColliderCoroutine());
    }

    /// <summary>
    /// プレイヤーHPを増減させる
    /// </summary>
    /// <param name="_value">増減値</param>
    public void HpFluctuation(float _value)
    {
        StartCoroutine(new Generic.CalcuRation().ValueFluctuation(_value, hpGage, hpValue));
    }
    #endregion

    #endregion


    #region アニメーションEvent

    /// <summary>
    /// 攻撃が終わったらコンボできる状態にする
    /// </summary>
    void AttackEnd()
    {
        isCanCombo = true;
    }

    /// <summary>
    /// 攻撃モーションが終わったら攻撃をやめる
    /// </summary>
    void AttackAnimationEnd()
    {
        isCanCombo = false;
        animator.SetFloat("Atack", 0f);
        animator.ResetTrigger("Rolling");
    }

    /// <summary>
    /// 攻撃判定が出たらエフェクトを出す
    /// </summary>
    void SlashEfectr()
    {
        if(animator.GetFloat("Atack") == 0f) {return;}

        slashEfects[(int)animator.GetFloat("Atack") - 1].Play();
        parryCoroutine = StartCoroutine(ParryColliderCoroutine());
    }

    #endregion


    #region　コルーチン

    /// <summary>
    /// 当たり判定を再度出す
    /// </summary>
    /// <returns></returns>
    IEnumerator RollingCoroutine()
    {
        float rollingTime = 0.5f;  // 連続して回避出来る時間
        float continuousRolingTime = 0.6f; // 回避から移動できる時間
        isCanRolling = false;
        isNowRolling = true;
        playerHitManager.enabled = false;

        yield return new WaitForSeconds(rollingTime);
        isCanRolling = true;
        playerHitManager.enabled = true;

        yield return new WaitForSeconds(continuousRolingTime - rollingTime);
        isCanRolling = false;
        isNowRolling = false;
    }

    /// <summary>
    /// コンボ攻撃の終了からもう一度攻撃が出来るまでの処理
    /// </summary>
    /// <returns></returns>
    IEnumerator ComboEnd()
    {
        float canAtackTime = 0f; // 攻撃が出来るようになるまでの時間
        yield return new WaitForSeconds(canAtackTime);
        isCanAtack = true;
    }

    /// <summary>
    /// エフェクトが出たタイミングでパリィ判定を出す
    /// </summary>
    /// <returns></returns>
    IEnumerator ParryColliderCoroutine()
    {
        float parryOnTime = 0.2f; // パリィの当たり判定がONになる時間
        parryCollider.enabled = true;
        yield return new WaitForSeconds(parryOnTime);
        parryCollider.enabled = false;
    }

    #endregion
}
