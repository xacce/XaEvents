#if !XA_EVENTS_OFF_UNMANAGED_PART
using Unity.Burst;
using Unity.Entities;
using Xacce.XaEvents.Runtime;

namespace Runtime.Systems
{
    [BurstCompile]
    public partial struct XaEventsUnmanagedSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<XaDotsGlobalContext>();
            state.RequireForUpdate<XaEventsRoot>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var rootEntity = SystemAPI.GetSingletonEntity<XaEventsRoot>();
            var ctx = SystemAPI.GetSingleton<XaDotsGlobalContext>();
            var burstEvents = SystemAPI.GetBuffer<XaMonoEvent>(rootEntity);
            for (int i = 0; i < burstEvents.Length; i++)
            {
                var e = burstEvents[i];
                e.Handle(ctx, ref state);
            }

            burstEvents.Clear();
        }
    }
}
#endif