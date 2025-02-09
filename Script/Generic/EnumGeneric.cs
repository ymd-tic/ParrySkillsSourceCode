using System;
using System.Collections.Generic;

/// <summary>
/// enum�֗̕��N���X �l�b�g����E���Ă����R�[�h�ł�
/// </summary>
public static class EnumGeneric
{

    //=================================================================================
    //�擾
    //=================================================================================

    /// <summary>
    /// ���ڐ����擾
    /// </summary>
    public static int GetTypeNum<T>() where T : struct
    {
        return System.Enum.GetValues(typeof(T)).Length;
    }

    /// <summary>
    /// ���ڂ������_���Ɉ�擾
    /// </summary>
    public static T GetRandom<T>() where T : struct
    {
        int no = UnityEngine.Random.Range(0, GetTypeNum<T>());
        //int no = new System.Random().Next(0, GetTypeNum<T>()); //UnityEngine���g��Ȃ��ꍇ
        return NoToType<T>(no);
    }

    /// <summary>
    /// �S�Ă̍��ڂ�������List���擾
    /// </summary>
    public static List<T> GetAllInList<T>() where T : struct
    {
        var list = new List<T>();
        foreach (T t in System.Enum.GetValues(typeof(T)))
        {
            list.Add(t);
        }
        return list;
    }

    //=================================================================================
    //�ϊ�
    //=================================================================================

    /// <summary>
    /// ���͂��ꂽ������Ɠ������ڂ��擾
    /// </summary>
    public static T KeyToType<T>(string targetKey) where T : struct
    {
        return (T)System.Enum.Parse(typeof(T), targetKey);
    }

    /// <summary>
    /// ���͂��ꂽ�ԍ��̍��ڂ��擾
    /// </summary>
    public static T NoToType<T>(int targetNo) where T : struct
    {
        return (T)System.Enum.ToObject(typeof(T), targetNo);
    }

    //=================================================================================
    //����
    //=================================================================================

    /// <summary>
    /// ���͂��ꂽ������̍��ڂ��܂܂�Ă��邩
    /// </summary>
    public static bool ContainsKey<T>(string tagetKey) where T : struct
    {
        foreach (T t in System.Enum.GetValues(typeof(T)))
        {
            if (t.ToString() == tagetKey)
            {
                return true;
            }
        }

        return false;
    }

    //=================================================================================
    //���s
    //=================================================================================

    /// <summary>
    /// �S�Ă̍��ڂɑ΂��ăf���Q�[�g�����s
    /// </summary>
    public static void ExcuteActionInAllValue<T>(Action<T> action) where T : struct
    {
        foreach (T t in System.Enum.GetValues(typeof(T)))
        {
            action(t);
        }
    }

}