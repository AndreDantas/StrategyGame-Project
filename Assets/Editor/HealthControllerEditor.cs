﻿using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(HealthController))]
public class HealthControllerEditor : Editor
{


    HealthController m_Instance;
    PropertyField[] m_fields;


    public void OnEnable()
    {
        m_Instance = target as HealthController;
        m_fields = ExposeProperties.GetProperties(m_Instance);
    }

    public override void OnInspectorGUI()
    {

        if (m_Instance == null)
            return;

        this.DrawDefaultInspector();

        ExposeProperties.Expose(m_fields);

    }
}