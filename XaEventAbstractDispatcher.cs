using Unity.Collections;
using UnityEngine;
using Xacce.XaEvents.Runtime;

namespace Runtime
{
    public abstract class XaEventAbstractDispatcher : MonoBehaviour
    {
        public abstract void Dispatch(in NativeArray<XaDotsEvent>.ReadOnly events);
        public abstract void Initialize(ref NativeList<XaMonoEvent> output);
    }
}