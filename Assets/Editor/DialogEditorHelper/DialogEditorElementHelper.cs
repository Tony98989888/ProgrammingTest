using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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

        public static Vector2 GetScreenLocalMousePosition(Vector2 mousePos, GraphView graphView) 
        {
            // Need to turn a world position point into element space
            Vector2 worldMousePos = mousePos;
            Vector2 localMousePos = graphView.contentViewContainer.WorldToLocal(worldMousePos);
            return localMousePos;
        }

        public static Vector2 GetSearchWindowLocalMousePosition(Vector2 mousePos, DialogGraphView graphView)
        { // Need to turn a world position point into element space
            Vector2 worldMousePos = mousePos;
            worldMousePos -= graphView.Parent.position.position;
            Vector2 localMousePos = graphView.contentViewContainer.WorldToLocal(worldMousePos);
            return localMousePos;
        }
    }
}
