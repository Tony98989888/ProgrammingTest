using DialogEditor.Data.Save;
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
        // Unique id for reference
        public string Id;

        public enum NodeStyle 
        {
            Error,
            Normal,
        }

        NodeStyle m_style = NodeStyle.Normal;

        protected DialogGraphView m_graphView;

        public string DialogName { get; set; }
        public List<DialogEditorChoiceSaveData> Choices { get; set; }
        public string Context { get; set; }
        public DialogType NodeType { get; set; }

        // The group node is currently in
        private DialogNodeGroup m_group;
        public DialogNodeGroup ParentGroup { get => m_group; set => m_group = value; }

        public virtual void Init(DialogGraphView graphView, Vector2 initPos)
        {
            Id = Guid.NewGuid().ToString();

            DialogName = "Name";
            Choices = new List<DialogEditorChoiceSaveData>();
            Context = "Dialog Context";
            m_graphView = graphView;

            SetPosition(initPos.ToRect());
            // Add style sheet
            mainContainer.AddToClassList("dialogeditor-node-maincontainer");
            extensionContainer.AddToClassList("dialogeditor-node-extensioncontainer");
        }

        public virtual void InitNodeUI()
        {
            TextField dialogName = DialogEditorElementHelper.CreateTextField(DialogName, null, changeEvent => 
            {
                TextField tmpTextField = (TextField)changeEvent.target;
                tmpTextField.value = DialogEditorStringHelper.FormatText(changeEvent.newValue);

                if (m_group == null)
                {
                    m_graphView.RemoveUngroupedNode(this);
                    DialogName = tmpTextField.value;
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

            TextField context = DialogEditorElementHelper.CreateTextArea(Context, null, cb => 
            {
                Context = cb.newValue;
            });

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

        public void ClearPortEdges(VisualElement container) 
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected) { continue; }
                m_graphView.DeleteElements(port.connections);
            }
        }

        public void ClearAllPorts() 
        {
            ClearPortEdges(inputContainer);
            ClearPortEdges(outputContainer);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            evt.menu.AppendAction("Clear Input Ports", actionEvent => ClearPortEdges(inputContainer));
            evt.menu.AppendAction("Clear Output Ports", actionEvent => ClearPortEdges(outputContainer));
        }
    }
}

