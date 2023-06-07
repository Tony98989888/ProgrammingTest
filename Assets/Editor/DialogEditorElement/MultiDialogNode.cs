using DialogEditor.Data.Save;
using DialogEditor.Helper;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogEditor
{
    public class MultiDialogNode : DialogNode
    {
        public override void Init(DialogGraphView graphView, Vector2 initPos)
        {
            base.Init(graphView, initPos);
            NodeType = DialogType.Multiple;
            DialogEditorChoiceSaveData data = new DialogEditorChoiceSaveData
            {
                Text = "New Choice",
            };

            Choices.Add(data);
        }

        public override void InitNodeUI()
        {
            base.InitNodeUI();
            Button addChoiceBtn = DialogEditorElementHelper.CreateButton("Add Choice", () =>
            {
                DialogEditorChoiceSaveData data = new DialogEditorChoiceSaveData
                {
                    Text = "New Choice",
                };

                Choices.Add(data);

                Port port = AddChoicePort(data);
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

        Port AddChoicePort(object userData)
        {
            Port port = this.CreatePort(string.Empty, Orientation.Horizontal, Direction.Output, Port.Capacity.Single);
            port.userData = userData;

            DialogEditorChoiceSaveData choiceData = (DialogEditorChoiceSaveData)userData;

            Button deleteBtn = DialogEditorElementHelper.CreateButton("Delete", () => 
            {
                // Do not delete ports when there is oly one choice
                if (Choices.Count == 1)
                {
                    return;
                }

                if (port.connected)
                {
                    m_graphView.DeleteElements(port.connections);
                }

                Choices.Remove(choiceData);
                m_graphView.RemoveElement(port);
            });
            deleteBtn.AddToClassList("dialogeditor-node-button");

            TextField textField = DialogEditorElementHelper.CreateTextField(choiceData.Text, null, callback => 
            {
                choiceData.Text = callback.newValue;
            });
            textField.AddToClassList("dialogeditor-node-textfield");
            textField.AddToClassList("dialogeditor-node-filename-textfield");

            port.Add(deleteBtn);
            port.Add(textField);
            return port;
        }
    }
}

