
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogEditor.Save 
{
    public class NodeSO
    {
        public string NodeName { get; set; }
        [field: TextArea]
        public string Text { get; set; }
        public List<ChoiceData> Choices { get; set; }
        public DialogType Dialogtype { get; set; }

        public bool IsStartNode { get; set; }

        public void Init(string dialogName, string text, List<ChoiceData> choices, DialogType type, bool isStartNode) 
        {
            NodeName = dialogName;
            Text = text;
            Choices = choices;
            Dialogtype = type;
            IsStartNode = isStartNode;
        }

    }
}

