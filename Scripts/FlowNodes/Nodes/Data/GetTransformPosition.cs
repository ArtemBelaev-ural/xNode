﻿using UnityEngine;
using XMonoNode;

namespace XMonoNode
{
    [CreateNodeMenu("GameObject/GetTransformPosition", 417)]
    public class GetTransformPosition : MonoNode
    {
        [Input(connectionType:ConnectionType.Override)]
        public Transform _transform;
        [Output]
        public Vector3 localPosition;

        private NodePort transformPort;

        protected override void Init()
        {
            base.Init();

            transformPort = GetInputPort(nameof(_transform));
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            Transform target = transformPort.GetInputValue(_transform);
            if (target == null)
            {
                Debug.LogErrorFormat("Transform is null {0}.{1}", gameObject.name, Name);
                return null;
            }

            return target.position;
        }
    }
}
