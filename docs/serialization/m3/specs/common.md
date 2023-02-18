# Общие схемы данных

## Vector

```json
[
    x # float, опционально, по умолчанию 0,
    y # float, опционально, по умолчанию 0,
    z # float, опционально,
    w # float, опционально,
]
```

## Shape

```json
{
    "path":  # [Vector], Список точек замкнутой кривой, определяющей границы полигона
    # или
    "paths": # [[Vector]], Список замкнутых кривых, определяющих границы полигона
}
```

## Rect : Shape

```json
{
    "min": # Левый нижний угол фигуры (в зависимости от размерности вектора),
    "max": # Правый верхний угол фигуры (в зависимости от размерности вектора)
}
```

## Quaternion

```json
[
    x, # float, по умолчанию 0,
    w, # float, по умолчанию 0,
    y, # float, по умолчанию 0,
    z, # float, по умолчанию 0,
]
```

## Transform

```json
{
    "p": # Vector,
    "r": # Quaternion,
    "s": # Vector
}
```

## BezierCurve

```json
```

## Asset

Как в unity – какие-либо данные + настройки их импорта

```json
{
    "content": # url к файлу или b64-encoded файл
}
```

## Sprite : Asset

```json
{
    "content": # url к изображению, из которого будет вырезан спрайт,
    "shape": # Shape, границы спрайта, в px,
    "pivot": # 2d Vector по модулю <= √2, центральная точка спрайта, в px,
    "ppu": # float, pixels per unit, количество пикселей текстуры на единицу длины, по сути масштаб
}
```

## StatefulObject

```json
{
    # Обычные поля
    ...
    # Stateful-поля
    ...
    "state": # одно из состояний `state1`-`stateN`, по умолчанию `state1`,
    "states": [`state1`, `state2`, ..., `stateN`]
}
```

где Stateful-поля – словарь вида ```{`state`: value}```

## Item : StatefulObject

```json
{
    # обязательно
    "type": # тип предмета
    
    "name":
    {
        `state1`: # имя предмета, опционально локализованная строка,
        ...
        `stateN`: # имя предмета, опционально локализованная строка,
    }

    "sprite": 
    {
        `state1`: # url для спрайта предмета
        ...
        `stateN`: # url для спрайта предмета
    }

    ...
    # дополнительные свойства предмета
    ...

    # опциональное поле для предмета с сотояниями
    "state": # одно из состояний `state1`-`stateN`, по умолчанию `state1`
}
```