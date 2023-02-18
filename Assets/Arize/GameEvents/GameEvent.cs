using System;
using UnityEngine;

namespace Ninsar.GameEvents
{
    public abstract class GameEvent : ScriptableObject
    {
        public abstract void Init();

        public abstract GameObject Instantiate(Transform parent);
        
        public abstract void Update();

        public abstract void Load();

        public abstract void Save();
    }
}
