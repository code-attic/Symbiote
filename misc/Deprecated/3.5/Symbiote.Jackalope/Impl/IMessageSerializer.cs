namespace Symbiote.Jackalope.Impl
{
    public interface IMessageSerializer
    {
        T Deserialize<T>(byte[] message)
            where T : class;

        object Deserialize(byte[] message);

        byte[] Serialize<T>(T body)
            where T : class;
    }
}
