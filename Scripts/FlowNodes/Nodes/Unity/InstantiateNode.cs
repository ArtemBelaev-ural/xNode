﻿using UnityEngine;
using XMonoNode;

namespace XMonoNode
{
    [CreateNodeMenu("GameObject/Instantiate", 401)]
    public class InstantiateNode : FlowNodeInOut
    {
        [Input] public GameObject Prefab;
        [Input] public Transform Parent;
        [Output] public GameObject Instance;

        public override void Flow(NodePort flowPort)
        {
            var prefab = GetInputValue(nameof(Prefab), Prefab);
            var parent = GetInputValue(nameof(Parent), Parent);
            if (parent != null)
            {
                Instance = Instantiate(prefab, parent);
            }
            else
            {
                Instance = Instantiate(prefab);
            }
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            if (port.fieldName == nameof(Instance))
            {
                return Instance;
            }
            return null; // Replace this
        }
    }
}