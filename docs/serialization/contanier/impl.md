# Сериализация-десериализация

Должна проходить в две фазы по причине наличия ссылок (которые в том числе могут быть рекурсивными)

1. Сериализация всего, кроме сслыок
2. Сериализация ссылок

Для резолвинга ссылок используются реализации `IReferenceResolver`

```csharp
public class Reference { }

public interface IReferenceResolver 
{
    public bool Supports(Reference r);
    public bool Supports(object o);

    public Reference CreateReference(object o);
    public Reference ResolveReference(Reference r);
}
```

Где `Supports` указывает какие ссылки и или объекты поддерживаются данным резолвером.

!Придумать, как резолвить ссылкки лениво и прозрачно для модели
!Посмотреть, можно ли сделать на Newtonsoft.JSON

За подгрузку ресурсов отвечает `ResourcePool`, который "кэширует" загруженные объекты и "владет ими".

API подгрузки ресурсов расположено в классе `ContainerAPI`.
