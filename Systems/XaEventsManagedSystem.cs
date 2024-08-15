#if !XA_EVENTS_OFF_MANAGED_PART
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Xacce.XaEvents.Runtime;

namespace Runtime.Systems
{
    public sealed partial class XaEventsManagedSystem : SystemBase
    {
        private NativeList<XaMonoEvent> _monoEvents;
        private XaEventAbstractDispatcher _dispatcher;

        protected override void OnCreate()
        {
            _monoEvents = new NativeList<XaMonoEvent>(Allocator.Persistent);
            RequireForUpdate<XaEventsRoot>();
        }

        protected override void OnStartRunning()
        {
            var gameObject = GameObject.FindObjectOfType<XaEventAbstractDispatcher>();
            if (gameObject != null)
            {
                _dispatcher = gameObject;
                _dispatcher.Initialize(ref _monoEvents);
                Debug.Log("Ui mono dispatcher was ready");
            }
            else
            {
                Debug.LogError(
                    $"U have a authored UIBurstBridgeSingleton, link to gameobject with IMonoUIEventDispatcher invalid, check entity with UIBurstBridgeSingleton component ");
            }
        }


        protected override void OnDestroy()
        {
            if (_monoEvents.IsCreated) _monoEvents.Dispose();
        }


        protected override void OnUpdate()
        {
            if (_dispatcher == null) return;
            var rootEntity = SystemAPI.GetSingletonEntity<XaEventsRoot>();
            var burstEvents = SystemAPI.GetBuffer<XaDotsEvent>(rootEntity);
            var monoBuffer = SystemAPI.GetBuffer<XaMonoEvent>(rootEntity);
            if (!_monoEvents.IsEmpty)
            {
                Debug.Log($"Drop: {_monoEvents.Length} events from mono");
                monoBuffer.CopyFrom(_monoEvents);
                _monoEvents.Clear();
            }

            if (burstEvents.IsEmpty) return;
            Debug.Log($"Dispatching {burstEvents.Length} events from dots");
            _dispatcher.Dispatch(burstEvents.AsNativeArray().AsReadOnly());
            burstEvents.Clear();
        }
    }
}
#endif