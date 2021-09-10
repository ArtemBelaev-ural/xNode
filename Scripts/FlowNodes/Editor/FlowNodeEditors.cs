using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XMonoNode;
using XMonoNodeEditor;

namespace FlowNodesEditor
{
    [CustomNodeEditor(typeof(InputFlowParameterFloat))]
    public class XFlowNodeFloatParameterEditor : NodeEditor
    {
        private InputFlowParameterFloat node = null;
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (node == null)
            {
                node = target as InputFlowParameterFloat;
                if (node == null)
                {
                    return;
                }
            }
            serializedObject.Update();

            GUILayout.BeginHorizontal();

            float newValue = GUILayout.HorizontalSlider(node.DefaultValue, 0.0f, 1.0f, GUILayout.MinHeight(10));
            UpdateValue(newValue);

            GUILayout.EndHorizontal();
        }

        private void UpdateValue(float newValue)
        {
            if (!Mathf.Approximately(newValue, node.DefaultValue))
            {
                Undo.RecordObject(node, node.Name);
                node.DefaultValue = newValue;
                FlowNodeGraph flowGraph = node.graph as FlowNodeGraph;
                if (flowGraph != null)
                {
                    flowGraph.UpdateTestParameters();
                }
                EditorUtility.SetDirty(node.gameObject);
            }
        }
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

    [CustomNodeEditor(typeof(FloatEase))]
    public class FloatEaseEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (Target.Minimized)
            {
                return;
            }

            FloatEase node = target as FloatEase;

            Texture2D tex = node.Clamped01 ? FlowNodeEditorResources.EaseTextureClamped01(node.EasingMode) : FlowNodeEditorResources.EaseTexture(node.EasingMode);
             
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.ExpandWidth(true), GUILayout.MinWidth(50));
            GUILayout.Label(new GUIContent(tex), GUILayout.MinWidth(tex.width + 2), GUILayout.Height(tex.height + 2));
            GUILayout.EndHorizontal();
        }
    }
   
    public class AnimateEaseEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (Target.Minimized)
            {
                return;
            }

            AnimateValue node = target as AnimateValue;

            node.EasingMode = (EasingMode)EditorGUILayout.EnumPopup(new GUIContent(ObjectNames.NicifyVariableName(nameof(AnimateValue.EasingMode))), node.EasingMode);

            Texture2D tex = FlowNodeEditorResources.EaseTextureClamped01(node.EasingMode);

            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.ExpandWidth(true), GUILayout.MinWidth(50));
            GUILayout.Label(new GUIContent(tex), GUILayout.MinWidth(tex.width + 2), GUILayout.Height(tex.height + 2));
            GUILayout.EndHorizontal();
        }
    }

    [CustomNodeEditor(typeof(AnimateFloatEase))]
    public class AnimateFloatEaseEditor : AnimateEaseEditor
    {
    }

    [CustomNodeEditor(typeof(AnimateVector3Ease))]
    public class AnimateVector3EaseEditor : AnimateEaseEditor
    {
    }

    [CustomNodeEditor(typeof(AnimateColorEase))]
    public class AnimateColorEaseEditor : AnimateEaseEditor
    {
    }

    public class TweenNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            if (Target.Minimized)
            {
                return;
            }

            TweenNode node = target as TweenNode;

            Texture2D tex = FlowNodeEditorResources.EaseTextureClamped01(node.easingMode);

            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.ExpandWidth(true), GUILayout.MinWidth(50));
            GUILayout.Label(new GUIContent(tex), GUILayout.MinWidth(tex.width + 2), GUILayout.Height(tex.height + 2));
            GUILayout.EndHorizontal();
        }
    }

    [CustomNodeEditor(typeof(TweenAnchoredPosition))]
    public class TweenAnchoredPositionEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenBottomLeftColorTextMeshProUGUI))]
    public class TweenBottomLeftColorTextMeshProUGUIEditor : TweenNodeEditor
    { }

    [CustomNodeEditor(typeof(TweenBottomRightColorTextMeshProUGUI))]
    public class TweenBottomRightColorTextMeshProUGUIEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenColorMaterial))]
    public class TweenColorMaterialEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenColorTextMeshProUGUI))]
    public class TweenColorTextMeshProUGUIEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenFloatMaterial))]
    public class TweenFloatMaterialEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenGradientTextMeshProUGUI))]
    public class TweenGradientTextMeshProUGUIEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenGraphicColor))]
    public class TweenGraphicColorEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenLocalPosition))]
    public class TweenLocalPositionEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenLocalPosition2D))]
    public class TweenLocalPosition2DEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenLocalRotation))]
    public class TweenLocalRotationEditor : TweenNodeEditor
    {

    }
    [CustomNodeEditor(typeof(TweenLocalRotation2D))]
    public class TweenLocalRotation2DEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenLocalScale))]
    public class TweenLocalScaleEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenLocalScale2D))]
    public class TweenLocalScale2DEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenPosition))]
    public class TweenPositionEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenPosition2D))]
    public class TweenPosition2DEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenRotation))]
    public class TweenRotationEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenRotation2D))]
    public class TweenRotation2DEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenTopLeftColorTextMeshProUGUI))]
    public class TweenTopLeftColorTextMeshProUGUIEditor : TweenNodeEditor
    {
    }
    [CustomNodeEditor(typeof(TweenTopRightColorTextMeshProUGUI))]
    public class TweenTopRightColorTextMeshProUGUIEditor : TweenNodeEditor
    {
    }

    [CustomNodeEditor(typeof(TweenVectorMaterial))]
    public class TweenVectorMaterialEditor : TweenNodeEditor
    {
    }

}
