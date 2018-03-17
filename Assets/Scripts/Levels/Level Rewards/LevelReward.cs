using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public abstract class LevelReward : MonoBehaviour
{
    protected int rewardValue;
    public Image rewardImage;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI rewardValueText;

    public abstract void GetReward();

    public virtual void SetReward(int value)
    {
        rewardValue = value;
        if (rewardValueText)
            rewardValueText.text = value.ToString();
        rewardValue = value;
    }
}
