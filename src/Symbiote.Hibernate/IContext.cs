namespace Symbiote.Hibernate
{
    public interface IContext
    {
        bool Contains(string key);
        void Set(string key, object value);
        object Get(string key);
    }
}