// Copyright (C) 2014 - 2016 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;
using System.Collections;

namespace TMPro.EditorUtilities
{
    [CustomEditor(typeof(TMP_SubMeshUI)), CanEditMultipleObjects]
    public class TMP_SubMeshUI_Editor : Editor
    {
        private TMP_SubMeshUI m_SubMeshComponent;

        private CanvasRenderer m_canvasRenderer;
        private Editor m_materialEditor;
        private Material m_targetMaterial;


        public void OnEnable()
        {
            // Get the UI Skin and Styles for the various Editors
            TMP_UIStyleManager.GetUIStyles();

            m_SubMeshComponent = target as TMP_SubMeshUI;
            //m_rectTransform = m_SubMeshComponent.rectTransform;
            m_canvasRenderer = m_SubMeshComponent.canvasRenderer;


            // Create new Material Editor if one does not exists
            if (m_canvasRenderer != null && m_canvasRenderer.GetMaterial() != null)
            {
                m_materialEditor = Editor.CreateEditor(m_canvasRenderer.GetMaterial());
                m_targetMaterial = m_canvasRenderer.GetMaterial();
            }
        }


        public void OnDisable()
        {
            // Destroy material editor if one exists
            if (m_materialEditor != null)
            {
                //Debug.Log("Destroying Inline Material Editor.");
                DestroyImmediate(m_materialEditor);
            }
        }



        public override void OnInspectorGUI()
        {
            // If a Custom Material Editor exists, we use it.
            if (m_canvasRenderer != null && m_canvasRenderer.GetMaterial() != null)
            {
                Material mat = m_canvasRenderer.GetMaterial();

                //Debug.Log(mat + "  " + m_targetMaterial);

                if (mat != m_targetMaterial)
                {
                    // Destroy previous Material Instance
                    //Debug.Log("New Material has been assigned.");
                    m_targetMaterial = mat;
                    DestroyImmediate(m_materialEditor);
                }


                if (m_materialEditor == null)
                {
                    m_materialEditor = Editor.CreateEditor(mat);
                }

                m_materialEditor.DrawHeader();


                m_materialEditor.OnInspectorGUI();
            }
        }

    }
}
