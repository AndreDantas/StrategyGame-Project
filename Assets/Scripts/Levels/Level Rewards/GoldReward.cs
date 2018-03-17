using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldReward : LevelReward
{
    public override void SetReward(int value)
    {
        base.SetReward(value);
        rewardValueText.text += "G";
        if (descriptionText)
            descriptionText.text = "Gold earned: ";
    }
    public override void GetReward()
    {
        GameData.instance.AddGold(rewardValue);
    }

}
