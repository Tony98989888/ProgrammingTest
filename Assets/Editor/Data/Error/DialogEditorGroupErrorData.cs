using DialogColor;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace DialogEditor 
{
    public class DialogEditorGroupErrorData
    {
        public DialogEditorErrorData ErrorData { get; set; }
        List<Group> m_groups;

        public List<Group> Groups => m_groups;

        public DialogEditorGroupErrorData() 
        {
            ErrorData = new DialogEditorErrorData();
            m_groups = new List<Group>();
        }
    }
}

