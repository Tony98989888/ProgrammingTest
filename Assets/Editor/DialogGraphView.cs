using System.Collections;
using System.Collections.Generic;
using Codice.CM.WorkspaceServer;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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

        private DialogNode InitNode(Vector2 pos)
        {
            var node = new DialogNode();
            node.Init(pos);
            node.InitNodeUI();
            AddElement(node);
            return node;
        }

        private void InitManipulator()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(InitMenuManipulator());
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

        // In this way we can use right click menu to add new node
        private IManipulator InitMenuManipulator()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction("Add Dialog Node", actionEvent => AddElement(InitNode(actionEvent.eventInfo.localMousePosition))));
            return contextualMenuManipulator;
        }
    }
}