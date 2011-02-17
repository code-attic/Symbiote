namespace Core.Tests.Actor.KeyAccess
{
    public interface IHaveId
    {
        string Id { get; }
        void SetId( string id );
    }
}