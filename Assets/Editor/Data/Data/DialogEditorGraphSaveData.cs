using System.Collections.Generic;
using UnityEngine;

namespace DialogEditor.Data.Save 
{
    public class DialogEditorGraphSaveData : ScriptableObject
    {
        public string FileName { get; set; }
        public List<DialogEditorGroupSaveData> Groups { get; set; }
        public List<DialogEditorNodeSaveData> Nodes { get; set; }
        public List<string> PreviousGroupNames { get; set; }
        public List<string> PreviousUngroupedNode { get; set; }

        public SerializableDictionary<string, List<string>> PreviousGroupedNodes { get; set; }

        public void Init(string fileName) 
        {
            FileName = fileName;

            Groups = new List<DialogEditorGroupSaveData>();
            Nodes = new List<DialogEditorNodeSaveData>();
        }
    }
}

