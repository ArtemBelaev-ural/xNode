﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMonoNode
{
    /// <summary> Base class for all node graphs </summary>
    [Serializable]
    public class MonoNodeGraph : MonoBehaviour, INodeGraph, ISerializationCallbackReceiver
    {
        /// <summary> All nodes in the graph. <para/>
        /// See: <see cref="AddNode{T}"/> </summary>
        [SerializeField, HideInInspector] public MonoNode[] nodes = new MonoNode[0];

        public int NodesCount => nodes.Length;

        public INode[] GetNodes()
        {
            var result = new INode[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                result[i] = nodes[i];
            }
            return result;
        }

        /// <summary> Add a node to the graph by type </summary>
        public T AddNode<T>() where T : class, INode
        {
            return AddNode(typeof(T)) as T;
        }

        /// <summary> Placing it last in the nodes list </summary>
        public void MoveNodeToTop(INode node)
        {
            var castedNode = node as MonoNode;
            int index;
            while ((index = Array.IndexOf(nodes, castedNode)) != NodesCount - 1)
            {
                nodes[index] = nodes[index + 1];
                nodes[index + 1] = castedNode;
            }
        }

        /// <summary> Add a node to the graph by type </summary>
        public virtual INode AddNode(Type type)
        {
            MonoNode.graphHotfix = this;
            MonoNode node = gameObject.AddComponent(type) as MonoNode;
            node.OnEnable();
            node.graph = this;
            var nodesList = new List<MonoNode>(nodes);
            nodesList.Add(node);
            nodes = nodesList.ToArray();
            return node;
        }

        /// <summary> Creates a copy of the original node in the graph </summary>
        public virtual INode CopyNode(INode original)
        {
            MonoNode castedNode = original as MonoNode;
            if(castedNode == null)
            {
                throw new ArgumentException("NodeGraph can only copy nodes scriptable objects");
            }

            MonoNode.graphHotfix = this;
            MonoNode node = gameObject.AddComponent(original.GetType()) as MonoNode;
            node.graph = this;
            node.ClearConnections();
            var nodesList = new List<MonoNode>(nodes);
            nodesList.Add(node);
            nodes = nodesList.ToArray();
            return node;
        }

        /// <summary> Safely remove a node and all its connections </summary>
        /// <param name="node"> The node to remove </param>
        public void RemoveNode(INode node)
        {
            node.ClearConnections();
            var nodesList = new List<MonoNode>(nodes);
            nodesList.Remove(node as MonoNode);
            nodes = nodesList.ToArray();
            if (Application.isPlaying) Destroy(node as UnityEngine.Object);
        }

        /// <summary> Remove all nodes and connections from the graph </summary>
        public void Clear()
        {
            if (Application.isPlaying)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    Destroy(nodes[i]);
                }
            }
            nodes = new MonoNode[0];
        }

        /// <summary> Create a new deep copy of this graph </summary>
        public XMonoNode.INodeGraph Copy()
        {
            // Instantiate a new nodegraph instance
            MonoNodeGraph graph = Instantiate(this);
            return graph;
        }

        private void OnDestroy()
        {
            // Remove all nodes prior to graph destruction
            Clear();
        }

        public void OnBeforeSerialize()
        {
            try // GetComponents() causes NullreferenceException in reset()
            {
                nodes = GetComponents<MonoNode>();
            }
            catch {}

            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].UpdatePorts();
                nodes[i].graph = this;
            }
        }

        public void OnAfterDeserialize()
        {
        }

        public System.Type getNodeType()
        {
            return typeof(MonoNode);
        }
    }
}
