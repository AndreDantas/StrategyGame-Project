using UnityEngine;
using System.Collections;

[System.Serializable]
public class Stat
{

    [SerializeField]
    Bar bar;

    public void SetBar(Bar bar)
    {
        this.bar = bar;

    }
    public Bar GetBar()
    {
        return bar;

    }
    public Stat()
    {

    }

    public Stat(Bar bar, float maxValue)
    {
        this.bar = bar;
        this.MaxValue = maxValue;
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
            if (value > MaxValue)
            {
                this._currentValue = MaxValue;
                if (bar)
                    bar.Value = _currentValue;
                return;
            }
            this._currentValue = value;
            if (bar)
            {
                bar.Value = _currentValue;
                bar.UpdateBar();
            }
        }
    }
}
