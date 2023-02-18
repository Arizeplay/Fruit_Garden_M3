namespace Ninsar.Networking
{
    public interface ISerializationOption
    {
        string ContentType { get; }
        public T Deserialize<T>(string text);
    }
}