using TMPro;
using UnityEngine;

public class DamageUI : MonoBehaviour
{

    //-----SerializeField------------------------------------------------------------
    [Header("�t�F�[�h���鎞��")][SerializeField] private float feadSpeed = 10.0f;


    //-----privateField--------------------------------------------------------------
    private TextMeshProUGUI damageText;
    private float startAlpha;
    private float curTime = 0;

    //-----publicField---------------------------------------------------------------



    //-----staticField---------------------------------------------------------------



    //-----protectedField------------------------------------------------------------

    #region �V�X�e��

    void Start()
    {
        damageText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        startAlpha = damageText.color.a;
    }


    void Update()
    {
        // UI���J�����Ɍ�������
        transform.rotation = Camera.main.transform.rotation;

        // ���X�ɔ������ɂ��ď���
        curTime += Time.deltaTime;
        float colorAlpha = Mathf.Lerp(startAlpha, 0, curTime / feadSpeed);
        Color newColor = damageText.color;
        newColor.a = colorAlpha;
        damageText.color = newColor;

        // �����x0
        if (colorAlpha <= 0)
        {
            Destroy(gameObject);
        }
    }

    #endregion

}
