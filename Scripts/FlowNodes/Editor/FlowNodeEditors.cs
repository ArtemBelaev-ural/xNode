using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XMonoNode;
using XMonoNodeEditor;

namespace FlowNodesEditor
{
    [CustomNodeEditor(typeof(FlowParameterFloat))]
    public class XFlowNodeFloatParameterEditor : NodeEditor
    {
        private FlowParameterFloat node = null;
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (node == null)
            {
                node = target as FlowParameterFloat;
                if (node == null)
                {
                    return;
                }
            }
            serializedObject.Update();
            
            GUILayout.BeginHorizontal();

            GUILayout.Label(new GUIContent("Test:", "Test float parameter of Play()"), GUILayout.MaxWidth(40));
            float newValue = GUILayout.HorizontalSlider(node.TestValue, 0.0f, 1.0f);
            UpdateValue(newValue);
            newValue = EditorGUILayout.FloatField(node.TestValue, GUILayout.MaxWidth(50));
            UpdateValue(newValue);

            GUILayout.EndHorizontal();
        }

        private void UpdateValue(float newValue)
        {
            if (!Mathf.Approximately(newValue, node.TestValue))
            {
                Undo.RecordObject(node, node.Name);
                node.TestValue = newValue;
                FlowNodeGraph flowGraph = node.graph as FlowNodeGraph;
                if (flowGraph != null)
                {
                    flowGraph.UpdateTestParameters();
                }
                EditorUtility.SetDirty(node.gameObject);
            }
        }
    }

    [CustomNodeEditor(typeof(FlowParameterVector3))]
    public class FlowParameterVector3Editor : NodeEditor
    {
        private FlowParameterVector3 node = null;
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (node == null)
            {
                node = target as FlowParameterVector3;
                if (node == null)
                {
                    return;
                }
            }
            serializedObject.Update();
            EditorGUILayout.BeginHorizontal();
            Vector3 oldValue = node.TestValue;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("testValue"), new GUIContent());

            if (!Mathf.Approximately((oldValue - node.TestValue).magnitude, 0.0f))
            {
                FlowNodeGraph flowGraph = node.graph as FlowNodeGraph;
                if (flowGraph != null)
                {
                    flowGraph.UpdateTestParameters();
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    public class XSoundNodeParameterEditor<N, T> : NodeEditor where N : FlowParameter<T> 
    {
        private N node = null;
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (node == null)
            {
                node = target as N;
                if (node == null)
                {
                    return;
                }
            }
            serializedObject.Update();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Test:", "Test parameter of Play()"), GUILayout.MaxWidth(40));

            T oldValue = node.TestValue;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("testValue"), new GUIContent());
            if (node.TestValue != null && !node.TestValue.Equals(oldValue))
            {
                FlowNodeGraph flowGraph = node.graph as FlowNodeGraph;
                if (flowGraph != null)
                {
                    flowGraph.UpdateTestParameters();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    [CustomNodeEditor(typeof(FlowParameterGameObject))]
    public class FlowParameterGameObjectEditor : XSoundNodeParameterEditor<FlowParameterGameObject, GameObject> 
    { 
    }

    [CustomNodeEditor(typeof(FlowParameterTransform))]
    public class FlowParameterTransformEditor : XSoundNodeParameterEditor<FlowParameterTransform, Transform>
    { 
    }

    [CustomNodeEditor(typeof(FlowParameterInt))]
    public class FlowParameterIntEditor : XSoundNodeParameterEditor<FlowParameterInt, int>
    { 
    }

    [CustomNodeEditor(typeof(FlowParameterString))]
    public class FlowParameterStringEditor : XSoundNodeParameterEditor<FlowParameterString, string>
    {
    }

    [CustomNodeEditor(typeof(ButtonNode))]
    public class ButtonNodeEditor : NodeEditor
    {
        public ButtonNode Node => target as ButtonNode;

        public override void OnBodyGUI()
        {
            Node.FlowOutputPort.label = Node.ButtonText;
            base.OnBodyGUI();
        }
    }


}
