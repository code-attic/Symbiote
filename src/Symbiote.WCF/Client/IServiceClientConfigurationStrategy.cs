namespace Symbiote.Wcf.Client
{
    public interface IServiceClientConfigurationStrategy<TContract>
    {
        void ConfigureServiceClient(IServiceConfiguration configuration);
    }
}