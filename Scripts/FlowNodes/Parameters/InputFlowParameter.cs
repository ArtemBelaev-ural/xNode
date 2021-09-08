using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMonoNode;

namespace XMonoNode
{
    public abstract class InputFlowParameter : MonoNode
    {
        /// <summary>
        /// �������� ���������
        /// </summary>
        public abstract object GetDefaultValue();
    }

    /// <summary>
    /// ���������� ��������, ���������� � ����� FlowNodeGraph.Flow()
    /// </summary>
    [NodeTint(50, 70, 105)]
    [NodeWidth(200)]
    public abstract class InputFlowParameter<T> : InputFlowParameter
    {
        [Output(backingValue: ShowBackingValue.Always)] public T   output;

        

        /// <summary>
        /// ��������, ������������ �� ���������
        /// </summary>
        public T DefaultValue
        {
            get => output;
            set => output = value;
        }

        private void Reset()
        {
            Name = "Input Param: " + NodeUtilities.PrettyName(typeof(T));
        }

        public override object GetValue(NodePort port)
        {
            FlowNodeGraph flowGraph = graph as FlowNodeGraph;
            if (flowGraph != null)
            {
                if (flowGraph.OutputFlowParametersDict.TryGetValue(Name, out object value))
                {
                    return value;
                }
                else
                {
                    
                    value = flowGraph.FlowParametersArray.Get<T>();
                    return !Equals(value, default(T)) ? value : output;
                }
            }
  
            return output;
        }

        public override object GetDefaultValue()
        {
            return output;
        }
    }
}