using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillCtrl : MonoBehaviour
{
    [System.Serializable]
    private class SkillParamate // スキルのパラメータ
    {
        public int cost; // 消費アドレナリン
        public int coolTime; // リキャスト時間
        public Image icon; // スキルアイコン
    }

    //-----SerializeField------------------------------------------------------------
    [Header("アドレナリン")]
    [SerializeField] private float maxAdrenaline;   // 最大アドレナリン
    [SerializeField] public Slider adrenalineGauge; // アドレナリンゲージ

    [Header("スキル")]
    [SerializeField] private SkillParamate[] skills = new SkillParamate[4]; // スキルのパラメータ

    [Header("エフェクト")]
    [SerializeField] private GameObject healEffect;   // 回復
    [SerializeField] private GameObject buffEffect;   // 強化

    //-----privateField--------------------------------------------------------------
    private delegate void SkillAction();
    private int atackPowerBuff = 0;
    private PlayerCtrl playerController;
    private Generic.ParamateValue adrenalineValue;
    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------


    #region システム

    private void Awake()
    {
        adrenalineValue = new Generic.ParamateValue(0, maxAdrenaline, 0);
    }

    void Start()
    {
        playerController = GetComponent<PlayerCtrl>();
        adrenalineGauge.value = adrenalineValue.cur / adrenalineValue.max;
    }

    private void Update()
    {
        // アドレナリンゲージが足りなかったらアイコンを薄くする
        foreach (var skill in skills)
        {
            if (skill.cost >= adrenalineValue.cur) skill.icon.fillAmount = 1;
            else skill.icon.fillAmount = 0;
        }
    }

    #endregion


    #region InputSystem関連

    /// <summary>
    /// スキル1発動
    /// </summary>
    /// <param name="_context"></param>
    public void OnSkillOne(InputAction.CallbackContext _context)
    {
        if (!_context.performed) return;

        if( SkillExe(skills[0], () => StartCoroutine(SkillBuffCoroutine(3))))
        {
            Instantiate(buffEffect, this.transform.position, Quaternion.identity, transform);
        }
    }

    /// <summary>
    /// スキル2発動
    /// </summary>
    /// <param name="_context"></param>
    public void OnSkillTwo(InputAction.CallbackContext _context)
    {
        if (!_context.performed) return;

        if( SkillExe(skills[1], () => playerController.HpFluctuation(20)))
        {
            Instantiate(healEffect, this.transform.position, Quaternion.identity, transform);
        }
    }

    /// <summary>
    /// スキル3発動
    /// </summary>
    /// <param name="_context"></param>
    public void OnSkillThree(InputAction.CallbackContext _context)
    {
        if (!_context.performed) return;
        SkillExe(skills[2], () => Debug.Log("スキル3は未実装"));
    }

    /// <summary>
    /// スキル4発動
    /// </summary>
    /// <param name="_context"></param>
    public void OnSkillFour(InputAction.CallbackContext _context)
    {
        if (!_context.performed) return;
        SkillExe(skills[3], () => Debug.Log("スキル4は未実装"));
    }

    #endregion


    #region スキル制御

    /// <summary>
    /// スキルの使用
    /// </summary>
    /// <param name="_skills"></param>
    private bool SkillExe(SkillParamate _skills, SkillAction skillAction)
    {
        // アドレナリンゲージが十分にあるかチェック
        if (adrenalineValue.cur - _skills.cost < adrenalineValue.min) { return false; }
        // クールタイムが終わっているかチェック
        if (_skills.icon.fillAmount != 0) { return false; }

        // クールタイムを設ける
        StartCoroutine(SkillCoolTimeCoroutine(_skills.coolTime, _skills.icon));
        // アドレナリンゲージを消費
        AdrenalineGaugeCalculation(-_skills.cost);
        // スキル内容実行
        skillAction?.Invoke();

        return true;
    }

    /// <summary>
    /// アドレナリンゲージを増減させる
    /// </summary>
    /// <param name="_value"></param>
    public void AdrenalineGaugeCalculation(float _value)
    {
        StartCoroutine(new Generic.CalcuRation().ValueFluctuation(_value, adrenalineGauge, adrenalineValue));
    }

    /// <summary>
    /// バフ量を返す
    /// </summary>
    /// <returns>追加攻撃力</returns>
    public int GetAtackBuff()
    {
        return atackPowerBuff;
    }

    #endregion


    #region コルーチン

    /// <summary>
    /// クールタイム
    /// </summary>
    /// <param name="_coolTime">リキャスト時間</param>
    /// <param name="_icon">アイコン</param>
    /// <returns></returns>
    IEnumerator SkillCoolTimeCoroutine(float _coolTime, Image _icon)
    {
        float curTime = 0f; // 経過時間

        while (curTime < _coolTime)
        {
            _icon.fillAmount = Mathf.Lerp(1, 0, curTime / _coolTime);
            curTime += Time.deltaTime;
            yield return null;
        }
        _icon.fillAmount = 0;
    }

    /// <summary>
    /// 攻撃力upバフの効果
    /// </summary>
    /// <returns></returns>
    IEnumerator SkillBuffCoroutine(int _buffValue)
    {
        atackPowerBuff += _buffValue;
        yield return new WaitForSeconds(10);
        atackPowerBuff -= _buffValue;
    }

    #endregion


}
