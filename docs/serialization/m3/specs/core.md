# Схемы данных M3

## Reward

```json
[{count: Item}] # список в формате кол-во: предмет, которые получит игрок
```

## Objective

```json
[{count: Item}] # список в формате кол-во: предмет, которые нужно получить игроку в процессе прохождения уровня
```

## Cell

```json
{
    "cords": # Vector2,
    "contains": # [Item] | Item | null, опционально, по умолчанию null,
    "gravity": Vector2, опционально, по умолчанию {"y": -1}, гравитация ячейки: cords + gravity – координаты ячейки, в которую упадет предмет
}
```

## Spawner : Cell

```json
{
    "cords": # Vector2,
    "gravity": Vector2, опционально, по умолчанию {"y": -1}, гравитация ячейки: cords + gravity – координаты ячейки, в которую упадет предмет
    "spawns": # [{probability: Item}] – список в формате вероятность спауна: предмет, где probability – BezierCurve
}
```

## Level

```json
[
    {
        "cells": # [Cell],
        "objective": # Objective,
        "reward": # Reward
        // "difficulty": # int, сложность уровня. Больше – сложнее
    }
]
```
