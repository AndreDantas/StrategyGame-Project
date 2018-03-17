using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public struct RewardDictonary
{
    public string rewardName;
    public GameObject rewardPrefab;
}
public class LevelRewardManager : MonoBehaviour
{
    public static LevelRewardManager instance;
    public List<RewardDictonary> rewards = new List<RewardDictonary>();
    protected List<LevelReward> rewardsReference;
    public GameObject rewardsDisplayObject;

    public delegate void OnContinueEventHandler();
    public OnContinueEventHandler OnContinue;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        rewardsReference = new List<LevelReward>();
        instance = this;
    }

    public void AddReward(string name, int value)
    {
        foreach (RewardDictonary reward in rewards)
        {
            if (reward.rewardName.Trim().ToLower() == name.Trim().ToLower())
            {
                if (rewardsDisplayObject)
                {
                    LevelReward lr;
                    if ((lr = reward.rewardPrefab.GetComponent<LevelReward>()) != null)
                    {
                        GameObject newReward = Instantiate(reward.rewardPrefab);
                        newReward.transform.SetParent(rewardsDisplayObject.transform);
                        lr.SetReward(value);
                        rewardsReference.Add(lr);
                    }

                }
                return;
            }
        }
    }

    private void OnDisable()
    {
        ClearRewards();

    }

    public void ClearRewards()
    {
        rewardsReference.Clear();
        if (rewardsDisplayObject)
        {
            List<GameObject> destroyList = new List<GameObject>();
            foreach (Transform child in rewardsDisplayObject.transform)
            {
                destroyList.Add(child.gameObject);
            }

            for (int i = destroyList.Count - 1; i >= 0; i--)
            {
                Destroy(destroyList[i]);
            }
        }
    }

    public void Continue()
    {
        if (OnContinue != null)
            OnContinue();
    }

    public List<LevelReward> GetRewards()
    {
        return rewardsReference;
    }
}
