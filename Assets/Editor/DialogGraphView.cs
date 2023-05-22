using System.Collections;
using System.Collections.Generic;
using Codice.CM.WorkspaceServer;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;


namespace DialogEditor
{
    public class DialogGraphView : GraphView
    {
        public DialogGraphView()
        {
            InitManipulator();
            InitGridBackGround();
            // Add style to graph view for customization
            InitGraphStyleSheet();
        }

        private void InitManipulator()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
        }

        private void InitGraphStyleSheet()
        {
            var styleSheet =
                EditorGUIUtility.Load("Assets/DialogEditorResource/DialogEditorGraphViewStyleSheet.uss") as StyleSheet;
            styleSheets.Add(styleSheet);
        }

        /*
         * First need to init grid to add element
         */
        private void InitGridBackGround()
        {
            GridBackground backGround = new GridBackground();
            backGround.StretchToParentSize();
            Insert(0, backGround);
        }
    }
}