using UnityEngine;

public class PlayerAtackColliderCtrl : MonoBehaviour
{

    //-----SerializeField------------------------------------------------------------
    [Header("コライダー")][SerializeField] BoxCollider atackCollider;    // 攻撃判定


    //-----privateField--------------------------------------------------------------



    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    #region コライダー制御

    public void ColliderOn()
    {
        atackCollider.enabled = true;
    }

    public void ColliderOff()
    {
        atackCollider.enabled = false;

    }

    #endregion
}
