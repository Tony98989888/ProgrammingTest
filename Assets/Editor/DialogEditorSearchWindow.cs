using DialogEditor.Helper;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace DialogEditor
{
    public class DialogEditorSearchWindow : ScriptableObject, ISearchWindowProvider
    {

        DialogGraphView m_GraphView;
        Texture2D m_Icon;

        public void Initialize(DialogGraphView graphView)
        {
            m_GraphView = graphView;
            m_Icon = new Texture2D(1, 1);
            m_Icon.SetPixel(0, 0, Color.clear);
            m_Icon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> entries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Dialog Node"), 1),
                new SearchTreeEntry(new GUIContent("Single Choice", m_Icon)){ level = 2, userData = DialogType.Single },
                new SearchTreeEntry(new GUIContent("Multi Choice", m_Icon)){ level = 2, userData = DialogType.Multiple },
                new SearchTreeGroupEntry(new GUIContent("Dialog Group"), 1),
                new SearchTreeEntry(new GUIContent("Create Group", m_Icon)){ level = 2, userData = new Group() },
            };

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var localMousePos = DialogEditorElementHelper.GetSearchWindowLocalMousePosition(context.screenMousePosition, m_GraphView);

            switch (SearchTreeEntry.userData)
            {
                case DialogType.Single:
                    LinearDialogNode singleNode = (LinearDialogNode)m_GraphView.InitNode(DialogType.Single, localMousePos);
                    m_GraphView.AddElement(singleNode);
                    return true;
                case DialogType.Multiple:
                    MultiDialogNode multiNode = (MultiDialogNode)m_GraphView.InitNode(DialogType.Multiple, localMousePos);
                    m_GraphView.AddElement(multiNode);
                    return true;
                case Group _:
                    Group group = m_GraphView.InitGroup("Dialog Group", localMousePos);
                    m_GraphView.AddElement(group);
                    return true;
                default:
                    return false;
            }
        }
    }
}

