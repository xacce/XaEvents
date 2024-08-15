using Trove.PolymorphicStructs;
using Unity.Entities;

namespace Xacce.XaEvents.Runtime
{


    [PolymorphicStructInterface]
    public interface IXaMonoEvent
    {
        public void Handle(XaDotsGlobalContext context, ref SystemState ss);
    }


    //Extend generated structure for IBuffer
    public partial struct XaMonoEvent : IBufferElementData
    {
    }


    [PolymorphicStruct]
    public partial struct TestXaMonoEvent : IXaMonoEvent
    {
        public void Handle(XaDotsGlobalContext context, ref SystemState ss)
        {
        }
    }
}