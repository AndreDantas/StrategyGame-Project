using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
[System.Serializable]
public class Dialogue
{
    public string name;
    [TextArea(3, 10)]
    public List<string> sentences;
    public Color textColor = Color.black;
    public Sprite speaker;
    public AudioSource dialogueSound;

    public void PlayAudio()
    {
        if (dialogueSound != null)
            dialogueSound.Play();
    }

    public void StopAudio()
    {
        if (dialogueSound != null)
            dialogueSound.Stop();
    }
}
