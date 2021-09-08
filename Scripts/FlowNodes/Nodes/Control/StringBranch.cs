﻿using XMonoNode;

namespace XMonoNode
{
    [NodeWidth(250)]
    [CreateNodeMenu("Control/" + nameof(StringBranch), 18)]
    public class StringBranch : FlowNodeInOut
    {
        [Input] public string StringTrue;
        
        [Output] public Flow FalseOutput;
        [Input] public string StringFalse;

        private NodePort portOutFalse;

        public override void OnNodeEnable()
        {
            base.OnNodeEnable();

            portOutFalse = GetOutputPort(nameof(FalseOutput));

            // Для удобства изменим подпись к стандартным flow портам

            FlowOutputPort.label = "True";
            portOutFalse.label = "False";
        }

        public override void TriggerFlow()
        {
            var stringA = GetInputValue(nameof(StringTrue), StringTrue);
            var stringB = GetInputValue(nameof(StringFalse), StringFalse);

            FlowUtils.FlowOutput(stringA == stringB ? FlowOutputPort : portOutFalse);
        }

        public override void Flow(NodePort flowPort)
        {
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}