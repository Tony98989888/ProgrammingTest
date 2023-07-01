using DialogEditor.Helper;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogEditor
{
    public class DialogEditorWindow : EditorWindow
    {

        Button m_saveButton;
        TextField m_fileNameTextField;
        DialogGraphView m_graphView;

        [MenuItem("Window/UI Toolkit/DialogEditorWindow")]
        public static void ShowExample()
        {
            DialogEditorWindow wnd = GetWindow<DialogEditorWindow>();
            wnd.titleContent = new GUIContent("DialogEditor");
        }

        public void CreateGUI()
        {
            InitGraphView();
            // Each editor window contains a root VisualElement object
            // VisualElement root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy.
            InitToolBar();


        }
        private void OnEnable()
        {
            InitGraphView();
            InitStyleSheets();
        }

        private void InitToolBar()
        {
            Toolbar toolbar = new Toolbar();
            m_fileNameTextField = DialogEditorElementHelper.CreateTextField(DialogEditorElementHelper.DefaultFileName, DialogEditorElementHelper.DefaultLabelName
                , callback =>
                {
                    m_fileNameTextField.value = DialogEditorStringHelper.FormatText(callback.newValue);
                }
                );
            m_saveButton = DialogEditorElementHelper.CreateButton(DialogEditorElementHelper.DefaultSaveButtonName, () => Save());
            toolbar.Add(m_fileNameTextField);
            toolbar.Add(m_saveButton);
            toolbar.ApplyStyleSheet("Assets/DialogEditorResource/DialogEditorToolbarStyle.uss");

            rootVisualElement.Add(toolbar);
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(m_fileNameTextField.text)) 
            {
                EditorUtility.DisplayDialog("Invalid File Name", "File name can not be empty!", "Yes");
                return;
            }
            DialogEditorSaveHelper.Initialize(m_graphView, m_fileNameTextField.value);
            DialogEditorSaveHelper.Save();
        }

        private void InitStyleSheets()
        {
            rootVisualElement.ApplyStyleSheet("Assets/DialogEditorResource/DialogEditorVariables.uss");
        }

        private void InitGraphView()
        {
            m_graphView = new DialogGraphView(this);
            m_graphView.StretchToParentSize();
            rootVisualElement.Add(m_graphView);
        }

        public void ActiveSaveButton(bool state)
        {
            m_saveButton.SetEnabled(state);
        }
    }
}