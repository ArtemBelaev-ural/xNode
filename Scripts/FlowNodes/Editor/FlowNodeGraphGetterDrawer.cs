using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XMonoNode;

namespace FlowNodesEditor
{
    [CustomPropertyDrawer(typeof(FlowNodeGraphGetter), true)]
    public class FlowNodeGraphGetterDrawer : PropertyDrawer
    {
        private FlowNodeGraphContainer      container = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property == null)
            {
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int pinButtonWidth = 20;
            int playButtonWidth = 30;
            position.width -= pinButtonWidth + playButtonWidth;

            // Draw path to containers
            string pathToContainers = property.FindPropertyRelative("pathToContainers").stringValue;
            bool drawPathToContainers = property.FindPropertyRelative("drawPathToContainers").boolValue;

            if (drawPathToContainers)
            {
                position.width /= 3; // ����� ������: ���� � �����������, ���������, Id
                //Rect pathRect = new Rect(position.x, position.y, position.width, position.height);
                pathToContainers = EditorGUI.TextField(position, pathToContainers);
                property.FindPropertyRelative("pathToContainers").stringValue = pathToContainers;
                position.x += position.width;
            }
            else
            {
                position.width /= 2;
            }

            // Draw containers popup
            string[] containersNames = GetContainersNames(pathToContainers);
            
            string containerFileName = property.FindPropertyRelative("containerFileName").stringValue;
            int currentContainerIndex = System.Array.IndexOf(containersNames, containerFileName);
            currentContainerIndex = EditorGUI.Popup(position, currentContainerIndex, containersNames);
            if (currentContainerIndex < 0)
            {
                currentContainerIndex = 0;
            }
            containerFileName = containersNames[currentContainerIndex];
            property.FindPropertyRelative("containerFileName").stringValue = containerFileName;
            position.x += position.width;

            // Draw graphId popup
            string[] graphIds = GetGraphIds(pathToContainers + containerFileName, out FlowNodeGraphContainer container);
            string id = property.FindPropertyRelative("graphId").stringValue;
            int index = System.Array.IndexOf(graphIds, id);
            index = EditorGUI.Popup(position, index, graphIds);
            if (index < 0)
            {
                index = 0;
            }
            id = graphIds[index];
            property.FindPropertyRelative("graphId").stringValue = id;

            if (container != null)
            {
                position.x += position.width;
                position.width = pinButtonWidth;
                if (GUI.Button(position, new GUIContent(">", "Play")))
                {
                    EditorGUIUtility.PingObject(container.GetPrefab(id));
                    container.Execute(id);
                }

                position.x += position.width;
                position.width = playButtonWidth;
                if (GUI.Button(position, new GUIContent("=>", "Open")))
                {
                    FlowNodeGraph graph = container.GetPrefab(id) as FlowNodeGraph;
                    Selection.SetActiveObjectWithContext(graph, graph);
                    XMonoNodeEditor.NodeEditorWindow.Open(graph);
                    EditorGUIUtility.PingObject(container.GetPrefab(id));

                }
            }

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        private string[] GetGraphIds(string containerFullName, out FlowNodeGraphContainer container)
        {
            container = Resources.Load<FlowNodeGraphContainer>(containerFullName);

            if (container == null)
            {
                Debug.LogError("Container is null! " + containerFullName);
                return new string[1];
            }

            string[] graphIds = new string[container.ItemsList.Count + 1];

            graphIds[0] = "NULL";
            for (int i = 0; i < container.ItemsList.Count; ++i)
            {
                graphIds[i + 1] = container.ItemsList[i].Id;
            }

            return graphIds;
        }

        string[] GetContainersNames(string pathToContainers)
        {
            FlowNodeGraphContainer[] containers = Resources.LoadAll<FlowNodeGraphContainer>(pathToContainers);
            string[] containersNames = new string[containers.Length + 1];
            containersNames[0] = FlowNodeGraphGetter.NO_CONTAINER;
            for (int i = 0; i < containers.Length; ++i)
            {
                containersNames[i + 1] = containers[i].name;
            }
            return containersNames;
        }
    }

}