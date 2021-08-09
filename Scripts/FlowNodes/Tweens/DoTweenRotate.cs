﻿#if DOTWEEN_SUPPORTED
using DG.Tweening;
using UnityEngine;
using XMonoNode;

namespace XMonoNode
{
    [CreateNodeMenu("Animation/Tween/Rotate")]
    public class DoTweenRotate : BaseDoTween
    {
        [Input] public GameObject Target;
        [Input] public Vector3 TargetValue;

        public override void FlowNode()
        {
            StartTween(GetInputValue(nameof(TargetValue), TargetValue));
        }

        public void StartTween(Vector3 targetValue)
        {
            if (tween == null)
            {
                var target = GetInputValue(nameof(Target), Target);
                var duration = GetInputValue(nameof(Duration), Duration);
                tween = target.transform.DORotate(targetValue, duration);
                SetupTween(tween);
            }
        }
    }
}
#endif
