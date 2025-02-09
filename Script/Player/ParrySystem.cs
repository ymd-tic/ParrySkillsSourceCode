using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParrySystem : MonoBehaviour
{

    //-----SerializeField------------------------------------------------------------
    [Header("�p���B�����G�t�F�N�g")]
    [SerializeField] ParticleSystem[] parryEfects;

    [Header("�X�N���v�g")]
    [SerializeField] private PlayerAudioCtrl audioCtrl;

    //-----privateField--------------------------------------------------------------

    private Dictionary<int, bool> atackCollider { get; } = new Dictionary<int, bool>();


    //-----publicField---------------------------------------------------------------
    public static bool parrySuccess = false;    // �p���B�����������t���O


    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    #region �V�X�e��


    private void OnTriggerEnter(Collider other)
    {
        // �q�b�g�����^�O��EnemyAtack�����m
        if (!other.CompareTag("EnemyAtack")) { return; }
        if (parrySuccess) {return; }

        SuccessParry(other);
    }

    #endregion


    #region �p���B����

    /// <summary>
    /// �p���B�̐���
    /// </summary>
    /// <param name="_other">�q�b�g���������蔻��</param>
    private void SuccessParry(Collider _other)
    {
        parrySuccess = true;

        // �������x�����ɂ���
        EnemyBase enemy = _other.GetComponent<EnemyAtack>().enemy;
        int enemyId = enemy.GetInstanceID();
        if (atackCollider.ContainsKey(enemyId)) { return; }
        atackCollider[enemyId] = true;

        // �A�h���i�����Q�[�W�𑝂₷
        this.transform.parent.GetComponent<SkillCtrl>().AdrenalineGaugeCalculation(10f);

        // �G���m�b�N�o�b�N������
        enemy.TakeParry();

        // �G�t�F�N�g����
        Vector3 efectPos = this.transform.position;
        efectPos.y = 1.5f;
        foreach (var efect in parryEfects)
        {
            Instantiate(efect, efectPos, Quaternion.identity);
        }


        Time.timeScale = 0.5f;

        StartCoroutine(ReseetParryFlag());
    }

    #endregion

    /// <summary>
    /// �p���B�t���O�̃��Z�b�g
    /// </summary>
    /// <returns></returns>
    IEnumerator ReseetParryFlag()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        parrySuccess = false;
        Time.timeScale = 1.0f;
        yield return new WaitForSecondsRealtime(0.8f);
        atackCollider.Clear();
    }
}
