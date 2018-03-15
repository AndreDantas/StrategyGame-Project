using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class LevelUIController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public int levelIndex;
    [SerializeField]
    bool _locked;
    public bool locked
    {
        get
        {
            return _locked;
        }
        set
        {

            _locked = value;
            UpdateSprite();
        }
    }
    [SerializeField]
    bool _completed;
    public bool completed
    {
        get
        {
            return _completed;
        }
        set
        {
            _completed = value;
            UpdateSprite();
        }
    }
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    public Sprite completedSprite;
    public List<LevelUIController> unlockLevels = new List<LevelUIController>();
    // Use this for initialization
    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnValidate()
    {
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (spriteRenderer == null)
            return;
        if (locked)
            spriteRenderer.sprite = lockedSprite;
        else
        {
            if (completed)
                spriteRenderer.sprite = completedSprite;
            else
                spriteRenderer.sprite = unlockedSprite;
        }
    }

    private void OnMouseUpAsButton()
    {
        if (!locked)
            LevelManager.instance.LoadLevel(GetLevel());
    }

    public Level GetLevel()
    {
        Level l = new Level();
        l.levelIndex = levelIndex;
        l.locked = locked;
        l.completed = completed;
        return l;
    }

    public void SetLevel(Level l)
    {
        levelIndex = l.levelIndex;
        locked = l.locked;
        completed = l.completed;
        UpdateSprite();
    }

    public void CompleteLevel()
    {
        completed = true;
        foreach (LevelUIController l in unlockLevels)
        {
            l.locked = false;
        }
    }
}
