using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    //-----SerializeField------------------------------------------------------------



    //-----privateField--------------------------------------------------------------


    //-----publicField---------------------------------------------------------------


    //-----staticField---------------------------------------------------------------
    // �X�|�[�������G�̊i�[�p���X�g
    static public List<GameObject> enemyList = new();


    //-----ComponentField------------------------------------------------------------

    #region �G���A�Ǘ�

    /// <summary>
    /// ��ԋ߂��G�l�~�[�𑖍�
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    static public Vector3 NearEnemy(GameObject player)
    {
        Vector3 nearEnemy = enemyList[0].transform.position;

        foreach (GameObject obj in enemyList)
        {
            if (Vector3.Distance(nearEnemy, player.transform.position) >
                Vector3.Distance(obj.transform.position, player.transform.position))
            {
                nearEnemy = obj.transform.position;
            }
        }

        Vector3 targetPos = new(nearEnemy.x, 0, nearEnemy.z);

        return targetPos;
    }

    #endregion

}