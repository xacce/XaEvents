using System.Runtime.CompilerServices;
using Trove.PolymorphicStructs;
using Unity.Entities;

namespace Xacce.XaEvents.Runtime
{
    public partial struct XaDotsGlobalContext : IComponentData
    {
    }
    //Singleton entity, all event buffers here
    public partial struct XaEventsRoot : IComponentData
    {
    }


    [PolymorphicStructInterface]
    public interface IXaDotsEvent
    {
    }

    //Extend generated structure for IBuffer
    public partial struct XaDotsEvent : IBufferElementData
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DropMainThread(XaDotsEvent evt, EntityManager manager, Entity rootEntity)
        {
            var buffer = manager.GetBuffer<XaDotsEvent>(rootEntity);
            buffer.Add(evt);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DropEcb(XaDotsEvent evt, EntityCommandBuffer ecb, Entity rootEntity)
        {
            ecb.AppendToBuffer(rootEntity, evt);
        }
    }
}