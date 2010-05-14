namespace Symbiote.Wcf.Server
{
    public class WcfServiceConfigurator<TContract>
        where TContract : class
    {
        protected IWcfServiceConfiguration<TContract> configuration { get; set; }

        public IWcfServiceConfiguration<TContract> GetConfiguration()
        {
            return configuration;
        }

        public WcfServiceConfigurator<TContract> UseDefaults()
        {
            configuration.UseDefaults();
            return this;
        }

        public WcfServiceConfigurator()
        {
            configuration = new WcfServiceConfiguration<TContract>();
        }
    }
}