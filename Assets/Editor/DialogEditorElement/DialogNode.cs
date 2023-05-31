using DialogEditor.Helper;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogEditor
{
    // Should be added in graph view
    public class DialogNode : Node
    {
        public enum NodeStyle 
        {
            Error,
            Normal,
        }

        NodeStyle m_style = NodeStyle.Normal;

        DialogGraphView m_graphView;

        public string DialogName { get; set; }
        public List<string> Choices { get; set; }
        public string Context { get; set; }
        public DialogType NodeType { get; set; }

        // The group node is currently in
        private DialogNodeGroup m_group;
        public DialogNodeGroup ParentGroup { get => m_group; set => m_group = value; }

        public virtual void Init(DialogGraphView graphView, Vector2 initPos)
        {
            DialogName = "Name";
            Choices = new List<string>();
            Context = "Dialog Context";
            m_graphView = graphView;

            SetPosition(initPos.ToRect());
            // Add style sheet
            mainContainer.AddToClassList("dialogeditor-node-maincontainer");
            extensionContainer.AddToClassList("dialogeditor-node-extensioncontainer");
        }

        public virtual void InitNodeUI()
        {
            TextField dialogName = DialogEditorElementHelper.CreateTextField(DialogName, changeEvent => 
            {
                if (m_group == null)
                {
                    m_graphView.RemoveUngroupedNode(this);
                    DialogName = changeEvent.newValue;
                    m_graphView.AddUngroupedNode(this);
                }
                else
                {
                    DialogNodeGroup tmp = m_group;
                    // Group will be null
                    m_graphView.RemoveGroupedNode(this, m_group);
                    DialogName = changeEvent.newValue;
                    m_graphView.AddGroupedNode(this, tmp);
                }
            });

            dialogName.ApplyClasses("dialogeditor-node-textfield", "dialogeditor-node-filename-textfield", "dialogeditor-node-textfield-hidden");

            titleContainer.Insert(0, dialogName);

            Port inputPort = this.CreatePort("Dialog Input Port", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(inputPort);

            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("dialogeditor-node-custom-data-container");

            Foldout textFoldout = DialogEditorElementHelper.CreateFoldout("Dialog Text");

            TextField context = DialogEditorElementHelper.CreateTextArea(Context);

            context.AddToClassList("dialogeditor-node-textfield");
            context.AddToClassList("dialogeditor-node-choice-textfield");
            context.AddToClassList("dialogeditor-node-textfield-hidden");

            textFoldout.Add(context);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);
        }

        public void UpdateNodeColor(NodeStyle style, Color color) 
        {
            switch (style)
            {
                case NodeStyle.Error:
                    mainContainer.style.backgroundColor = color;
                    break;
                case NodeStyle.Normal:
                    mainContainer.style.backgroundColor = DialogEditorStyleSheetHelper.DefaultNodeBGColor;
                    break;
                default:
                    break;
            }
        }
    }
}

