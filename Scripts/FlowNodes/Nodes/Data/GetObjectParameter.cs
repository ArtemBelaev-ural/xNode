﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace XMonoNode
{
    public abstract class GetObjectParameter<ObjType, ParamType> : MonoNode where ObjType : Object
    {
        [Input(connectionType: ConnectionType.Override)]
        public ObjType _object;

        [Output]
        public ParamType parameter;

        public NodePort ObjectPort => objectPort;
        private NodePort objectPort;

        public NodePort ParameterPort => parameterPort;
        private NodePort parameterPort;

        protected override void Init()
        {
            base.Init();

            objectPort = GetInputPort(nameof(_object));
            parameterPort = GetOutputPort(nameof(parameter));

#if UNITY_EDITOR
            objectPort.label = UnityEditor.ObjectNames.NicifyVariableName(typeof(ObjType).PrettyName());
            parameterPort.label = UnityEditor.ObjectNames.NicifyVariableName(typeof(ParamType).PrettyName());
#endif
        }

        public override object GetValue(NodePort port)
        {
            object obj = ObjectPort.GetInputValue();
            if (obj is ObjType)
            {
                _object = obj as ObjType;
            }
            else if (obj is Component)
            {
                _object = (obj as Component).GetComponent<ObjType>();
            }

            return obj != null ? GetValue(_object) : default(ParamType);
        }

        protected abstract ParamType GetValue(ObjType obj);
    }
}