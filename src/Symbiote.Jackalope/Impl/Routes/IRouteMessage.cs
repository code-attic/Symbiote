namespace Symbiote.Jackalope.Impl.Routes
{
    public interface IRouteMessage
    {
        bool AllowsInheritence { get; set; }
        bool AppliesToMessage(object message);
        string GetExchange(object message);
        string GetRoutingKey(object message);
    }
}