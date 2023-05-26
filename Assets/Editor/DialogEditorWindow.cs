using DialogEditor.Helper;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogEditor
{
    public class DialogEditorWindow : EditorWindow
    {
        [MenuItem("Window/UI Toolkit/DialogEditorWindow")]
        public static void ShowExample()
        {
            DialogEditorWindow wnd = GetWindow<DialogEditorWindow>();
            wnd.titleContent = new GUIContent("DialogEditor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
        
            // VisualElements objects can contain other VisualElement following a tree hierarchy.
            VisualElement label = new Label("Hello World! From C#");
            root.Add(label);
            InitGraphView();
        }

        private void OnEnable()
        {
            InitGraphView();
            InitStyleSheets();
        }

        private void InitStyleSheets()
        {
            rootVisualElement.ApplyStyleSheet("Assets/DialogEditorResource/DialogEditorVariables.uss");
        }

        private void InitGraphView()
        {
            var graphView = new DialogGraphView();
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }
    }
}