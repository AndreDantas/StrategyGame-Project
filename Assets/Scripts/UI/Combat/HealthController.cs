using UnityEngine;
using System.Collections;
using TMPro;
[System.Serializable]
public class HealthController : MonoBehaviour
{

    [SerializeField]
    Bar bar;
    public TextMeshProUGUI damageText;
    public static float animTime = 0.4f;
    public static float countdownWaitTime = 0.02f;
    private void Start()
    {
        if (damageText)
            damageText.enabled = false;
        if (bar != null)
        {
            bar.SetColor(bar.barColor);
        }
    }

    private void OnValidate()
    {
        if (bar != null)
        {
            bar.SetColor(bar.barColor);
            bar.UpdateBar();
        }
    }
    public void SetBar(Bar bar)
    {
        this.bar = bar;

    }
    public Bar GetBar()
    {
        return bar;

    }

    public IEnumerator DamageAnim(int damage)
    {
        if (damageText == null || bar == null)
            yield break;
        yield return null;

        damageText.text = "-" + damage.ToString();
        damageText.enabled = true;

        yield return new WaitForSeconds(animTime);
        while (damage > 0)
        {
            damage -= 1;
            CurrentValue -= 1;
            damageText.text = "-" + damage.ToString();
            yield return new WaitForSeconds(countdownWaitTime);
        }
        yield return new WaitForSeconds(animTime / 2);
        damageText.enabled = false;
    }

    [SerializeField]
    float _maxValue;
    public float MaxValue
    {

        get
        {
            return _maxValue;
        }

        set
        {
            this._maxValue = value;
            bar.MaxValue = _maxValue;
        }
    }

    [SerializeField]
    float _currentValue;

    public float CurrentValue
    {

        get
        {
            return _currentValue;
        }

        set
        {
            this._currentValue = value;
            this._currentValue = Mathf.Clamp(_currentValue, 0, MaxValue);

            if (bar != null)
            {
                bar.Value = _currentValue;
                bar.UpdateBar();
            }
        }
    }



}
