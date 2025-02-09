using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillCtrl : MonoBehaviour
{
    [System.Serializable]
    private class SkillParamate // �X�L���̃p�����[�^
    {
        public int cost; // ����A�h���i����
        public int coolTime; // ���L���X�g����
        public Image icon; // �X�L���A�C�R��
    }

    //-----SerializeField------------------------------------------------------------
    [Header("�A�h���i����")]
    [SerializeField] private float maxAdrenaline;   // �ő�A�h���i����
    [SerializeField] public Slider adrenalineGauge; // �A�h���i�����Q�[�W

    [Header("�X�L��")]
    [SerializeField] private SkillParamate[] skills = new SkillParamate[4]; // �X�L���̃p�����[�^

    [Header("�G�t�F�N�g")]
    [SerializeField] private GameObject healEffect;   // ��
    [SerializeField] private GameObject buffEffect;   // ����

    //-----privateField--------------------------------------------------------------
    private delegate void SkillAction();
    private int atackPowerBuff = 0;
    private PlayerCtrl playerController;
    private Generic.ParamateValue adrenalineValue;
    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------


    #region �V�X�e��

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
        // �A�h���i�����Q�[�W������Ȃ�������A�C�R���𔖂�����
        foreach (var skill in skills)
        {
            if (skill.cost >= adrenalineValue.cur) skill.icon.fillAmount = 1;
            else skill.icon.fillAmount = 0;
        }
    }

    #endregion


    #region InputSystem�֘A

    /// <summary>
    /// �X�L��1����
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
    /// �X�L��2����
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
    /// �X�L��3����
    /// </summary>
    /// <param name="_context"></param>
    public void OnSkillThree(InputAction.CallbackContext _context)
    {
        if (!_context.performed) return;
        SkillExe(skills[2], () => Debug.Log("�X�L��3�͖�����"));
    }

    /// <summary>
    /// �X�L��4����
    /// </summary>
    /// <param name="_context"></param>
    public void OnSkillFour(InputAction.CallbackContext _context)
    {
        if (!_context.performed) return;
        SkillExe(skills[3], () => Debug.Log("�X�L��4�͖�����"));
    }

    #endregion


    #region �X�L������

    /// <summary>
    /// �X�L���̎g�p
    /// </summary>
    /// <param name="_skills"></param>
    private bool SkillExe(SkillParamate _skills, SkillAction skillAction)
    {
        // �A�h���i�����Q�[�W���\���ɂ��邩�`�F�b�N
        if (adrenalineValue.cur - _skills.cost < adrenalineValue.min) { return false; }
        // �N�[���^�C�����I����Ă��邩�`�F�b�N
        if (_skills.icon.fillAmount != 0) { return false; }

        // �N�[���^�C����݂���
        StartCoroutine(SkillCoolTimeCoroutine(_skills.coolTime, _skills.icon));
        // �A�h���i�����Q�[�W������
        AdrenalineGaugeCalculation(-_skills.cost);
        // �X�L�����e���s
        skillAction?.Invoke();

        return true;
    }

    /// <summary>
    /// �A�h���i�����Q�[�W�𑝌�������
    /// </summary>
    /// <param name="_value"></param>
    public void AdrenalineGaugeCalculation(float _value)
    {
        StartCoroutine(new Generic.CalcuRation().ValueFluctuation(_value, adrenalineGauge, adrenalineValue));
    }

    /// <summary>
    /// �o�t�ʂ�Ԃ�
    /// </summary>
    /// <returns>�ǉ��U����</returns>
    public int GetAtackBuff()
    {
        return atackPowerBuff;
    }

    #endregion


    #region �R���[�`��

    /// <summary>
    /// �N�[���^�C��
    /// </summary>
    /// <param name="_coolTime">���L���X�g����</param>
    /// <param name="_icon">�A�C�R��</param>
    /// <returns></returns>
    IEnumerator SkillCoolTimeCoroutine(float _coolTime, Image _icon)
    {
        float curTime = 0f; // �o�ߎ���

        while (curTime < _coolTime)
        {
            _icon.fillAmount = Mathf.Lerp(1, 0, curTime / _coolTime);
            curTime += Time.deltaTime;
            yield return null;
        }
        _icon.fillAmount = 0;
    }

    /// <summary>
    /// �U����up�o�t�̌���
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
