using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SpriteSwap
{
    public Image imageRef;
    public List<Sprite> sprites;

    public void Swap(int index, Color color)
    {
        if (imageRef == null)
            return;
        if (sprites == null ? true : sprites.Count == 0)
            return;
        if (index < 0 || index >= sprites.Count)
            return;

        imageRef.sprite = sprites[index];
        imageRef.color = color;
    }
}