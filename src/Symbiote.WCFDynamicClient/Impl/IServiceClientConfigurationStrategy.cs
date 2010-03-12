namespace Symbiote.WCFDynamicClient.Impl
{
    public interface IServiceClientConfigurationStrategy<TContract>
    {
        void ConfigureServiceClient(IServiceConfiguration configuration);
    }
}