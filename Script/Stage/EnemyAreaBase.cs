using System.Collections.Generic;
using UnityEngine;

public class EnemyAreaBase : MonoBehaviour
{
    [System.Serializable]
    public class SpownEnemy // �X�|�[������G�̏��
    {
        public GameObject enemyObj; // �G�l�~�[
        public int spownValue;   // �X�|�[����
    }

    //-----SerializeField------------------------------------------------------------
    [Header("�O��̃o���A")]
    [SerializeField] protected GameObject[] barrier = new GameObject[2];

    [Header("�G�l�~�[")]
    [SerializeField] protected List<SpownEnemy> nomalEnemys = new List<SpownEnemy>();


    //-----privateField--------------------------------------------------------------


    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------
    protected SphereCollider sphereCollider;
    protected bool inArea = false; // �G���A�ɓ�����������
    protected bool isAreaClear = false; // �G���A�̃N���A����

    #region �V�X�e��
    protected virtual void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }


    protected virtual void Update()
    {
        // �G���A�ɓ����Ă���ꍇ
        if (inArea)
        {
            // �G���A���S�Ă̓G���|���ꂽ��
            if(AreaManager.enemyList.Count <= 0)
            {
                isAreaClear = true;
            }
        }
        else { return; }

        // �G���A���N���A������
        if(isAreaClear)
        {
            // �G���A�����
            foreach(var _barrier in barrier)
            {
                _barrier.gameObject.SetActive(false);
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (inArea) { return; }
        if (!other.gameObject.CompareTag("Player")) { return; }

        // �G���G���X�|�[��
        foreach (var _enemy in nomalEnemys)
        {
            for (int i = 0; i < _enemy.spownValue; i++)
            {
                AreaManager.enemyList.Add(Instantiate(_enemy.enemyObj, GetRandomPosInSphere(), Quaternion.identity, transform));
            }
        }

        // �G���A�𕕍�
        foreach (var _barrier in barrier)
        {
            _barrier.gameObject.SetActive(true);
        }
        inArea = true;
    }
    #endregion


    #region �@�\

    /// <summary>
    /// �~���̃����_���ȍ��W��Ԃ�
    /// </summary>
    /// <returns>���W</returns>
    public Vector3 GetRandomPosInSphere()
    {
        // SphereCollider�̒��S�ƒ��a�����߂�
        Vector3 center = sphereCollider.transform.position + sphereCollider.center;
        float radius = sphereCollider.radius * sphereCollider.transform.localScale.x;

        // �����ƒ��S����
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere;
        float radomDistance = UnityEngine.Random.Range(5, radius);

        Vector3 randomPosition = center + randomDirection * radomDistance;
        randomPosition.y = 0;
        return randomPosition;
    }
    #endregion
}
