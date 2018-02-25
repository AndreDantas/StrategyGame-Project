using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    /// <summary>
    /// The speaker's name.
    /// </summary>
    public string name;

    /// <summary>
    /// A list with the sentences.
    /// </summary>
    [TextArea(3, 10)]
    public List<string> sentences;
    /// <summary>
    /// The color of the text.
    /// </summary>
    public Color textColor = Color.black;
    /// <summary>
    /// The sprite of the speaker.
    /// </summary>
    public Sprite speaker;
    /// <summary>
    /// The sound that plays during dialogue.
    /// </summary>
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
