## Смысл

Крайне спорная, но тем не менее предоставляющая некоторую структуру для коммуникаций между Dots/Mono миром.
Я ее использую для коммуникации между Dots и UiToolkit.

Не вынесена в пакет т.к подразумевает работу с partial.

Создает в проекте единую энтити с двумя полиморфными буферами. Один содержит события из мира dots, другой содержит события из мира Mono

Так обьявляются события из мира Dots

```csharp
    [PolymorphicStruct]
    public partial struct XaDotsGameInitEvent : IXaDotsEvent
    {
        public float speed;
    }
```

Так обьявляются события из мира Mono, обрати внимание, что обработчик события находится тут же.

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

```XaDotsGlobalContext``` это условно-глобальный контекст поставляемый из мира Dots в мир Mono. Он не содержит данных по конкретному событию, а скорей является хранилищем
каких-либо часто используемых Entity, данные и проч

Так его расширять

```csharp
public partial struct XaDotsGlobalContext
{
    public Entity rc;
}
```

Стоит понимать, что заполнять этот контекст сам не будет и тебе нужно будет его чем-то заполнить

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

Далее - обработа событии на стороне Mono

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

Не забудь добавить авторинг куда-нибудь ```XaEventsRootEntity```