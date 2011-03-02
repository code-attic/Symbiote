namespace Symbiote.Jackalope.Config
{
    public interface IAmqpServerConfiguration
    {
        string Protocol { get; set; }
        string Address { get; set; }
        int Port { get; set; }
        string User { get; set; }
        string Password { get; set; }
        string VirtualHost { get; set; }
    }
}