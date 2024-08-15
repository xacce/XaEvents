## idea

Extremely controversial, but nevertheless provides some structure for communication between the Dots/Mono world. I use it for communication between Dots and UiToolkit.

It is not included in the package because it implies working with partial.

It creates a single entity in the project with two polymorphic buffers. One contains events from the dots world, the other contains events from the Mono world

This is how events from the Dots world are announced

Translated with DeepL.com (free version)

```csharp
    [PolymorphicStruct]
    public partial struct XaDotsGameInitEvent : IXaDotsEvent
    {
        public float speed;
    }
```

This is how events from the Mono world are announced, note that the event handler is right there.

```csharp
 [PolymorphicStruct]
public partial struct XaMonoChangeGameSpeedEvent : IXaMonoEvent
{
    public float speed;


    public void Handle(XaDotsGlobalContext context, ref SystemState ss)
    {
        var exists = ss.EntityManager.GetComponentData<GameRuntimeConfig>(context.rc);
        exists.speed = speed;
        ss.EntityManager.SetComponentData(context.rc, exists);
    }
}
```

```XaDotsGlobalContext``` is a conditionally global context delivered from the Dots world to the Mono world. It does not contain event-specific data, but rather is a repository of any commonly used Entity, data, and so on.
of any frequently used Entity, data, etc

Так его расширять

```csharp
public partial struct XaDotsGlobalContext
{
    public Entity rc;
}
```

It's worth realizing that it won't fill that context itself and you'll need to fill it with something

```csharp
 [BurstCompile]
    [UpdateBefore(typeof(Runtime.Systems.XaEventsUnmanagedSystem))]
    public partial struct PrepareGameContextSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameRuntimeConfig>();
            state.RequireForUpdate<XaDotsGlobalContext>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ctx = SystemAPI.GetSingletonRW<XaDotsGlobalContext>();
            ref var rw = ref ctx.ValueRW;
            rw.rc = SystemAPI.GetSingletonEntity<GameRuntimeConfig>();
        }
    }
```

Next - event processing on the Mono side

```csharp
public class GameMain : XaEventAbstractDispatcher
    {
        [SerializeField] private UIDocument mainMenu;
        [SerializeField] private UIDocument overlay;
        private NativeList<XaMonoEvent> _output;


        public override void Initialize(ref NativeList<XaMonoEvent> output)
        {
            _output = output;
           
        }

        public override void Dispatch(in NativeArray<XaDotsEvent>.ReadOnly events)
        {
            foreach (var evt in events)
            {
                switch (evt.CurrentTypeId)
                {
                    case XaDotsEvent.TypeId.XaDotsGameInitEvent:
                        GameInitEvent(evt);
                        break;
                }
            }
        }

        private void GameInitEvent(XaDotsGameInitEvent evt)
        {
           
        }
    }
```

Don't forget to add authoring somewhere ```XaEventsRootEntity```
