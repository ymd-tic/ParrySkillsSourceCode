using UnityEngine;

/// <summary>
/// �l�b�g����E���Ă����R�[�h�ł�
/// </summary>
public class CloudMovement : MonoBehaviour
{
    public Material cloudMaterial;

    void Update()
    {
        // Time.unscaledTime���V�F�[�_�[�ɓn��
        cloudMaterial.SetFloat("_UnscaledTime", Time.unscaledTime);
    }
}