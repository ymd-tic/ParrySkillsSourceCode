using System.Collections;
using UnityEngine;

public class EnemySkeletonCtrl : EnemyBase
{
    private enum AIState // ��ԃp�^�[��
    {
        Idle,       // �ҋ@
        Wait,       // �҂�
        Patrol,     // ����
        Chase,      // �ǐ�
        Atack,      // �U��
        Damage,     // �_���[�W
        KnockBack,  // �m�b�N�o�b�N
        Distance    // �������Ƃ�
    }

    private enum AtackState // �U���p�^�[��
    {
        Melee1,     // �ߐ�1
    }
    //-----SerializeField------------------------------------------------------------
    [Header("�N�[���^�C��")]
    [SerializeField] private CoolTime atackTime;    // �U���N�[���^�C��



    //-----privateField--------------------------------------------------------------
    private EnemyAreaBase enemyArea;    // �X�|�[�������G���A
    private Coroutine coroutine;    // �R���[�`��
    private AIState aiState = AIState.Patrol;
    private AtackState atackState = AtackState.Melee1;



    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------




    #region �V�X�e��

    protected override void Start()
    {
        base.Start();

        // �ړI�n���G���A���ɐݒ�
        enemyArea = this.transform.parent.GetComponent<EnemyAreaBase>();
        agent.destination = enemyArea.GetRandomPosInSphere();
    }

    protected override void Update()
    {
        base.Update();

        // ���񂾂牽�����Ȃ�
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


    #region ��ԃX�e�[�g

    private void Idle()
    {
        // �v���C���[�Ƃ̋������ǐՔ͈͊O�Ȃ�
        if (DistanceFromPlayer() > range.far)
        {
            ChangeAIState(AIState.Patrol);
        }
        // �v���C���[�Ƃ̋������U���͈͊O�Ȃ�
        else if (DistanceFromPlayer() > range.atack)
        {
            ChangeAIState(AIState.Chase);
        }
        else if(DistanceFromPlayer() > range.near)
        {
            // �U���N�[���^�C�����I�������U���X�e�[�g�Ɉڂ�
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
            // �ҋ@���Ԃ��I������玟�̒n�_�����߂�
            coroutine = StartCoroutine(SetNextPatrolPoint());
        }

        // �v���C���[���߂��ɗ�����ҋ@������
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
        // �ڕW�n�_�ɓ���������
        if (agent.remainingDistance < 2f)
        {
            ChangeAIState(AIState.Wait);
        }

        // �v���C���[�Ƃ̋������ǐՔ͈͓��Ȃ�
        if (DistanceFromPlayer() <= range.far)
        {
            ChangeAIState(AIState.Chase);
            return;
        }
    }

    private void Chase()
    {
        agent.destination = playerPos.position;

        // �v���C���[�Ƃ̋������ǐՔ͈͊O�Ȃ�
        if (DistanceFromPlayer() > range.far)
        {
            ChangeAIState(AIState.Patrol);
        }
        // �v���C���[�Ƃ̋������U���͈͊O�Ȃ�
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
        // �U������
        switch(atackState)
        {
            case AtackState.Melee1:
                Melee1();
                break;
        }

        // �U�����[�V�����̏I��
        if (AnimationEnd("Atack"))
        {
            // �v���C���[�Ƃ̋�����
            // �ǐՔ͈͊O
            if (DistanceFromPlayer() > range.far)
            {
                ChangeAIState(AIState.Patrol);
            }
            // �U���͈͊O
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

        #region �U���p�^�[��

        void Melee1()
        {

        }

        #endregion

    }

private void Damage()
    {
        // �U���N�[���^�C�����I�������U���X�e�[�g�Ɉڂ�
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

            // �v���C���[�Ƃ̋�����
            // �ǐՔ͈͊O
            if (DistanceFromPlayer() > range.far)
            {
                ChangeAIState(AIState.Patrol);
            }
            // �U���͈͊O
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

            // �v���C���[�Ƃ̋������ǐՔ͈͓��Ȃ�
            if (DistanceFromPlayer() <= range.far)

            {   // ���U���͈͓��Ȃ�
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
        // �v���C���[���痣�������Ɉړ�
        Vector3 directionAwayFromPlayer = (transform.position - playerPos.position).normalized;
        Vector3 retreatPosition = transform.position + directionAwayFromPlayer * range.near;

        agent.destination = retreatPosition;

        // �U���N�[���^�C�����I�������U���X�e�[�g�Ɉڂ�
        atackTime.cur += Time.deltaTime;
        if (atackTime.cur > atackTime.goal)
        {
            ChangeAIState(AIState.Atack);
            return;
        }

        // ���̋������������Idle��ԂɑJ��
        if (DistanceFromPlayer() > range.near)
        {
            ChangeAIState(AIState.Idle);
        }
    }
    #endregion


    #region �G�l�~�[�̐���

    /// <summary>
    /// �_���[�W���󂯂�
    /// </summary>
    /// <param name="_damage">�_���[�W</param>
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
    /// ��ԃX�e�[�g��ς���
    /// �X�e�[�g���̃A�j���[�V�������Đ�
    /// </summary>
    /// <param name="_nextState">���̃X�e�[�g</param>
    private void ChangeAIState(AIState _nextState)
    {
        if(isDie) {return;}

        aiState = _nextState;   // �X�e�[�g�X�V

        foreach (var animState in animator.parameters)
        {
            // �g���K�[�ȊO�̓X�L�b�v
            if (animState.type != AnimatorControllerParameterType.Trigger) { continue; }

            if (animState.name == $"{_nextState}")
            {
                animator.SetTrigger($"{_nextState}");   // �X�V
            }
            else
            {
                animator.ResetTrigger($"{animState.name}"); // �������Z�b�g
            }
        }

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

            case AIState.Chase:
                agent.speed = speed.fast;
                agent.destination = playerPos.position;
                break;

            case AIState.Atack:
                // �U���������_���őI��
                AtackState atack = EnumGeneric.GetRandom<AtackState>();
                SetAtackState(atack);

                // �U���N�[���^�C���������_���Őݒ�
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

        Debug.Log($"{_nextState}�X�e�[�g�ɍX�V");
    }

    /// <summary>
    /// �U���X�e�[�g��ݒ�
    /// </summary>
    /// <param name="_atack">�U��</param>
    private void SetAtackState(AtackState _atack)
    {
        atackState = _atack;

        animator.SetInteger("AtackValue", (int)_atack + 1);
        // +1�Ƃ��Ă���̂�Animator�̊e�J�ڏ�����1����n�܂邽��

        // �U���X�e�[�g�Ɉڂ鎞��1�񂾂��Ă΂�鏈��
        switch (_atack)
        {
            case AtackState.Melee1:
                atackPower = Generic.RandomErrorRange(-10.0f, 2.0f);
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
        float waitTime = 5.5f; // �ҋ@����
        yield return new WaitForSeconds(waitTime);
        ChangeAIState(AIState.Patrol);
    }

    #endregion
}