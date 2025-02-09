using UnityEngine;
using System.Collections;
public class EnemyHitManager : MonoBehaviour
{

    //-----SerializeField------------------------------------------------------------



    //-----privateField--------------------------------------------------------------
    private EnemyBase enemy;
    private new Collider collider = null;
    private bool canDamage = true; // �_���[�W���󂯂邩����


    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    #region �V�X�e��

    void Start()
    {
        enemy = GetComponent<EnemyBase>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerAtack") && canDamage)
        {
            collider = other;
            StartCoroutine(ExeDamage());
        }
    }

    #endregion


    #region �R���[�`��

    /// <summary>
    /// �p���B�E�q�b�g����̎��s���p
    /// </summary>
    IEnumerator ExeDamage()
    {
        yield return new WaitForSecondsRealtime(0.04f);

        if (!ParrySystem.parrySuccess)
        {
            PlayerCtrl playerCtrl = collider.transform.root.GetComponent<PlayerCtrl>();

            collider.transform.root.GetComponent<SkillCtrl>().AdrenalineGaugeCalculation(1.0f);
            enemy.TakeDamage(-playerCtrl.atackPower);
            StartCoroutine(CanDamage());
        }
    }

    /// <summary>
    /// ��莞�Ԗ��G
    /// </summary>
    /// <returns></returns>
    IEnumerator CanDamage()
    {
        float canDamageTime = 0.2f; // ���G����

        canDamage = false;
        yield return new WaitForSeconds(canDamageTime);
        canDamage = true;
    }

    #endregion
}
