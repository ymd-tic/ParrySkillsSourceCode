using UnityEngine;
using System.Collections.Generic;

public class EnemyAtackColliderCtrl : MonoBehaviour
{
    [System.Serializable]
    public class ColliderList // �R���C�_�[�̃��X�g
    {
        [Header("�R���C�_�[")]
        public List<Collider> colliders = new List<Collider>();

    }

    //-----SerializeField------------------------------------------------------------


    [Header("�e�U���̔���")]
    [SerializeField] new List<ColliderList> collider = new List<ColliderList>();


    //-----privateField--------------------------------------------------------------
    Animator animator;



    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    #region �V�X�e��

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    #endregion


    #region �A�j���[�V����Event

    /// <summary>
    /// �R���C�_�[��L���ɂ���
    /// </summary>
    public void SetColliderOn()
    {
        // ���݂̍U�����擾
        int atackValue = animator.GetInteger("AtackValue");

        // �R���C�_�[��L���ɂ���
        foreach (var col in collider[atackValue-1].colliders)
        {
            col.enabled = true;
        }
    }

    /// <summary>
    /// �R���C�_�[�𖳌��ɂ���
    /// </summary>
    public void SetColliderOff()
    {
        // �R���C�_�[�𖳌��ɂ���
        foreach (var col in collider)
        {
            foreach (var col2 in col.colliders)
            {
                col2.enabled = false;
            }
        }
    }
    #endregion

}
