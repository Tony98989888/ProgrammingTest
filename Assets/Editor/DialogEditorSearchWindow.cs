using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace DialogEditor
{
    public class DialogEditorSearchWindow : ScriptableObject, ISearchWindowProvider
    {

        DialogGraphView m_GraphView;
        public void Initialize(DialogGraphView graphView)
        {
            m_GraphView = graphView;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Dialog Node"), 1),
                new SearchTreeEntry(new GUIContent("Single Choice")){ level = 2, userData = DialogType.Single },
                new SearchTreeEntry(new GUIContent("Multi Choice")){ level = 2, userData = DialogType.Multiple },
                new SearchTreeGroupEntry(new GUIContent("Dialog Group"), 1),
                new SearchTreeEntry(new GUIContent("Create Group")){ level = 2, userData = new Group() },
            };

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            switch (SearchTreeEntry.userData)
            {
                case DialogType.Single:
                    LinearDialogNode singleNode = (LinearDialogNode)m_GraphView.InitNode(DialogType.Single, context.screenMousePosition);
                    m_GraphView.AddElement(singleNode);
                    return true;
                case DialogType.Multiple:
                    MultiDialogNode multiNode = (MultiDialogNode)m_GraphView.InitNode(DialogType.Multiple, context.screenMousePosition);
                    m_GraphView.AddElement(multiNode);
                    return true;
                case Group _:
                    Group group = m_GraphView.InitGroup("Dialog Group", context.screenMousePosition);
                    m_GraphView.AddElement(group);
                    return true;
                default:
                    return false;
            }
        }
    }
}

