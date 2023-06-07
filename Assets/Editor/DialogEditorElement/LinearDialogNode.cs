using DialogEditor.Data.Save;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace  DialogEditor
{
    public class LinearDialogNode : DialogNode
    {
        // For data
        public override void Init(DialogGraphView graphView, Vector2 initPos)
        {
            base.Init(graphView, initPos);
            NodeType = DialogType.Single;
            DialogEditorChoiceSaveData data = new DialogEditorChoiceSaveData()
            {
                Text = "Next Node",
            };
            Choices.Add(data);
        }


        // For view
        public override void InitNodeUI()
        {
            base.InitNodeUI();
            foreach (var data in Choices)
            {
                Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                    typeof(bool));
                port.userData = data;
                outputContainer.Add(port);
            }
            
            RefreshExpandedState();
        }
    }
}

