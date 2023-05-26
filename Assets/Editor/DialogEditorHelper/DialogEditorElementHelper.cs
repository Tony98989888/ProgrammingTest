using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DialogEditor.Helper
{
    public static class DialogEditorElementHelper
    {
        public static Foldout CreateFoldout(string name = "Foldout", bool collapsed = false)
        {
            Foldout foldout = new Foldout()
            {
                text = name,
                value= collapsed,
            };

            return foldout;
        }

        public static Button CreateButton(string name, Action onClick = null) 
        {
            Button button = new Button(onClick)
            {
                text = name,
            };

            return button;
        }

        public static TextField CreateTextField(string name = "TextField", EventCallback<ChangeEvent<string>> onValueChange = null)
        {
            TextField textField = new TextField()
            {
                value = name,
            };

            if (onValueChange != null)
            {
                textField.RegisterValueChangedCallback(onValueChange);
            }
            return textField;
        }

        public static TextField CreateTextArea(string name = "TextField", EventCallback<ChangeEvent<string>> onValueChange = null)
        {
            TextField textarea = CreateTextField(name, onValueChange);
            textarea.multiline = true;
            return textarea;
        }

        public static Port CreatePort(this DialogNode node, string portName = "", Orientation orientation = Orientation.Horizontal
            , Direction dir = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)  
        {
            Port port = node.InstantiatePort(orientation, dir, capacity, typeof(bool));
            port.portName = portName;
            return port;
        }
    }
}
