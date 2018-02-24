using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSupporter : MonoBehaviour
{

    public GameObject nextDialogue;

    public void Start()
    {
        if (nextDialogue)
            nextDialogue.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (DialogueManager.instance != null ? DialogueManager.instance.WaitingNextSentence() : false)
        {
            if (nextDialogue)
                nextDialogue.SetActive(true);
        }
        else
        {
            if (nextDialogue)
                nextDialogue.SetActive(false);
        }
    }
}
