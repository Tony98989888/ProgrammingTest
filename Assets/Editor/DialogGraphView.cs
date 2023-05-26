using DialogEditor.Helper;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace DialogEditor
{
    public class DialogGraphView : GraphView
    {

        DialogEditorSearchWindow m_searchWindow;
        public DialogGraphView()
        {
            InitManipulator();
            InitGridBackGround();
            InitSearchWindow();

            // Add style to graph view for customization
            InitGraphStyleSheet();
        }

        private void InitSearchWindow()
        {
            if (m_searchWindow == null) 
            {
                m_searchWindow = ScriptableObject.CreateInstance<DialogEditorSearchWindow>();
                m_searchWindow.Initialize(this);
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiablePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort == port) { return; }

                if (startPort.node == port.node) { return; }
                if (startPort.direction == port.direction) { return; }
                compatiablePorts.Add(port);
            });

            return compatiablePorts;
        }

        public DialogNode InitNode(DialogType nodeType, Vector2 pos)
        {
            Type nodeClassType;

            switch (nodeType)
            {
                case DialogType.Single:
                    nodeClassType = typeof(LinearDialogNode);
                    break;
                case DialogType.Multiple:
                    nodeClassType = typeof(MultiDialogNode);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null);
            }
            var node = (DialogNode)Activator.CreateInstance(nodeClassType);
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
            this.AddManipulator(InitMenuManipulator("Linear Choice Node", DialogType.Single));
            this.AddManipulator(InitMenuManipulator("Multi Choice Node", DialogType.Multiple));

            this.AddManipulator(InitGroupContextualMenu());
        }

        private IManipulator InitGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(InitGroup("Dialog Node Group", viewTransform.matrix.inverse.MultiplyPoint(actionEvent.eventInfo.localMousePosition)))));
            return contextualMenuManipulator;
        }

        public Group InitGroup(string title, Vector2 localMousePosition)
        {
            Group group = new Group()
            {
                title = title,
            };

            group.SetPosition(localMousePosition.ToRect());
            return group;
        }

        private void InitGraphStyleSheet()
        {
            this.ApplyStyleSheet("Assets/DialogEditorResource/DialogEditorNodeStyle.uss", 
                "Assets/DialogEditorResource/DialogEditorGraphViewStyleSheet.uss");
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
        private IManipulator InitMenuManipulator(string actionName, DialogType dialogType)
        {
            // Need to change mouse position to correct position
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(actionName, actionEvent => AddElement(InitNode(dialogType, viewTransform.matrix.inverse.MultiplyPoint(actionEvent.eventInfo.localMousePosition)))));
            return contextualMenuManipulator;
        }
    }
}