using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Level : MonoBehaviour
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
    public List<Level> unlockLevels = new List<Level>();

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
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
            LevelManager.instance.LoadLevel(this);
    }
}
