using DialogEditor.Data.Save;
using DialogEditor.Helper;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace DialogEditor
{
    public class DialogGraphView : GraphView
    {

        DialogEditorSearchWindow m_searchWindow;
        DialogEditorWindow m_window;

        // Nodes and groups should not have same name
        SerializableDictionary<string, DialogEditorNodeErrorData> m_unGroupedNodes;
        SerializableDictionary<Group, SerializableDictionary<string, DialogEditorNodeErrorData>> m_groupedNodes;
        SerializableDictionary<string, DialogEditorGroupErrorData> m_groups;

        int m_errorNameCount = 0;

        public int ErrorNameCount
        {
            get { return m_errorNameCount; }
            set
            {
                m_errorNameCount = value;
                if (m_errorNameCount == 0)
                {
                    // TODO Enable save funciton button
                    m_window.ActiveSaveButton(true);
                }
                else
                {
                    // TODO Disable save button
                    m_window.ActiveSaveButton(false);
                }
            }
        }

        public EditorWindow Parent => m_window;
        public DialogGraphView(DialogEditorWindow mainWindow)
        {

            m_window = mainWindow;
            m_unGroupedNodes = new SerializableDictionary<string, DialogEditorNodeErrorData>();
            m_groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DialogEditorNodeErrorData>>();
            m_groups = new SerializableDictionary<string, DialogEditorGroupErrorData>();

            InitManipulator();
            InitGridBackGround();
            InitSearchWindow();
            OnGraphElementDelete();
            OnGroupNodeAdded();
            OnGroupNodeRemoved();
            OnGroupTitleChanged();
            OnGraphViewChanged();

            // Add style to graph view for customization
            InitGraphStyleSheet();
        }

        private void InitSearchWindow()
        {
            if (m_searchWindow == null)
            {
                m_searchWindow = ScriptableObject.CreateInstance<DialogEditorSearchWindow>();
                m_searchWindow.Initialize(this);
            }
            // Happen when press space in graph view
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), m_searchWindow);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiablePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort == port) { return; }
                if (startPort.node == port.node) { return; }
                if (startPort.direction == port.direction) { return; }
                compatiablePorts.Add(port);
            });

            return compatiablePorts;
        }

        public DialogNode InitNode(DialogType nodeType, Vector2 pos)
        {
            Type nodeClassType;

            switch (nodeType)
            {
                case DialogType.Single:
                    nodeClassType = typeof(LinearDialogNode);
                    break;
                case DialogType.Multiple:
                    nodeClassType = typeof(MultiDialogNode);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null);
            }
            var node = (DialogNode)Activator.CreateInstance(nodeClassType);
            node.Init(this, pos);
            node.InitNodeUI();

            AddUngroupedNode(node);
            AddElement(node);
            return node;
        }

        public void AddUngroupedNode(DialogNode node)
        {
            string nodeName = node.DialogName.ToLower();
            if (!m_unGroupedNodes.ContainsKey(nodeName))
            {
                DialogEditorNodeErrorData data = new DialogEditorNodeErrorData();
                data.DialogNodes.Add(node);
                m_unGroupedNodes.Add(nodeName, data);
                return;
            }
            else
            {
                m_unGroupedNodes[nodeName].DialogNodes.Add(node);
                ErrorNameCount++;
                //Have duplicated nodes means error
                foreach (var item in m_unGroupedNodes[nodeName].DialogNodes)
                {
                    var errorColor = m_unGroupedNodes[nodeName].ErrorData.ErrorColor;
                    item.UpdateNodeColor(DialogNode.NodeStyle.Error, errorColor);
                }
            }
        }

        public void RemoveUngroupedNode(DialogNode node)
        {
            string nodeName = node.DialogName.ToLower();
            if (m_unGroupedNodes.TryGetValue(nodeName, out var data))
            {
                node.UpdateNodeColor(DialogNode.NodeStyle.Normal, DialogEditorStyleSheetHelper.DefaultNodeBGColor);
                data.DialogNodes.Remove(node);
                ErrorNameCount--;
                switch (data.DialogNodes.Count)
                {
                    case 0:
                        m_unGroupedNodes.Remove(nodeName);
                        break;
                    case 1:
                        data.DialogNodes[0].UpdateNodeColor(DialogNode.NodeStyle.Normal, DialogEditorStyleSheetHelper.DefaultNodeBGColor);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Debug.LogError("Removing Node Do Not Exist!");
            }
        }

        private void OnGroupNodeAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (var element in elements)
                {
                    if (element is DialogNode)
                    {
                        RemoveUngroupedNode((DialogNode)element);
                        AddGroupedNode((DialogNode)element, (DialogNodeGroup)group);
                    }
                }
            };
        }

        private void OnGroupNodeRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (var element in elements)
                {
                    if (element is DialogNode)
                    {
                        // TODO Need to remove node from group
                        RemoveGroupedNode((DialogNode)element, group);
                        AddUngroupedNode((DialogNode)element);
                    }
                }
            };
        }

        void OnGraphViewChanged() 
        {
            graphViewChanged = (change) => 
            {
                if (change.edgesToCreate != null)
                {
                    foreach (var edge in change.edgesToCreate)
                    {
                        var nextNode = (DialogNode)edge.input.node;
                        var choiceData = (DialogEditorChoiceSaveData)edge.output.userData;
                        choiceData.ChoiceId = nextNode.Id;

                    }
                }

                if (change.elementsToRemove != null)
                {
                    var edgeType = typeof(Edge);
                    // Remove edges in remove elements
                    foreach (GraphElement element in change.elementsToRemove)
                    {
                        if (element.GetType() != edgeType)
                        {
                            continue;
                        }

                        Edge edge = (Edge)element;
                        var choiceData = (DialogEditorChoiceSaveData)edge.output.userData;
                        choiceData.ChoiceId = string.Empty;
                    }
                }

                return change;
            };
        }

        public void AddGroupedNode(DialogNode node, DialogNodeGroup group)
        {
            string name = node.DialogName.ToLower();
            node.ParentGroup = group;
            if (m_groupedNodes.TryGetValue(group, out var data))
            {
                if (data.TryGetValue(name, out var errorData))
                {
                    errorData.DialogNodes.Add(node);
                    Color errorColor = errorData.ErrorData.ErrorColor;
                    if (errorData.DialogNodes.Count > 1)
                    {
                        foreach (var item in errorData.DialogNodes)
                        {
                            item.UpdateNodeColor(DialogNode.NodeStyle.Error, errorColor);
                        }
                    }
                }
                else
                {
                    // Group exist add new node
                    DialogEditorNodeErrorData _errorData = new DialogEditorNodeErrorData();
                    _errorData.DialogNodes.Add(node);
                    ErrorNameCount++;
                    m_groupedNodes[group].Add(name, _errorData);
                }
            }
            else
            {
                // Group do not exist add new node in it
                m_groupedNodes.Add(group, new SerializableDictionary<string, DialogEditorNodeErrorData>());
                DialogEditorNodeErrorData _errorData = new DialogEditorNodeErrorData();
                _errorData.DialogNodes.Add(node);
                m_groupedNodes[group].Add(name, _errorData);
            }
        }

        public void RemoveGroupedNode(DialogNode node, Group group)
        {
            string name = node.DialogName.ToLower();
            node.ParentGroup = null;
            if (m_groupedNodes.TryGetValue(group, out var data))
            {
                if (data.TryGetValue(name, out var errorData))
                {
                    // node.UpdateNodeColor(DialogNode.NodeStyle.Normal, DialogEditorStyleSheetHelper.DefaultNodeBGColor);
                    errorData.DialogNodes.Remove(node);
                    ErrorNameCount--;
                    switch (errorData.DialogNodes.Count)
                    {
                        case 0:
                            m_groupedNodes[group].Remove(name);
                            if (m_groupedNodes[group].Count == 0)
                            {
                                m_groupedNodes.Remove(group);
                            }
                            break;
                        case 1:
                            foreach (var item in errorData.DialogNodes)
                            {
                                item.UpdateNodeColor(DialogNode.NodeStyle.Normal, DialogEditorStyleSheetHelper.DefaultNodeBGColor);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void InitManipulator()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(InitMenuManipulator("Linear Choice Node", DialogType.Single));
            this.AddManipulator(InitMenuManipulator("Multi Choice Node", DialogType.Multiple));

            this.AddManipulator(InitGroupContextualMenu());
        }

        private IManipulator InitGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => InitGroup("Dialog Node Group", viewTransform.matrix.inverse.MultiplyPoint(actionEvent.eventInfo.localMousePosition))));
            return contextualMenuManipulator;
        }

        public DialogNodeGroup InitGroup(string title, Vector2 localMousePosition)
        {
            DialogNodeGroup group = new DialogNodeGroup(title, localMousePosition);
            AddGroup(group);
            AddElement(group);
            // When create group with nodes selected, should add selected nodes to group automatically
            foreach (GraphElement element in selection)
            {
                if (element is DialogNode node)
                {
                    group.AddElement(node);
                }
            }
            return group;
        }

        private void AddGroup(DialogNodeGroup group)
        {
            var name = group.title.ToLower();
            if (!m_groups.TryGetValue(name, out var data))
            {
                DialogEditorGroupErrorData errorData = new DialogEditorGroupErrorData();
                errorData.Groups.Add(group);
                m_groups.Add(name, errorData);
                return;
            }
            else
            {
                data.Groups.Add(group);
                Color errorColor = data.ErrorData.ErrorColor;
                foreach (var item in data.Groups)
                {
                    item.UpdateGroupColor(DialogNodeGroup.GroupStyle.Error, errorColor);
                }
            }
        }

        private void InitGraphStyleSheet()
        {
            this.ApplyStyleSheet("Assets/DialogEditorResource/DialogEditorNodeStyle.uss",
                "Assets/DialogEditorResource/DialogEditorGraphViewStyleSheet.uss");
        }

        /*
         * First need to init grid to add element
         */
        private void InitGridBackGround()
        {
            GridBackground backGround = new GridBackground();
            backGround.StretchToParentSize();
            Insert(0, backGround);
        }

        private void OnGroupTitleChanged()
        {
            groupTitleChanged = (group, title) =>
            {
                DialogNodeGroup _group = (DialogNodeGroup)group;
                _group.title = DialogEditorStringHelper.FormatText(group.title);

                if (string.IsNullOrEmpty(_group.title))
                {
                    if (!string.IsNullOrEmpty(_group.PreviousTitle))
                    {
                        ++ErrorNameCount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(_group.PreviousTitle))
                    {
                        --ErrorNameCount;
                    }
                }

                DeleteGroup(_group);
                _group.PreviousTitle = _group.title;
                AddGroup(_group);
            };
        }

        // In this way we can use right click menu to add new node
        private IManipulator InitMenuManipulator(string actionName, DialogType dialogType)
        {
            // Need to change mouse position to correct position
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(menuEvent => menuEvent.menu.AppendAction(actionName, actionEvent => AddElement(InitNode(dialogType, viewTransform.matrix.inverse.MultiplyPoint(actionEvent.eventInfo.localMousePosition)))));
            return contextualMenuManipulator;
        }

        private void OnGraphElementDelete()
        {
            Type groupType = typeof(DialogNodeGroup);
            List<DialogNode> readyDeleteNodes = new List<DialogNode>();
            List<DialogNodeGroup> readyDeleteGroup = new List<DialogNodeGroup>();
            List<Edge> readyDeleteEdges = new List<Edge>();
            Action<List<ISelectable>> typeCheck = (List<ISelectable> element) =>
            {
                foreach (var item in element)
                {
                    switch (item)
                    {
                        case DialogNode node:
                            readyDeleteNodes.Add(node);
                            break;
                        case Group group:
                            var _group = (DialogNodeGroup)group;
                            DeleteGroup(_group);
                            readyDeleteGroup.Add(_group);
                            break;
                        case Edge edge:
                            readyDeleteEdges.Add(edge);
                            break;
                        default:
                            break;
                    }
                }
            };

            deleteSelection = (operationName, askUser) =>
            {
                typeCheck(selection);

                DeleteElements(readyDeleteEdges);

                foreach (DialogNodeGroup item in readyDeleteGroup)
                {
                    List<DialogNode> nodes = new List<DialogNode>();
                    foreach (GraphElement element in item.containedElements)
                    {
                        if (element is DialogNode node)
                        {
                            nodes.Add(node);
                        }
                    }

                    // Need to remove all nodes when deleteing a group
                    item.RemoveElements(nodes);
                    RemoveElement(item);
                }

                foreach (var item in readyDeleteNodes)
                {
                    if (item.ParentGroup != null)
                    {
                        item.ParentGroup.RemoveElement(item);
                    }
                    RemoveUngroupedNode(item);
                    item.ClearAllPorts();
                    RemoveElement(item);
                }
            };
        }

        private void DeleteGroup(DialogNodeGroup group)
        {
            string name = group.PreviousTitle.ToLower();
            if (m_groups.TryGetValue(name, out var data))
            {
                group.UpdateGroupColor(DialogNodeGroup.GroupStyle.Normal, DialogEditorStyleSheetHelper.DefaultNodeBGColor);
                data.Groups.Remove(group);
                switch (data.Groups.Count)
                {
                    case 0:
                        break;
                    case 1:
                        data.Groups[0].UpdateGroupColor(DialogNodeGroup.GroupStyle.Normal, DialogEditorStyleSheetHelper.DefaultNodeBGColor);
                        break;
                }
            }
        }
    }
}