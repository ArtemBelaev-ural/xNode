using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
#endif

namespace XMonoNodeEditor {
    /// <summary> Base class to derive custom Node editors from. Use this to create your own custom inspectors and editors for your nodes. </summary>
    [CustomNodeEditor(typeof(XMonoNode.INode))]
    public class NodeEditor : XMonoNodeEditor.Internal.NodeEditorBase<NodeEditor, NodeEditor.CustomNodeEditorAttribute, XMonoNode.INode> {

        /// <summary> Fires every whenever a node was modified through the editor </summary>
        public static Action<XMonoNode.INode> onUpdateNode;
        public readonly static Dictionary<XMonoNode.NodePort, Vector2> portPositions = new Dictionary<XMonoNode.NodePort, Vector2>();

        //public XNode.INode Target
        //{
        //    get; set;
        //}
        public UnityEngine.Object target
        {
            get
            {
                return Target as UnityEngine.Object;
            }
        }

#if ODIN_INSPECTOR
        protected internal static bool inNodeEditor = false;
#endif

        public virtual void OnHeaderGUI()
        {
            GUILayout.Label(Target.Name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        /// <summary> Draws standard field editors for all public fields </summary>
        public virtual void OnBodyGUI() {
#if ODIN_INSPECTOR
            inNodeEditor = true;
#endif

            // Unity specifically requires this to save/update any serial object.
            // serializedObject.Update(); must go at the start of an inspector gui, and
            // serializedObject.ApplyModifiedProperties(); goes at the end.
            serializedObject?.Update();
            string[] excludes = { "m_Script", "graph", "position", "ports" , "_name"};

#if ODIN_INSPECTOR
            try
            {
#if ODIN_INSPECTOR_3
                objectTree.BeginDraw( true );
#else
                InspectorUtilities.BeginDrawPropertyTree(objectTree, true);
#endif
            }
            catch ( ArgumentNullException )
            {
#if ODIN_INSPECTOR_3
                objectTree.EndDraw();
#else
                InspectorUtilities.EndDrawPropertyTree(objectTree);
#endif
                NodeEditor.DestroyEditor(this.target);
                return;
            }

            GUIHelper.PushLabelWidth( 84 );
            objectTree.Draw( true );
#if ODIN_INSPECTOR_3
            objectTree.EndDraw();
#else
            InspectorUtilities.EndDrawPropertyTree(objectTree);
#endif
            GUIHelper.PopLabelWidth();
#else

            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            bool inlineStarted = false; 
            int inlineCounter = 0;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name))
                    continue;

                // Draw two fields in one row
                if (inlineStarted == false && NodeEditorUtilities.GetCachedAttrib(iterator, out XMonoNode.InlineAttribute inlineAttr))
                {
                    GUILayout.BeginHorizontal();
                    inlineStarted = true;
                    inlineCounter = 0;
                }

                NodeEditorGUILayout.PropertyField(iterator, true);

                if (inlineStarted)
                {
                    ++inlineCounter;
                    if (inlineCounter == 2)
                    {
                        GUILayout.EndHorizontal();
                        inlineStarted = false;
                    }
                }
            }
            if (inlineStarted)
            {
                GUILayout.EndHorizontal();
            }
#endif

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            foreach (XMonoNode.NodePort dynamicPort in Target.DynamicPorts)
            {
                if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;

                if (Target.ShowState == XMonoNode.INode.ShowAttribState.Minimize  && dynamicPort.ConnectionCount == 0)
                {// Пропускаем скрытые свойства
                    continue;
                }

                NodeEditorGUILayout.PortField(dynamicPort);
            }

            serializedObject?.ApplyModifiedProperties();

#if ODIN_INSPECTOR
            // Call repaint so that the graph window elements respond properly to layout changes coming from Odin
            if (GUIHelper.RepaintRequested) {
                GUIHelper.ClearRepaintRequest();
                window.Repaint();
            }
#endif

#if ODIN_INSPECTOR
            inNodeEditor = false;
#endif
        }

        public virtual int GetWidth() {
            Type type = target.GetType();
            return GetWidth(type);
        }

        static public int GetWidth(Type type)
        {
            int width;
            if (type.TryGetAttributeWidth(out width))
                return width;
            else
                return 208;
        }

        /// <summary> Returns color for target node </summary>
        public virtual Color GetTint()
        {
            // Try get color from [NodeTint] attribute
            Type type = target.GetType();
            return GetTint(type);
        }

        public static Color GetTint(Type type)
        {
            Color color;
            if (type.TryGetAttributeTint(out color))
                return color;
            // Return default color (grey)
            else
                return NodeEditorPreferences.GetSettings().tintColor;
        }

        public virtual GUIStyle GetBodyStyle() {
            return NodeEditorResources.styles.nodeBody;
        }

        public virtual GUIStyle GetBodyHighlightStyle() {
            return NodeEditorResources.styles.nodeHighlight;
        }

        /// <summary> Override to display custom node header tooltips </summary>
        public virtual string GetHeaderTooltip() {
            return null;
        }

        /// <summary> Add items for the context menu when right-clicking this node. Override to add custom menu items. </summary>
        public virtual void AddContextMenuItems(GenericMenu menu) {
            bool canRemove = true;
            // Actions if only one node is selected
            if (Selection.objects.Length == 1 && Selection.activeObject is XMonoNode.INode) {
                XMonoNode.INode node = Selection.activeObject as XMonoNode.INode;
                menu.AddItem(new GUIContent("Move To Top"), false, () => NodeEditorWindow.current.MoveNodeToTop(node));
                menu.AddItem(new GUIContent("Rename"), false, NodeEditorWindow.current.RenameSelectedNode);
                
                canRemove = NodeGraphEditor.GetEditor(node.Graph, NodeEditorWindow.current).CanRemove(node);
            }

            // Add actions to any number of selected nodes
            menu.AddItem(new GUIContent("Copy"), false, NodeEditorWindow.current.CopySelectedNodes);
            menu.AddItem(new GUIContent("Duplicate"), false, NodeEditorWindow.current.DuplicateSelectedNodes);

            if (canRemove) menu.AddItem(new GUIContent("Remove"), false, NodeEditorWindow.current.RemoveSelectedNodes);
            else menu.AddItem(new GUIContent("Remove"), false, null);

            // Custom sctions if only one node is selected
            if (Selection.objects.Length == 1 && Selection.activeObject is XMonoNode.INode) {
                XMonoNode.INode node = Selection.activeObject as XMonoNode.INode;
                menu.AddCustomContextMenuItems(node);
            }
        }

        /// <summary> Rename the node asset. This will trigger a reimport of the node. </summary>
        public void Rename(string newName) {
            if (newName == null || newName.Trim() == "") newName = NodeEditorUtilities.NodeDefaultName(target.GetType());
            target.name = newName;
            OnRename();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
        }

        /// <summary> Called after this node's name has changed. </summary>
        public virtual void OnRename() { }

        [AttributeUsage(AttributeTargets.Class)]
        public class CustomNodeEditorAttribute : Attribute,
        XMonoNodeEditor.Internal.NodeEditorBase<NodeEditor, NodeEditor.CustomNodeEditorAttribute, XMonoNode.INode>.INodeEditorAttrib {
            private Type inspectedType;
            /// <summary> Tells a NodeEditor which Node type it is an editor for </summary>
            /// <param name="inspectedType">Type that this editor can edit</param>
            public CustomNodeEditorAttribute(Type inspectedType) {
                this.inspectedType = inspectedType;
            }

            public Type GetInspectedType() {
                return inspectedType;
            }
        }
    }
}