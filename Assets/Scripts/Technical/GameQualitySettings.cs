using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameQualitySettings : MonoBehaviour
{
    public static GameQualitySettings instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }
}
