# Схемы данных Meta

## Character: Item

## Phrase

```json
{
    "from": # Character,
    "to": # Character,
    "from-state": # Состояние персонажа в текущей фразе, опционально, по умолчанию дефолтное состояние персонажа,
    "to-state": # Состояние персонажа в текущей фразе, опционально, по умолчанию дефолтное состояние персонажа,
    "line": # опционально локализированная строка,
}
```

## Dialogue

```json
{
    "phrases": # [Phrase]
    "on": # идентификатор события, на которое запускается диалог, опционально
}
```

## SceneObject

```json
{
    "obj": # StatefulObject
    "transform": # Transform
}
```

## Scene

```json
{
    "objects": # [SceneObject]
    # ... additional data
}
```

или

```json
[SceneObject]
```

## Quest : StatefulObject

```json
{
    "price": # [{count: Item}] Цена выполнения квеста,
    "scene": # Scene,
    "dialogue": # Dialogue,
    
    "quest_objects": # Stateful-поле
    {
        `state`: {ссылка на Stateful-объект в сцене: state этого объект}
    },
    
    "level": # Level
}
```

## QuestGraph

```json
{
    Quest: [Quest]
}
```
