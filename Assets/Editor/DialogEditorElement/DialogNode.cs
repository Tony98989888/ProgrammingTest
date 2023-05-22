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
        }

        public virtual void InitNodeUI()
        {
            TextField dialogName = new TextField
            {
                value = "DialogName"
            };
            
            titleContainer.Insert(0, dialogName);

            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.portName = "Dialog Input Port";
            inputContainer.Add(inputPort);

            VisualElement customDataContainer = new VisualElement();
            Foldout textFoldout = new Foldout
            {
                text = "Dialog Text"
            };

            TextField context = new TextField
            {
                value = Context
            };
            
            textFoldout.Add(context);
            customDataContainer.Add(textFoldout);
            extensionContainer.Add(customDataContainer);

        }
    }
}

