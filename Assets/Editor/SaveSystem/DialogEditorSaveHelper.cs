using DialogEditor.Data.Save;
using DialogEditor.Save;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DialogEditor
{
    public static class DialogEditorSaveHelper
    {

        static string GraphFileName;
        static string GraphFolderPath;
        static DialogGraphView DialogGraphView;

        static List<DialogNode> DialogNodes;
        static List<DialogNodeGroup> DialogGroups;

        static Dictionary<string, GroupSO> ExistingDialogGroups;
        static Dictionary<string, NodeSO> ExistingNodes;

        public static void Initialize(DialogGraphView graphView, string graphName)
        {
            DialogGraphView = graphView;
            GraphFileName = graphName;
            // Save each graph here
            GraphFolderPath = $"Assets/DialogueSystem/Dialogues/{graphName}";

            DialogNodes = new List<DialogNode>();
            DialogGroups = new List<DialogNodeGroup>();
            ExistingDialogGroups = new Dictionary<string, GroupSO>();
            ExistingNodes = new Dictionary<string, NodeSO>();
        }
        public static void Save()
        {
            InitGraphFolder();
            LoadGraph();
            DialogEditorGraphSaveData_SO graphData = InitAssets<DialogEditorGraphSaveData_SO>("Assets/Editor/DialogueSystem/Graphs", $"{GraphFileName}");
            graphData.Init(GraphFileName);
            GraphSO graphSO = InitAssets<GraphSO>(GraphFolderPath, GraphFileName);
            graphSO.Init(GraphFileName);

            // Save groups
            SaveGroupsData(graphData, graphSO);
            SaveNodesData(graphData, graphSO);
            SaveData(graphData);
            SaveData(graphSO);
        }

        private static void SaveGroupsData(DialogEditorGraphSaveData_SO graphSaveData, GraphSO graphSO)
        {

            List<string> groupNames = new List<string>();
            // Just add all things in GraphSO and DialogEditorGraphSaveData
            foreach (DialogNodeGroup group in DialogGroups)
            {
                SaveGroupToGraphData(group, graphSaveData);
                SaveRTGraphData(group, graphSO);
                groupNames.Add(group.title);
            }

            UpdateUnusedGroups(groupNames, graphSaveData);
        }

        private static void SaveGroupToGraphData(DialogNodeGroup group, DialogEditorGraphSaveData_SO data)
        {
            DialogEditorGroupSaveData saveData = new DialogEditorGroupSaveData()
            {
                Id = group.Id,
                Name = group.title,
                Position = group.GetPosition().position,
            };

            data.Groups.Add(saveData);
        }

        private static void SaveRTGraphData(DialogNodeGroup data, GraphSO graphSO)
        {
            var groupName = data.title;
            InitFolder($"{GraphFolderPath}/Groups", groupName);
            InitFolder($"{GraphFolderPath}/Groups/{groupName}", "Dialogues");

            GroupSO saveData = InitAssets<GroupSO>($"{GraphFolderPath}/Groups/{groupName}", groupName);
            saveData.Init(groupName);

            ExistingDialogGroups.Add(data.Id, saveData);

            graphSO.Groups.Add(saveData, new List<NodeSO>());
            // Need to set dirty
            SaveData(saveData);
        }

        private static void SaveNodesData(DialogEditorGraphSaveData_SO editorGraphData, GraphSO graphSO)
        {
            List<string> unusedUngroupedNodeNames = new List<string>();
            SerializableDictionary<string, List<string>> groupedNodes = new SerializableDictionary<string, List<string>>();

            foreach (DialogNode node in DialogNodes)
            {
                SaveNodesToGraphData(node, editorGraphData);
                SaveRTNodeData(node, graphSO);

                if (node.Group != null)
                {
                    groupedNodes.AddItem(node.Group.title, node.DialogName);
                    continue;
                }

                unusedUngroupedNodeNames.Add(node.DialogName);
            }

            UpdateDialogChoicesConnections();
            UpdateGroupedNodes(groupedNodes, editorGraphData);
            UpdateUngroupedNodes(unusedUngroupedNodeNames, editorGraphData);
        }

        private static void SaveNodesToGraphData(DialogNode node, DialogEditorGraphSaveData_SO editorGraphData)
        {
            // Solving Reference problem
            List<DialogEditorChoiceSaveData> choicesSaveData = new List<DialogEditorChoiceSaveData>();

            foreach (var choice in node.Choices)
            {
                DialogEditorChoiceSaveData choiceData = new DialogEditorChoiceSaveData()
                {
                    Text = choice.Text,
                    ChoiceId = choice.ChoiceId,
                };

                choicesSaveData.Add(choiceData);
            }

            DialogEditorNodeSaveData nodeData = new DialogEditorNodeSaveData()
            {
                Id = node.Id,
                Name = node.DialogName,
                Choices = choicesSaveData,
                Text = node.Context,
                GroupId = node.Group?.Id,
                Type = node.NodeType,
                Position = node.GetPosition().position,
            };

            editorGraphData.Nodes.Add(nodeData);
        }
        static List<ChoiceData> ToChoiceData(List<DialogEditorChoiceSaveData> nodeChoices)
        {
            List<ChoiceData> choicesData = new List<ChoiceData>();
            foreach (var choice in nodeChoices)
            {
                ChoiceData choiceData = new ChoiceData() { Text = choice.Text };
                choicesData.Add(choiceData);
            }

            return choicesData;
        }

        private static void SaveRTNodeData(DialogNode node, GraphSO graphSO)
        {
            NodeSO nodeSO;
            if (node.Group != null)
            {
                nodeSO = InitAssets<NodeSO>($"{GraphFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogName);
                graphSO.Groups.AddItem(ExistingDialogGroups[node.Group.Id], nodeSO);
            }
            else
            {
                nodeSO = InitAssets<NodeSO>($"{GraphFolderPath}/Groups/Global/Dialogues", node.DialogName);
                graphSO.UnGroupedNodes.Add(nodeSO);
            }

            nodeSO.Init(
            node.DialogName,
            node.Context,
            ToChoiceData(node.Choices),
            node.NodeType,
            node.IsFirstNode()
            );

            ExistingNodes.Add(node.Id, nodeSO);
            SaveData(nodeSO);
        }

        private static void UpdateDialogChoicesConnections()
        {
            foreach (DialogNode node in DialogNodes)
            {
                NodeSO nodeSO = ExistingNodes[node.Id];
                for (int i = 0; i < node.Choices.Count; i++)
                {
                    DialogEditorChoiceSaveData saveData = node.Choices[i];
                    if (string.IsNullOrEmpty(node.Id))
                    {
                        continue;
                    }

                    nodeSO.Choices[i].NextNode = ExistingNodes[saveData.ChoiceId];
                    SaveData(nodeSO);
                }
            }
        }

        private static void UpdateGroupedNodes(SerializableDictionary<string, List<string>> groupedNodesNames, DialogEditorGraphSaveData_SO graphData)
        {
            if (graphData.PreviousGroupedNodes != null && graphData.PreviousGroupedNodes.Count != 0)
            {
                foreach (var groupedNode in graphData.PreviousGroupedNodes)
                {
                    List<string> nodeToRemove = new List<string>();

                    if (groupedNodesNames.ContainsKey(groupedNode.Key))
                    {
                        nodeToRemove = groupedNode.Value.Except(groupedNodesNames[groupedNode.Key]).ToList();
                    }

                    foreach (var nodeName in nodeToRemove)
                    {
                        DeleteAsset($"{GraphFolderPath}/Groups/{groupedNode.Key}/Dialogues", nodeName);
                    }
                }
            }

            graphData.PreviousGroupedNodes = new SerializableDictionary<string, List<string>>(groupedNodesNames);
        }

        private static void UpdateUngroupedNodes(List<string> unusedUngroupedNodeNames, DialogEditorGraphSaveData_SO graphData)
        {
            if (graphData.PreviousUngroupedNodeNames != null && graphData.PreviousUngroupedNodeNames.Count != 0)
            {
                List<string> nodesToRemove = graphData.PreviousUngroupedNodeNames.Except(unusedUngroupedNodeNames).ToList();

                foreach (var node in nodesToRemove)
                {
                    DeleteAsset($"{GraphFolderPath}/Global/Dialogues", node);
                }
            }

            graphData.PreviousUngroupedNodeNames = new List<string>(unusedUngroupedNodeNames);
        }

        private static void UpdateUnusedGroups(List<string> groupNames, DialogEditorGraphSaveData_SO data)
        {
            // Remove Unused groups
            if (data.PreviousGroupNames != null && data.PreviousGroupNames.Count != 0)
            {
                List<string> readyRemoveNames = data.PreviousGroupNames.Except(groupNames).ToList();
                foreach (string name in readyRemoveNames)
                {
                    // RemoveFolderHere
                    DeleteFolder($"{GraphFolderPath}/Groups/{name}");
                }
            }

            data.PreviousGroupNames = new List<string>(groupNames);
        }

        private static void DeleteFolder(string folderPath)
        {
            FileUtil.DeleteFileOrDirectory($"{folderPath}.meta");
            FileUtil.DeleteFileOrDirectory($"{folderPath}/");
        }

        private static void SaveData(UnityEngine.Object saveData)
        {
            EditorUtility.SetDirty(saveData);
            // Calling next functions to ensure asset will be generated or sometimes it fails 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static T InitAssets<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";
            T asset = LoadAsset<T>(path, assetName);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, fullPath);
            }
            return asset;
        }

        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject 
        {
            string fullPath = $"{path}/{assetName}.asset";
            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        private static void DeleteAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        private static void LoadGraph()
        {
            foreach (var item in DialogGraphView.graphElements)
            {
                if (item is DialogNode node)
                {
                    DialogNodes.Add(node);
                    return;
                }

                if (item.GetType() == typeof(DialogNodeGroup))
                {
                    DialogNodeGroup group = (DialogNodeGroup)item;
                    DialogGroups.Add(group);
                    return;
                }
            }
        }

        static void InitGraphFolder()
        {
            InitFolder("Assets/Editor", "DialogueSystem");
            InitFolder("Assets/Editor/DialogueSystem", "Graphs");
            InitFolder("Assets", "DialogueSystem");
            InitFolder("Assets/DialogueSystem", "Dialogues");
            InitFolder("Assets/DialogueSystem/Dialogues", GraphFileName);
            InitFolder(GraphFolderPath, "Global");
            InitFolder(GraphFolderPath, "Groups");
            InitFolder($"{GraphFolderPath}/Global", "Dialogues");
        }

        private static void InitFolder(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
            {
                // path exist
                return;
            }

            AssetDatabase.CreateFolder(path, folderName);
        }
    }

}


