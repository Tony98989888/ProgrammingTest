using DialogColor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogEditor 
{
    public class DialogEditorNodeErrorData 
    {
        public DialogEditorErrorData ErrorData;
        public List<DialogNode> DialogNodes;

        public DialogEditorNodeErrorData() 
        {
            ErrorData = new DialogEditorErrorData();
            DialogNodes = new List<DialogNode>();
        }
    }
}

