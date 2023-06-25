using DialogEditor.Data.Save;
using DialogEditor.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

        static void Initialize(DialogGraphView graphView, string graphName)
        {
            DialogGraphView = graphView;
            GraphFileName = graphName;
            GraphFolderPath = $"Assets/DialogSaveFile/Dialogues/{graphName}";

            DialogNodes = new List<DialogNode>();
            DialogGroups = new List<DialogNodeGroup>();
            ExistingDialogGroups = new Dictionary<string, GroupSO>();
            ExistingNodes = new Dictionary<string, NodeSO>();
        }

        public static void Save()
        {
            InitGraphFolder();
            LoadGraph();
            DialogEditorGraphSaveData data = InitAssets<DialogEditorGraphSaveData>("Assets/Editor/DialogEditor/Graphs", $"{GraphFileName}_Graph");
            data.Init(GraphFileName);
            GraphSO graphSO = InitAssets<GraphSO>(GraphFolderPath, GraphFileName);
            graphSO.Init(GraphFileName);

            // Save groups
            SaveGroupData(data, graphSO);
            SaveNodeData(data, graphSO);
            SaveData(data);
            SaveData(graphSO);
        }

        private static void SaveNodeData(DialogEditorGraphSaveData data, GraphSO graphSO)
        {
            List<string> unusedUngroupedNodeNames = new List<string>();
            SerializableDictionary<string, List<string>> groupedNodes = new SerializableDictionary<string, List<string>>();

            foreach (DialogNode node in DialogNodes)
            {
                SaveNodeToGraph(node, data);
                SaveSONode(node, graphSO);

                if (node.ParentGroup != null)
                {
                    groupedNodes.AddItem(node.ParentGroup.title, node.DialogName);
                    continue;
                }

                unusedUngroupedNodeNames.Add(node.DialogName);
            }

            SaveDialogChoiceConnection();
            UpdateGroupedNodes(groupedNodes, data);
            UpdateUngroupedNodes(unusedUngroupedNodeNames, data);
        }

        private static void UpdateGroupedNodes(SerializableDictionary<string, List<string>> groupedNodes, DialogEditorGraphSaveData data)
        {
            if (data.PreviousGroupedNodes != null && data.PreviousGroupedNodes.Count != 0)
            {
                foreach (var groupedNode in data.PreviousGroupedNodes)
                {
                    List<string> nodeToRemove = new List<string>();

                    if (groupedNodes.ContainsKey(groupedNode.Key))
                    {
                        nodeToRemove = groupedNode.Value.Except(groupedNodes[groupedNode.Key]).ToList();
                    }

                    foreach (var nodeName in nodeToRemove)
                    {
                        DeleteAsset($"{GraphFolderPath}/Groups/{groupedNode.Key}/Dialogues", nodeName);
                    }
                }
            }

            data.PreviousGroupedNodes = new SerializableDictionary<string, List<string>>(groupedNodes);
        }

        private static void UpdateUngroupedNodes(List<string> unusedUngroupedNodeNames, DialogEditorGraphSaveData data)
        {
            if (data.PreviousUngroupedNode != null && data.PreviousUngroupedNode.Count != 0)
            {
                List<string> nodesToRemove = data.PreviousUngroupedNode.Except(unusedUngroupedNodeNames).ToList();

                foreach (var node in nodesToRemove)
                {
                    DeleteAsset($"{GraphFolderPath}/Global/Dialogues", node);
                }
            }

            data.PreviousUngroupedNode = new List<string>(unusedUngroupedNodeNames);
        }

        private static void DeleteAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        private static void SaveDialogChoiceConnection()
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

        private static void SaveSONode(DialogNode node, GraphSO graphSO)
        {
            NodeSO nodeSO;
            if (node.ParentGroup != null)
            {
                nodeSO = InitAssets<NodeSO>($"{GraphFolderPath}/Groups/{node.ParentGroup.title}/Dialogues", node.DialogName);
                graphSO.Groups.AddItem(ExistingDialogGroups[node.ParentGroup.Id], nodeSO);
            }
            else
            {
                nodeSO = InitAssets<NodeSO>($"{GraphFolderPath}/Groups/UnGrouped/Dialogues", node.DialogName);
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


        private static void SaveNodeToGraph(DialogNode node, DialogEditorGraphSaveData data)
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
                GroupId = node.ParentGroup?.Id,
                Dialogtype = node.NodeType,
                Position = node.GetPosition().position,
            };

            data.Nodes.Add(nodeData);
        }

        private static void SaveGroupData(DialogEditorGraphSaveData data, GraphSO graphSO)
        {

            List<string> groupNames = new List<string>();
            // Just add all things in GraphSO and DialogEditorGraphSaveData
            foreach (DialogNodeGroup group in DialogGroups)
            {
                SaveGraphGroupData(group, data);
                SaveSOGroupData(group, graphSO);
                groupNames.Add(group.title);
            }

            UpdateUnusedGroups(groupNames, data);
        }

        private static void UpdateUnusedGroups(List<string> groupNames, DialogEditorGraphSaveData data)
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

        private static void SaveSOGroupData(DialogNodeGroup data, GraphSO graphSO)
        {
            var groupName = data.title;
            InitFolder($"{GraphFolderPath}/Groups", data.title);
            InitFolder($"{GraphFolderPath}/Groups/{groupName}", "Dialogues");

            GroupSO saveData = InitAssets<GroupSO>($"{GraphFolderPath}/Groups/{groupName}", groupName);
            saveData.Init(groupName);

            ExistingDialogGroups.Add(data.Id, saveData);

            graphSO.Groups.Add(saveData, new List<NodeSO>());
            // Need to set dirty
            SaveData(saveData);
        }

        private static void SaveData(UnityEngine.Object saveData)
        {
            EditorUtility.SetDirty(saveData);
            // Calling next functions to ensure asset will be generated or sometimes it fails 
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void SaveGraphGroupData(DialogNodeGroup group, DialogEditorGraphSaveData data)
        {
            DialogEditorGroupSaveData saveData = new DialogEditorGroupSaveData()
            {
                Id = group.Id,
                Name = group.title,
                Position = group.GetPosition().position,
            };

            data.Groups.Add(saveData);
        }

        private static T InitAssets<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";
            T asset = AssetDatabase.LoadAssetAtPath<T>(fullPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, fullPath);
            }
            return asset;
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
            InitFolder("Assets/Editor/DialogEditor", "Graphs");
            InitFolder("Assets", "DialogEditor");
            InitFolder("Assets/DialogEditor", "Dialogues");
            InitFolder("Assets/DialogSystem/Dialogues", GraphFileName);
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


