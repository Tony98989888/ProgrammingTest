using System;

namespace DialogEditor.Save
{
    [Serializable]
    public class ChoiceData
    {
        public string Text { get; set; }
        public NodeSO NextNode { get; set; }

    }
}

