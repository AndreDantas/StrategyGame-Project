using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public delegate void OnBackPressEventHandler();
    public event OnBackPressEventHandler OnBackPress;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
            instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackPress();
        }
    }

    public void BackPress()
    {
        if (OnBackPress != null)
            OnBackPress();
    }
}
