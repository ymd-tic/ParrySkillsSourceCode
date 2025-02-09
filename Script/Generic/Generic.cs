using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Generic: MonoBehaviour
{
    #region 汎用クラス

    /// <summary>
    /// 現在・最小・最大のパラメータ
    /// </summary>
    public class ParamateValue
    {
        public float cur; // 現在値
        public float max; // 最大値
        public float min; // 最小値

        /// <summary>
        /// 初期設定用
        /// </summary>
        /// <param name="curValue">初期値</param>
        /// <param name="maxValue">最大値</param>
        /// <param name="minValue">最小値</param>
        public ParamateValue(float curValue, float maxValue, float minValue)
        {
            cur = curValue;
            max = maxValue;
            min = minValue;
        }
    }

    /// <summary>
    /// 値の増減
    /// スライダーのグラデーション
    /// </summary>
    public class CalcuRation
    {
        /// <summary>
        /// 値とスライダーを徐々に増減
        /// </summary>
        /// <param name="_value">増減値</param>
        /// <returns></returns>
        public IEnumerator ValueFluctuation(float _value, Slider _slider, ParamateValue _paramateValue)
        {
            float curTime = 0; // 経過時間
            float completeTime = 0.07f; // スライダーの増減にかかる時間
            float startValue = _paramateValue.cur; // 開始値
            float endValue = Mathf.Clamp(_paramateValue.cur + _value, _paramateValue.min, _paramateValue.max); // 最終値

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
    /// 数値に誤差を追加して返す
    /// </summary>
    /// <param name="_value">初期値</param>
    /// <param name="_error">誤差</param>
    /// <returns>初期値±誤差 </returns>
    public static float RandomErrorRange(float _value , float _error)
    {
        return (int)Random.Range(_value - _error, _value + _error);
    }
    #endregion
}

// コルーチンのタスク
// ネットから拾ってきたコードです
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



