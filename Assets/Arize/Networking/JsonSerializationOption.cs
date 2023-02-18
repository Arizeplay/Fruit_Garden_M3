using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Ninsar.Networking
{
    public class JsonSerializationOption : ISerializationOption
    {
        public string ContentType => "application/json";

        public T Deserialize<T>(string text)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<T>(text);
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"{this} Could not parse response {text}. {e.Message}");
                return default;
            }
        }
    }
}