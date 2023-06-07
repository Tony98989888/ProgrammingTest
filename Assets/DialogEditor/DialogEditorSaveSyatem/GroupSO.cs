
using UnityEngine;

namespace DialogEditor.Save
{
    public class GroupSO : ScriptableObject
    {
        public string GroupName { get; set; }

        public void Init(string name) 
        {
            GroupName = name;
        }
    }
}

