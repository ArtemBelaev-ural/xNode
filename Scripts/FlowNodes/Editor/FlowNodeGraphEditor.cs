using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XMonoNode;
using XMonoNodeEditor;

namespace FlowNodesEditor
{
    /// <summary>
    /// ���� �����
    /// </summary>
    [CustomNodeGraphEditor(typeof(FlowNodeGraph))]
    public class FlowNodeGraphGraphEditor : NodeGraphEditor
    {
        private FlowNodeGraph graph = null;

        private FlowNodeGraph Graph
        {
            get
            {
                if (graph == null)
                {
                    graph = Target as FlowNodeGraph;
                }
                return graph;
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            window.titleContent = new GUIContent("Flow Node Graph", NodeEditorResources.graph);
        }

        [MenuItem("GameObject/xMonoNode/FlowNodeGraph", false, 2)]
        public static void CreateFlowNodeGraph()
        {
            GameObject current = Selection.activeGameObject;
            if (current != null)
            {
                var graph = current.AddComponent<FlowNodeGraph>();
                NodeEditorWindow.Open(graph);
            }
        }

        public override void OnToolBarGUI()
        {
            if (GUILayout.Button(new GUIContent("Flow"), EditorStyles.toolbarButton))
            {
                Graph.Flow();
            }

            if (GUILayout.Button(new GUIContent("Stop"), EditorStyles.toolbarButton))
            {
                Graph.Stop();
            }
        }

        public override float GetNoodleThickness(NodePort output, NodePort input)
        {
            float coef =  1.0f;
            if (output != null && output.ValueType == typeof(Flow) ||
                input != null && input.ValueType == typeof(Flow))
            {
                coef = 2.0f;
            }

            return NodeEditorPreferences.GetSettings().noodleThickness * coef;
        }

        public override GUIStyle GetPortStyle(NodePort port)
        {
            if (port.ValueType != typeof(Flow))
            {
                return base.GetPortStyle(port);
            }
            
            if (port.direction == NodePort.IO.Input)
                return NodeEditorResources.styles.inputPortFlow;

            return NodeEditorResources.styles.outputPortFlow;
        }

        public override string GetPortTooltip(NodePort port)
        {
            // ������� ����������� ������ ��� ������������ ���������, ����� ����� �� ���������� � �����
            Type portType = port.ValueType;
            if (portType == typeof(Flow))
            {
                return (port.direction == NodePort.IO.Input ? "Input " : "Output ") + portType.Name + ": " + port.label;
            }
            else
            {
                return base.GetPortTooltip(port);
            }
        }

    }

    /// <summary>
    /// ��������� �����. ��������� ������ execute, stop � �.�..
    /// </summary>
    [CustomEditor(typeof(FlowNodeGraph), true)]
    public class FlowNodeGraphInspector : MonoNodeInspector
    {
        private FlowNodeGraph flowNodeGraph = null;

        public override void OnInspectorGUI()
        {
            if (flowNodeGraph == null)
            {
                flowNodeGraph = target as FlowNodeGraph;
            }
            base.OnInspectorGUI();

            if (flowNodeGraph == null)
            {
                GUILayout.Label(new GUIContent("flowNodeGraph is null".Color(Color.red)));
                return;
            }

           // GUILayout.Label(new GUIContent("<color=green>=== Test ===</color>", "test float parameter of Execute()"), GUIStyle.none);

            // Start/Stop buttons

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Flow", GUILayout.Height(40)))
            {
                if (Application.isPlaying == false)
                {
                    OpenGraph();
                }
                flowNodeGraph.Flow();
            }
            if (GUILayout.Button("Stop", GUILayout.Height(40)))
            {
                flowNodeGraph.Stop();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
        }
    }
}
