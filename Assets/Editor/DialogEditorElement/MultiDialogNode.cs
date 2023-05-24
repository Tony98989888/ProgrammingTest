using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogEditor
{
    public class MultiDialogNode : DialogNode
    {
        public override void Init(Vector2 initPos)
        {
            base.Init(initPos);
            NodeType = DialogType.Multiple;
            Choices.Add("New Choice");
        }

        public override void InitNodeUI()
        {
            base.InitNodeUI();
            Button addChoiceBtn = new Button(){text = "Add Choice"};
            addChoiceBtn.AddToClassList("dialogeditor-node-button");

            mainContainer.Insert(1, addChoiceBtn);
            
            foreach (var data in Choices)
            {
                Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                    typeof(bool));
                port.portName = string.Empty;

                Button deleteBtn = new Button() { text = "Delete" };
                deleteBtn.AddToClassList("dialogeditor-node-button");

                TextField textField = new TextField() { value = data};
                textField.AddToClassList("dialogeditor-node-textfield");
                textField.AddToClassList("dialogeditor-node-filename-textfield");

                port.Add(deleteBtn);
                port.Add(textField);
                
                outputContainer.Add(port);
            }
            
            RefreshExpandedState();
        }
    }
}

