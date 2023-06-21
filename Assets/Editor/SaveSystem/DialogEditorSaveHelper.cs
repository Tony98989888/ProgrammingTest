using DialogEditor.Data.Save;
using DialogEditor.Save;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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

        static void Initialize(DialogGraphView graphView, string graphName)
        {
            DialogGraphView = graphView;
            GraphFileName = graphName;
            GraphFolderPath = $"Assets/DialogSaveFile/Dialogues/{graphName}";

            DialogNodes = new List<DialogNode>();
            DialogGroups = new List<DialogNodeGroup>();
        }

        public static void Save()
        {
            InitGraphFolder();
            LoadGraph();
            DialogEditorGraphSaveData data = InitAssets<DialogEditorGraphSaveData>("Assets/Editor/DialogEditor/Graphs", $"{GraphFileName}_Graph");
            data.Init(GraphFileName);
            GraphSO graphSO = new GraphSO();
            graphSO.Init(GraphFileName);

            // Save groups
            SaveGroupData(data, graphSO);
        }

        private static void SaveGroupData(DialogEditorGraphSaveData data, GraphSO graphSO)
        {
            foreach (DialogNodeGroup group in DialogGroups)
            {
                SaveGraphGroupData(group, data);
            }
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


