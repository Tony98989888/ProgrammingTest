using DialogColor;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace DialogEditor 
{
    public class DialogEditorGroupErrorData
    {
        public DialogEditorErrorData ErrorData { get; set; }
        List<DialogNodeGroup> m_groups;

        public List<DialogNodeGroup> Groups => m_groups;

        public DialogEditorGroupErrorData() 
        {
            ErrorData = new DialogEditorErrorData();
            m_groups = new List<DialogNodeGroup>();
        }
    }
}

