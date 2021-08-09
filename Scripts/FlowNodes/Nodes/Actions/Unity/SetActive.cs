﻿using UnityEngine;
using XMonoNode;

namespace XMonoNode
{
    [CreateNodeMenu("GameObject/SetActive")]
    public class SetActive : FlowNode 
    {
        public enum ActiveOptions
        {
            Enable,
            Disable,
            Toggle,
        }

        [Input] public GameObject Target;
        public ActiveOptions Options;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }

        public override void Flow()
        {
            var target = GetInputValue<GameObject>(nameof(Target), Target);
            var isActive = Options == ActiveOptions.Enable ? true : Options == ActiveOptions.Disable ? false : !target.activeSelf;
            target.SetActive(isActive);
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }
    }
}
