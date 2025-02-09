using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class EnemyWizardCtrl : EnemyBase
{
    private enum AIState // ��ԃp�^�[��
    {
        Idle,       // �ҋ@
        Wait,       // �҂�
        Patrol,     // ����
        Atack,      // �U��
        Damage,     // �_���[�W
        Distance,   // �������Ƃ�
        Warp        // ���[�v
    }

    private enum AtackState // �U���p�^�[��
    {
        Magic1,     // ���@1
        Magic2,     // ���@2
        Magic3,     // ���@3
        Magic4      // ���@4
    }

    //-----SerializeField------------------------------------------------------------
    [Header("�N�[���^�C��")]
    [SerializeField] private CoolTime atackTime;    // �U���N�[���^�C��
    [SerializeField] private CoolTime distanceTime; // �������Ƃ�N�[���^�C��
    [SerializeField] private CoolTime warpTime;     // ���[�v�N�[���^�C��
    [Header("�Q�[�W")]
    [SerializeField] private Slider hpGage; // HP�Q�[�W
    [Header("�������U��")]
    [SerializeField] private GameObject fireBall;   // �t�@�C�A�{�[��

    [Header("�G�t�F�N�g")]
    [SerializeField] private GameObject warpEffect; // ���[�v�G�t�F�N�g
    [SerializeField] private GameObject warpShadowEffect; // �e�G�t�F�N�g

    //-----privateField--------------------------------------------------------------
    private EnemyAreaBase enemyArea;    // �X�|�[�������G���A
    private Coroutine coroutine;    // �R���[�`��
    private AIState aiState = AIState.Patrol;
    private AtackState atackState = AtackState.Magic1;
    private bool isWarp = false;   // ���[�v�����ǂ���
    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    # region �V�X�e��
    protected override void Start()
    {
        base.Start();

        hpGage.value = hpValue.cur / hpValue.max;

        // �ړI�n���G���A���ɐݒ�
        enemyArea = this.transform.parent.GetComponent<EnemyAreaBase>();
        agent.destination = enemyArea.GetRandomPosInSphere();

        // �N�[���^�C���̏����l�ݒ�
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

        // ���񂾂牽�����Ȃ�
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

    #region ��ԃX�e�[�g
    private void Idle()
    {
        transform.LookAt(playerPos);

        if(isWarp) { return; }

        // �U���N�[���^�C�����I�������U���X�e�[�g�Ɉڂ�
        atackTime.cur += Time.deltaTime;
        if(atackTime.cur > atackTime.goal)
        {
            ChangeAIState(AIState.Atack);
            atackTime.cur = 0;
            return;
        }

        // �������Ƃ�N�[���^�C�����I������痣���X�e�[�g�Ɉڂ�
        distanceTime.cur += Time.deltaTime;
        if (distanceTime.cur > distanceTime.goal)
        {
            ChangeAIState(AIState.Distance);
            distanceTime.cur = 0;
        }
    }

    private void Wait()
    {
        // �����Ă���R���[�`��������ꍇ�̓X�L�b�v
        if(coroutine == null)
        {
            // �ҋ@���Ԃ��I������玟�̒n�_�����߂�
            coroutine = StartCoroutine(SetNextPatrolPoint());
        }

        // �v���C���[���߂��ɗ�����ҋ@������
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
        // �ڕW�n�_�ɒ������玟�̒n�_�����߂�
        if(agent.remainingDistance < 2f)
        {
            ChangeAIState(AIState.Wait);
        }

        // �v���C���[���߂��ɗ�����ҋ@������
        if (DistanceFromPlayer() <= range.far)
        {
            ChangeAIState(AIState.Idle);
        }
    }

    private void Atack()
    {
        // �U�����[�V�����̏I��
        if(AnimationEnd("Atack"))
        {
            //Debug.Log("�U���I��");

            ChangeAIState(AIState.Idle);
            canDamageAnim = true;
        }
        #region �U���X�e�[�g


        #endregion
    }

    private void Damage()
    {
        // ���[�v�N�[���^�C�����I�������ҋ@�X�e�[�g�Ɉڂ�
        warpTime.cur += Time.deltaTime;
        if (warpTime.cur > warpTime.goal)
        {
            ChangeAIState(AIState.Warp);
            warpTime.cur = 0;
            rigidbody.isKinematic = true;
            canDamageAnim = true;
            return;
        }

        // �U���N�[���^�C�����I�������U���X�e�[�g�Ɉڂ�
        atackTime.cur += Time.deltaTime;
        if (atackTime.cur > atackTime.goal)
        {
            ChangeAIState(AIState.Atack);
            atackTime.cur = 0;
            rigidbody.isKinematic = true;
            return;
        }

        // �A�j���[�V�������I��������
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

        // �v���C���[���߂��ɗ�����
        if (DistanceFromPlayer() <= range.atack)
        {
            // �v���C���[���痣�������Ɉړ�
            Vector3 directionAwayFromPlayer = (transform.position - playerPos.position).normalized;
            Vector3 retreatPosition = transform.position + directionAwayFromPlayer * range.atack;
            agent.destination = retreatPosition;
        }

        // �v���C���[�������ɍs������
        if (DistanceFromPlayer() >= range.far)
        {
            // �v���C���[�ɋ߂Â������Ɉړ�
            agent.destination = playerPos.position;
        }

        // �U���N�[���^�C�����I�������U���X�e�[�g�Ɉڂ�
        atackTime.cur += Time.deltaTime;
        if (atackTime.cur > atackTime.goal)
        {
            ChangeAIState(AIState.Atack);
            atackTime.cur = 0;
            return;
        }

        // ���[�v�N�[���^�C�����I�������ҋ@�X�e�[�g�Ɉڂ�
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

    #region �G�l�~�[�̐���

    /// <summary>
    /// �_���[�W���󂯂�
    /// </summary>
    /// <param name="_damage">�_���[�W</param>
    public override async void TakeDamage(int _damage)
    {
        if (isDie) { return; }

        // �󂯂��_���[�W�𔽉f
        damageText.text = $"{Mathf.Abs(_damage)}";
        // UI�̃|�b�v�A�b�v�ʒu
        Vector3 popTextPos = new Vector3(enemyPos.position.x, 2.5f, enemyPos.position.z);
        // �_���[�WUI����
        Instantiate(damageTextObj, popTextPos, Quaternion.identity);

        // HP�����炷
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
    /// ��ԃX�e�[�g��ݒ�
    /// �X�e�[�g���̃A�j���[�V�������Đ�
    /// </summary>
    /// <param name="_nextState">���̃X�e�[�g</param>
    private void ChangeAIState(AIState _nextState)
    {
        if (isDie) { return; }

        aiState = _nextState;   // �X�e�[�g�X�V


        // �A�j���[�V�����X�V
        foreach (var animState in animator.parameters)
        {
            // �g���K�[�ȊO�̓X�L�b�v
            if (animState.type != AnimatorControllerParameterType.Trigger) { continue; }

            if(animState.name == $"{_nextState}")
            {
                animator.SetTrigger($"{_nextState}");   // �X�V
            }
            else�@      
            {
                animator.ResetTrigger($"{animState.name}"); // �������Z�b�g
            }
        }

        // �N�[���^�C���������_���ɍĐݒ�
        distanceTime.goal = Generic.RandomErrorRange(distanceTime.def, 2f); // �����𑪂�܂ł̎���
        atackTime.goal = Generic.RandomErrorRange(atackTime.def, 1f);       // �U���܂ł̎���
        warpTime.goal = Generic.RandomErrorRange(warpTime.def, 2f);         // ���[�v�܂ł̎���

        // ���̃X�e�[�g�Ɉڂ鎞��1�񂾂��Ă΂�鏈��
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
                // �v���C���[�Ƃ̋����ɂ���čU���p�^�[����ς���
                AtackState atack;
                if(DistanceFromPlayer() <= range.atack)
                {
                    atack = (AtackState)Random.Range(0, 1); // �ߐڍU��
                }
                else
                {
                    atack = (AtackState)Random.Range(2, 4); // �������U��
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

        //Debug.Log($"{_nextState}�X�e�[�g�ɍX�V");
    }

    /// <summary>
    /// �U���X�e�[�g��ݒ�
    /// </summary>
    /// <param name="_atack">�U��</param>
    private void SetAtackState(AtackState _atack)
    {
        atackState = _atack;

        animator.SetInteger("AtackValue", (int)_atack + 1);
        // +1�Ƃ��Ă���̂�Animator�̑J�ڏ�����1����n�܂邽��

        // �U���X�e�[�g�Ɉڂ鎞��1�񂾂��Ă΂�鏈��
        switch (_atack)
        {
            case AtackState.Magic1:
                atackPower = Generic.RandomErrorRange(-10.0f, 2.0f);
                //Debug.Log("���@1");
                break;

            case AtackState.Magic2:
                atackPower = Generic.RandomErrorRange(-15.0f, 2.0f);
                //Debug.Log("���@2");
                break;

            case AtackState.Magic3:
                atackPower = Generic.RandomErrorRange(-20.0f, 3.0f);
                //Debug.Log("���@3");
                break;

            case AtackState.Magic4:
                atackPower = Generic.RandomErrorRange(-15.0f, 4.0f);
                //Debug.Log("���@4");
                break;
        }
    }

    #endregion

    #region �R���[�`��

    /// <summary>
    /// ����n�_�ɒ��������莞�ԑҋ@���Ă��玟�̒n�_�����߂�
    /// </summary>
    /// <returns></returns>
    IEnumerator SetNextPatrolPoint()
    {
        float waitTime = 2.5f; // �ҋ@����
        yield return new WaitForSeconds(waitTime);
        coroutine = null;
        ChangeAIState(AIState.Patrol);
    }

    /// <summary>
    /// ���[�v�ړ�
    /// </summary>
    /// <returns></returns>
    IEnumerator WarpCoroutine()
    {
        float waitTime = 1.5f; // �ҋ@����
        isWarp = true;

        // ������G�t�F�N�g����
        Vector3 efectPos = this.transform.position;
        Instantiate(warpEffect, efectPos, Quaternion.identity);

        // ���[�v
        Vector3 warpPos = enemyArea.GetRandomPosInSphere(); // �͈͓��̃����_���ȍ��W���擾
        enemyPos.position = warpPos;    // ���[�v�n�_�ɍ��W�ړ�

        // �e�G�t�F�N�g�̐����ƈړ�
        var shadow = Instantiate(warpShadowEffect, efectPos, Quaternion.identity);
        warpPos.y = 1f;   // ���[�v�n�_�̍������グ��
        shadow.transform.position = new(shadow.transform.position.x,warpPos.y,shadow.transform.position.z);
        shadow.transform.DOMove(warpPos, waitTime)    // ���[�v�n�_�܂Ŏc�����ړ�
                                .SetLink(gameObject);

        // �p���\��
        this.transform.GetChild(0).gameObject.SetActive(false);

        yield return new WaitForSeconds(waitTime);

        isWarp = false;
        warpTime.cur = 0;

        // �e�G�t�F�N�g�폜
        Destroy(shadow);

        // �G�t�F�N�g����
        efectPos = this.transform.position;
        Instantiate(warpEffect, efectPos, Quaternion.identity);

        // �p��\��
        this.transform.GetChild(0).gameObject.SetActive(true);

        ChangeAIState(AIState.Idle);
    }

    #endregion

    #region �A�j���[�V����Event

    /// <summary>
    /// ���������@�U��
    /// </summary>
    public void FireBall()
    {
        GameObject obj; // �t�@�C�A�{�[��
        float shotSpeed = 1000; // ���ˑ��x

        // �������W
        Vector3 pos = this.transform.position;
        Quaternion rot = this.transform.rotation;
        pos.y += 1f;

        // �t�@�C�A�{�[���𐶐�
        obj = Instantiate(fireBall, pos, rot);
        // ���ʕ����ɔ�΂�
        obj.GetComponent<Rigidbody>().AddForce(transform.forward * shotSpeed);
        // �_���[�W��ݒ�
        obj.GetComponent<EnemyAtack>().enemy = this;
    }
    #endregion
}

