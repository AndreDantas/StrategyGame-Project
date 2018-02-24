using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using TMPro;

public enum DialogueSpeed
{
    Slow,
    Normal,
    Fast
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    /// <summary>
    /// The current list of sentences.
    /// </summary>
    protected Queue<string> sentences;
    /// <summary>
    /// The current list of dialogues
    /// </summary>
    protected Queue<Dialogue> dialogues;
    /// <summary>
    /// The main text to type the dialogue.
    /// </summary>
    public TextMeshProUGUI mainDialogueText;
    /// <summary>
    /// The speaker sprite.
    /// </summary>
    public Image speakerSprite;
    public delegate void DialogueEventHandler();

    public event DialogueEventHandler OnComplete;
    /// <summary>
    /// The current dialogue.
    /// </summary>
    protected Dialogue currentDialogue;
    /// <summary>
    /// The speed of the dialogue
    /// </summary>
    public DialogueSpeed dialogueSpeed = DialogueSpeed.Normal;
    [Range(0, 1)]
    public float slowSpeed;
    [Range(0, 1)]
    public float normalSpeed;
    [Range(0, 1)]
    public float fastSpeed;

    /// <summary>
    /// Used to inform if a text is being altered or typed.
    /// </summary>
    protected bool alteringText = false;
    /// <summary>
    /// Used to inform that the next sentence is required.
    /// </summary>
    protected bool nextSentence = false;
    /// <summary>
    /// If the conversation is done.
    /// </summary>
    protected bool isDone = true;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(gameObject);
        sentences = new Queue<string>();

    }

    private void Start()
    {

    }

    /// <summary>
    /// Initiates the conversation.
    /// </summary>
    /// <param name="conversation">The conversation data.</param>
    public void StartConversation(ConversationData conversation)
    {
        if (mainDialogueText == null || conversation == null)
            return;
        if (conversation.dialogueList != null ? conversation.dialogueList.Count == 0 : true)
            return;

        if (IsAltering())
        {
            nextSentence = true;
            return;

        }

        isDone = false;

        dialogues = new Queue<Dialogue>();


        foreach (Dialogue d in conversation.dialogueList)
        {
            dialogues.Enqueue(d);
        }

        currentDialogue = dialogues.Dequeue();
        LoadSentences(currentDialogue);
        NextSentence();
    }

    /// <summary>
    /// Loads the current sentences.
    /// </summary>
    /// <param name="dialogue"></param>
    public void LoadSentences(Dialogue dialogue)
    {
        sentences = new Queue<string>();

        foreach (string s in currentDialogue.sentences)
        {
            sentences.Enqueue(s);
        }
    }

    /// <summary>
    /// Calls the next sentence in the sequence. 
    /// </summary>
    /// <param name="instant"></param>
    public void NextSentence(bool instant = false)
    {
        if (mainDialogueText == null)
            return;

        if (IsAltering())
        {
            nextSentence = true;
            return;

        }

        // This loop will keep searching for the next sentences in the dialogues. If there are no more, it ends the conversation.
        while (sentences == null ? true : sentences.Count == 0) // Check for valid sentences.
        {
            if (dialogues != null ? dialogues.Count > 0 : false) // Check if there are dialogues left
            {
                currentDialogue = dialogues.Dequeue(); // Get the next dialogue
                LoadSentences(currentDialogue);
            }
            else // No more dialogues or sentences, end conversation.
            {
                EndConversation();
                return;
            }
        }

        mainDialogueText.text = "";
        mainDialogueText.color = currentDialogue.textColor;

        if (speakerSprite != null)
            speakerSprite.sprite = currentDialogue.speaker;

        if (instant)
        {
            mainDialogueText.text = RemoveCmds(sentences.Dequeue());
        }
        else
            StartCoroutine(StartTypingText(sentences.Dequeue()));

    }

    /// <summary>
    /// Executes at the end of the dialogue chain
    /// </summary>
    protected void EndConversation()
    {
        if (mainDialogueText != null)
            mainDialogueText.text = "";
        isDone = true;
        if (OnComplete != null)
            OnComplete();
    }


    /// <summary>
    /// Executes before a sentence starts.
    /// </summary>
    protected void StartSentence()
    {
        alteringText = true;
        mainDialogueText.text = "";
        currentDialogue.PlayAudio();

    }
    /// <summary>
    /// Executes after a sentence ends
    /// </summary>
    protected void EndSentence()
    {
        currentDialogue.StopAudio();

        alteringText = false;
        nextSentence = false;
    }


    IEnumerator StartTypingText(string sentence)
    {
        if (mainDialogueText == null || alteringText)
            yield break;

        StartSentence();

        yield return TypeText(sentence, mainDialogueText);

        EndSentence();

    }

    /// <summary>
    /// Types the text.
    /// </summary>
    /// <param name="sentence">The sentence to be typed.</param>
    /// <param name="textBox">The text box to type the sentence.</param>
    /// <returns></returns>
    IEnumerator TypeText(string sentence, TextMeshProUGUI textBox)
    {

        dialogueSpeed = DialogueSpeed.Normal; // Start velocity at normal.
        sentence = sentence.Trim();
        for (int i = 0; i < sentence.Length; i++)
        {
            if (sentence[i] == '<') //The start of a command
            {
                string command = "";
                bool validCmd = false;
                int index = 0;

                for (int j = i; j < sentence.Length; j++)
                {
                    command += sentence[j];
                    if (sentence[j] == '>')
                    {
                        validCmd = true;
                        index = j;
                        break;
                    }
                }
                if (validCmd)
                {
                    i = index;
                    //Execute command
                    Regex rgx = new Regex("[^a-zA-Z0-9 -]");

                    command = rgx.Replace(command, "");
                    yield return ExecuteCommand(command);
                }
            }
            else
            {
                textBox.text += sentence[i];
                switch (dialogueSpeed)
                {
                    case DialogueSpeed.Slow:
                        yield return new WaitForSeconds(slowSpeed);
                        break;
                    case DialogueSpeed.Normal:
                        yield return new WaitForSeconds(normalSpeed);
                        break;
                    case DialogueSpeed.Fast:
                        yield return new WaitForSeconds(fastSpeed);
                        break;
                }

            }


            if (nextSentence) // If the next sentence is required, finish typing.
            {
                textBox.text = RemoveCmds(sentence);
                break;
            }
        }

    }

    /// <summary>
    /// Executes a command. Comand format: <commandcode>
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    IEnumerator ExecuteCommand(string command)
    {
        float counter = 0f;
        command = command.Trim().ToLower();
        switch (command)
        {
            case "s": // Sets the dialogue speed to slow.
                dialogueSpeed = DialogueSpeed.Slow;
                break;
            case "n": // Sets the dialogue speed to normal.
                dialogueSpeed = DialogueSpeed.Normal;
                break;
            case "f": // Sets the dialogue speed to fast.
                dialogueSpeed = DialogueSpeed.Fast;
                break;
            case "w": // Stops the typing for 1 second.
                counter = 0f;
                if (currentDialogue != null)
                    currentDialogue.StopAudio();

                while (!nextSentence && counter <= 1f)
                {
                    yield return null;
                    counter += Time.deltaTime;
                }

                if (currentDialogue != null)
                    currentDialogue.PlayAudio();
                break;

            case "wh": // Stops the typing for 0.5 second.
                counter = 0f;
                if (currentDialogue != null)
                    currentDialogue.StopAudio();

                while (!nextSentence && counter <= 0.5f)
                {
                    yield return null;
                    counter += Time.deltaTime;
                }

                if (currentDialogue != null)
                    currentDialogue.PlayAudio();
                break;
        }
    }

    /// <summary>
    /// Removes the command from string.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public string RemoveCmds(string s)
    {
        bool command = false;
        string getCmd = "";
        string resultText = "";
        foreach (char letter in s)
        {
            if (!command)
            {
                if (letter == '<')
                {
                    command = true;
                    getCmd += letter;
                }
                else
                {
                    resultText += letter;
                }
            }
            else
            {
                if (letter == '>')
                {
                    command = false;
                    getCmd = "";
                }
                else
                {
                    getCmd += letter;
                }
            }
        }
        return resultText + getCmd;
    }


    IEnumerator CreateText(string sentence, TextMeshProUGUI textBox)
    {
        if (textBox == null || alteringText)
            yield break;
        alteringText = true;
        if (currentDialogue.dialogueSound != null)
        {
            currentDialogue.dialogueSound.Play();
        }
        yield return TypeText(sentence, textBox);
        if (currentDialogue.dialogueSound != null)
        {
            currentDialogue.dialogueSound.Stop();
        }
        alteringText = false;
        nextSentence = false;
    }

    public void Delete()
    {
        StartCoroutine(DeleteText());
    }

    /// <summary>
    /// Deletes the text.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DeleteText()
    {
        if (mainDialogueText == null ? true : mainDialogueText.text.Trim() == "")
            yield break;
        if (IsAltering())
            yield break;
        alteringText = true;

        for (int i = mainDialogueText.text.Length - 1; i >= 0; i--)
        {
            mainDialogueText.text = mainDialogueText.text.Substring(0, mainDialogueText.text.Length - 1);

            yield return new WaitForSeconds(fastSpeed);
            if (nextSentence)
            {
                mainDialogueText.text = "";
                break;
            }
        }

        alteringText = false;
        nextSentence = false;
    }


    public bool IsAltering()
    {
        return alteringText;
    }

    public bool IsDone()
    {
        return isDone;
    }

    public bool WaitingNextSentence()
    {
        return (!isDone && !alteringText);
    }
}
