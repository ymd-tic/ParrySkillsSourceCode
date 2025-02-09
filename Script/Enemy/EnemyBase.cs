using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Serializable]
    protected class CoolTime // �N�[���^�C��
    {
        public float def = 0; // �f�t�H���g����
        [NonSerialized] public float cur = 0;   // ����
        [NonSerialized] public float goal = 0;  // �ڕW
    }

    [Serializable]
    protected class Range // �s���p�^�[���͈�
    {
        public float far = 0;   // ����
        public float near = 0;  // �߂�
        public float atack = 0; // �U������
    }

    [Serializable]
    protected class Speed // �ړ����x
    {
        public float fast = 0;  // ����
        public float slow = 0;  // �x��
        [NonSerialized]
        public readonly float zero = 0;  // ��~
    }

    //-----SerializeField------------------------------------------------------------
    [Header("�X�e�[�^�X")]          
    [SerializeField] private float maxHp = 100; // �ő�HP
    [SerializeField] protected Speed speed;     // �ړ����x
    [SerializeField] protected Range range;     // �s���p�^�[���͈�

    [Header("�G�t�F�N�g")]
    [SerializeField] private ParticleSystem parryEfect; // �p���B�G�t�F�N�g
    [SerializeField] protected GameObject damageTextObj;  // �_���[�WUI


    //-----privateField--------------------------------------------------------------
    private CapsuleCollider capsuleCollider;// �R���C�_�[
    private EnemyAudioCtrl audioCtrl;       // �I�[�f�B�I

    //-----publicField---------------------------------------------------------------
    [NonSerialized]public float atackPower = 10;


    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------
    protected Transform playerPos;  // �v���C���[���W
    protected Transform enemyPos;   // �G���W
    protected Generic.ParamateValue hpValue;  // HP
    protected bool canDamageAnim = true;   // �_���[�W�A�j���[�V�����̍Đ��t���O
    protected bool isDie = false;          // ���S�t���O
    protected NavMeshAgent agent;
    protected Animator animator;
    protected new Rigidbody rigidbody;
    protected TMP_Text damageText;            // �_���[�WUI�e�L�X�g


    #region �V�X�e��

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
    /// �v���C���[�Ƃ̋�����Ԃ�
    /// </summary>
    /// <returns>����</returns>
    protected float DistanceFromPlayer()
    {
        return Vector3.Distance(playerPos.position, enemyPos.position);
    }

    #endregion


    #region �G�l�~�[�̐���

    /// <summary>
    /// �_���[�W���󂯂�
    /// </summary>
    /// <param name="_damage">�_���[�W</param>
    public virtual void TakeDamage(int _damage)
    {
        if (isDie) { return; }

        hpValue.cur += _damage;

        // �_���[�WUI����
        damageText.text = $"{Mathf.Abs(_damage)}"; // �_���[�W���e�L�X�g�ɔ��f
        Vector3 popTextPos = new Vector3(enemyPos.position.x, 2.5f, enemyPos.position.z); // UI�̏o���ʒu����
        Instantiate(damageTextObj, popTextPos, Quaternion.identity); // ����

        if (hpValue.cur <= hpValue.min)
        {
            Die();
        }
    }

    /// <summary>
    /// �p���B���ꂽ����
    /// </summary>
    public virtual void TakeParry()
    {
        if(isDie) { return; }
    }

    /// <summary>
    /// HP��0�ɂȂ�����Ă΂��
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



    #region �A�j���[�V����Event

    /// <summary>
    /// �p���B�G�t�F�N�g���o��
    /// </summary>
    private void PlayParryEfect()
    {
        parryEfect.Play();
    }

    /// <summary>
    /// �A�j���[�V�����̏I��
    /// </summary>
    /// <param name="_animName">�X�e�[�g��</param>
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
