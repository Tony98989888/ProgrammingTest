
using System;
using UnityEngine;

namespace DialogEditor.Data.Save
{
    [Serializable]
    public class DialogEditorGroupSaveData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Vector2 Position { get; set; }
    }
}
