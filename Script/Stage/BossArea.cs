using System.Collections.Generic;
using UnityEngine;

public class BossArea : EnemyAreaBase
{
    [Header("エネミー")]
    [SerializeField] private List<SpownEnemy> bossEnemys = new List<SpownEnemy>();
    [SerializeField] private GameObject BossSpownObj;

    private Vector3 BossSpownPos;

    protected override void Start()
    {
        base.Start();
        BossSpownPos = BossSpownObj.transform.position;
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

        // ボスをスポーン
        foreach (var _enemy in bossEnemys)
        {
            for (int i = 0; i < _enemy.spownValue; i++)
            {
                AreaManager.enemyList.Add(Instantiate(_enemy.enemyObj, BossSpownPos, Quaternion.identity, transform));
            }
        }

        // エリアを封鎖
        foreach (var _barrier in barrier)
        {
            _barrier.gameObject.SetActive(true);
        }
        inArea = true;
    }
}
