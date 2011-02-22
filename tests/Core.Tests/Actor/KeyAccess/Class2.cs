namespace Core.Tests.Actor.KeyAccess
{
    public class Class2 : IHaveId
    {
        public string Id { get; protected set; }
        public void SetId(string id)
        {
            Id = id;
        }
    }
}