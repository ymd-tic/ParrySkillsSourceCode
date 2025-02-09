using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Generic: MonoBehaviour
{
    #region �ėp�N���X

    /// <summary>
    /// ���݁E�ŏ��E�ő�̃p�����[�^
    /// </summary>
    public class ParamateValue
    {
        public float cur; // ���ݒl
        public float max; // �ő�l
        public float min; // �ŏ��l

        /// <summary>
        /// �����ݒ�p
        /// </summary>
        /// <param name="curValue">�����l</param>
        /// <param name="maxValue">�ő�l</param>
        /// <param name="minValue">�ŏ��l</param>
        public ParamateValue(float curValue, float maxValue, float minValue)
        {
            cur = curValue;
            max = maxValue;
            min = minValue;
        }
    }

    /// <summary>
    /// �l�̑���
    /// �X���C�_�[�̃O���f�[�V����
    /// </summary>
    public class CalcuRation
    {
        /// <summary>
        /// �l�ƃX���C�_�[�����X�ɑ���
        /// </summary>
        /// <param name="_value">�����l</param>
        /// <returns></returns>
        public IEnumerator ValueFluctuation(float _value, Slider _slider, ParamateValue _paramateValue)
        {
            float curTime = 0; // �o�ߎ���
            float completeTime = 0.07f; // �X���C�_�[�̑����ɂ����鎞��
            float startValue = _paramateValue.cur; // �J�n�l
            float endValue = Mathf.Clamp(_paramateValue.cur + _value, _paramateValue.min, _paramateValue.max); // �ŏI�l

            while (curTime < completeTime)
            {
                curTime += Time.deltaTime;
                _paramateValue.cur = Mathf.Lerp(startValue, endValue, curTime / completeTime);
                _slider.value = _paramateValue.cur / _paramateValue.max;
                yield return null;
            }
            _paramateValue.cur = endValue;
            _slider.value = _paramateValue.cur / _paramateValue.max;
        }
    }

    /// <summary>
    /// ���l�Ɍ덷��ǉ����ĕԂ�
    /// </summary>
    /// <param name="_value">�����l</param>
    /// <param name="_error">�덷</param>
    /// <returns>�����l�}�덷 </returns>
    public static float RandomErrorRange(float _value , float _error)
    {
        return (int)Random.Range(_value - _error, _value + _error);
    }
    #endregion
}

// �R���[�`���̃^�X�N
// �l�b�g����E���Ă����R�[�h�ł�
public static class CoroutineExtensions
{
    public static Task AsTask(this IEnumerator coroutine, MonoBehaviour monoBehaviour)
    {
        var tcs = new TaskCompletionSource<bool>();
        monoBehaviour.StartCoroutine(RunCoroutine(coroutine, tcs));
        return tcs.Task;
    }

    private static IEnumerator RunCoroutine(IEnumerator coroutine, TaskCompletionSource<bool> tcs)
    {
        yield return coroutine;
        tcs.SetResult(true);
    }
}



