using DialogEditor.Helper;
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
            Button addChoiceBtn = DialogEditorElementHelper.CreateButton("Add Choice", () =>
            {
                Port port = AddChoicePort("New Dialog Choice");
                outputContainer.Add(port);
            });
            addChoiceBtn.AddToClassList("dialogeditor-node-button");

            mainContainer.Insert(1, addChoiceBtn);

            foreach (var data in Choices)
            {
                Port port = this.AddChoicePort(data);
                outputContainer.Add(port);
            }

            RefreshExpandedState();
        }

        Port AddChoicePort(string context)
        {
            Port port = this.CreatePort(string.Empty, Orientation.Horizontal, Direction.Output, Port.Capacity.Single);

            Button deleteBtn = DialogEditorElementHelper.CreateButton("Delete");
            deleteBtn.AddToClassList("dialogeditor-node-button");

            TextField textField = DialogEditorElementHelper.CreateTextField(context);
            textField.AddToClassList("dialogeditor-node-textfield");
            textField.AddToClassList("dialogeditor-node-filename-textfield");

            port.Add(deleteBtn);
            port.Add(textField);
            return port;
        }
    }
}

