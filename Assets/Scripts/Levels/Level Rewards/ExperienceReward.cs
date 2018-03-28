using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceReward : LevelReward
{
    public override void GetReward()
    {
        GameData.instance.playerExperience += rewardValue;

    }

    public override void SetReward(int value)
    {
        base.SetReward(value);

        rewardValueText.text += " xp";

        if (descriptionText)
            descriptionText.text = "Experience earned: ";


    }
}
