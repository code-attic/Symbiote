namespace Symbiote.Telepathy
{
    public class ServiceControlMessage
    {
        public ServiceAction Action { get; set; }

        public static ServiceControlMessage StartService()
        {
            return new ServiceControlMessage() { Action = ServiceAction.Start };
        }

        public static ServiceControlMessage StopService()
        {
            return new ServiceControlMessage() { Action = ServiceAction.Stop };    
        }

        public ServiceControlMessage()
        {
            Action = ServiceAction.NoOp;
        }
    }
}