
using System.Collections.Generic;
using UnityEngine;

namespace DialogEditor.Save 
{
    public class GraphSO : ScriptableObject
    {
        public string FileName { get; set; }

        public SerializableDictionary<GroupSO, List<NodeSO>> Groups { get; set; }

        public List<NodeSO> UnGroupedNodes { get; set; }

        public void Init(string fileName) 
        {
            FileName = fileName;
            Groups = new SerializableDictionary<GroupSO, List<NodeSO>>();
            UnGroupedNodes = new List<NodeSO>();
        }
    }
}

