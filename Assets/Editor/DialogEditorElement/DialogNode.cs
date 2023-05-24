using System.Collections.Generic;
using System.Net.Mime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace DialogEditor
{
    // Should be added in graph view
    public class DialogNode : Node
    {
        public string DialogName { get; set; }
        public List<string> Choices { get; set; }

        public string Context { get; set; }
        public DialogType NodeType { get; set; }

        public virtual void Init(Vector2 initPos)
        {
            DialogName = "Name";
            Choices = new List<string>();
            Context = "Dialog Context";
            
            SetPosition(initPos.ToRect());
            // Add style sheet
            mainContainer.AddToClassList("dialogeditor-node-maincontainer");
            extensionContainer.AddToClassList("dialogeditor-node-extensioncontainer");
        }

        public virtual void InitNodeUI()
        {
            TextField dialogName = new TextField
            {
                value = "DialogName"
            };

            dialogName.AddToClassList("dialogeditor-node-textfield");
            dialogName.AddToClassList("dialogeditor-node-filename-textfield");
            dialogName.AddToClassList("dialogeditor-node-textfield-hidden");

            titleContainer.Insert(0, dialogName);

            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.portName = "Dialog Input Port";
            inputContainer.Add(inputPort);

            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("dialogeditor-node-custom-data-container");

            Foldout textFoldout = new Foldout
            {
                text = "Dialog Text"
            };

            TextField context = new TextField
            {
                value = Context
            };

            context.AddToClassList("dialogeditor-node-textfield");
            context.AddToClassList("dialogeditor-node-choice-textfield");
            context.AddToClassList("dialogeditor-node-textfield-hidden");

            textFoldout.Add(context);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);

        }
    }
}

