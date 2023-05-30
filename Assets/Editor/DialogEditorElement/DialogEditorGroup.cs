using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogEditor
{
    public class DialogEditorGroup : Group
    {
        Color m_defaultBorderColor;
        float m_defaultBorderWidth;

        public DialogEditorGroup() 
        {
            m_defaultBorderColor = contentContainer.style.borderBottomColor.value;
            m_defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
        }
    }
}

