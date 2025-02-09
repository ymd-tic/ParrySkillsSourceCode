using System.Collections.Generic;
using UnityEngine;

public class EnemyAreaBase : MonoBehaviour
{
    [System.Serializable]
    public class SpownEnemy // スポーンする敵の情報
    {
        public GameObject enemyObj; // エネミー
        public int spownValue;   // スポーン数
    }

    //-----SerializeField------------------------------------------------------------
    [Header("前後のバリア")]
    [SerializeField] protected GameObject[] barrier = new GameObject[2];

    [Header("エネミー")]
    [SerializeField] protected List<SpownEnemy> nomalEnemys = new List<SpownEnemy>();


    //-----privateField--------------------------------------------------------------


    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------
    protected SphereCollider sphereCollider;
    protected bool inArea = false; // エリアに入ったか判定
    protected bool isAreaClear = false; // エリアのクリア判定

    #region システム
    protected virtual void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }


    protected virtual void Update()
    {
        // エリアに入っている場合
        if (inArea)
        {
            // エリア内全ての敵が倒されたら
            if(AreaManager.enemyList.Count <= 0)
            {
                isAreaClear = true;
            }
        }
        else { return; }

        // エリアをクリアしたら
        if(isAreaClear)
        {
            // エリアを解放
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

        // 雑魚敵をスポーン
        foreach (var _enemy in nomalEnemys)
        {
            for (int i = 0; i < _enemy.spownValue; i++)
            {
                AreaManager.enemyList.Add(Instantiate(_enemy.enemyObj, GetRandomPosInSphere(), Quaternion.identity, transform));
            }
        }

        // エリアを封鎖
        foreach (var _barrier in barrier)
        {
            _barrier.gameObject.SetActive(true);
        }
        inArea = true;
    }
    #endregion


    #region 機能

    /// <summary>
    /// 円内のランダムな座標を返す
    /// </summary>
    /// <returns>座標</returns>
    public Vector3 GetRandomPosInSphere()
    {
        // SphereColliderの中心と直径を求める
        Vector3 center = sphereCollider.transform.position + sphereCollider.center;
        float radius = sphereCollider.radius * sphereCollider.transform.localScale.x;

        // 向きと中心から
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere;
        float radomDistance = UnityEngine.Random.Range(5, radius);

        Vector3 randomPosition = center + randomDirection * radomDistance;
        randomPosition.y = 0;
        return randomPosition;
    }
    #endregion
}
