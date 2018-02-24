using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetCreator : MonoBehaviour
{

    [MenuItem("Assets/Create/Conversation Data")]
    public static void CreateConversationData()
    {
        ScriptableObjectUtility.CreateAsset<ConversationData>();
    }
}
