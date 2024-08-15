using Unity.Entities;
using UnityEngine;
using Xacce.XaEvents.Runtime;

namespace Runtime.Authoring
{
    public class XaEventsRootEntity : MonoBehaviour
    {
        private class UIRootEntityBaker : Baker<XaEventsRootEntity>
        {
            public override void Bake(XaEventsRootEntity authoring)
            {
                var e = GetEntity(authoring, TransformUsageFlags.None);
                AddComponent<XaEventsRoot>(e);
                AddComponent<XaDotsGlobalContext>(e);
                AddBuffer<XaMonoEvent>(e);
                AddBuffer<XaDotsEvent>(e);
            }
        }
    }
}