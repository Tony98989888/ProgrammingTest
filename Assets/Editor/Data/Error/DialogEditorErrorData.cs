using DialogEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogColor 
{
    public class DialogEditorErrorData
    {
        Color m_color;
        public Color ErrorColor => m_color;

        public DialogEditorErrorData()
        {
            GenerateRandomColor();
    }

        public void GenerateRandomColor() 
        {
            m_color = new Color32(
                (byte)Random.Range(8,88),
                (byte)Random.Range(88,188),
                (byte)Random.Range(188, 255),
                255
                );
        }
    }
}

