using System;
using System.Collections.Generic;
using System.Numerics;

namespace DialogEditor.Data.Save 
{
    [Serializable]
    public class DialogEditorNodeSaveData
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public List<DialogEditorChoiceSaveData> Choices { get; set; }

        public DialogType Dialogtype { get; set; }
        public Vector2 Position { get; set; }
    }
}

