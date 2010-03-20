namespace Symbiote.Hibernate
{
    public interface ISessionContext
    {
        bool Contains(string key);
        void Set(string key, object value);
        object Get(string key);
    }
}