namespace Symbiote.Core.DI
{
    public interface IDependencyRegistry
    {
        void Register(IDependencyDefinition dependency);
        void Scan(IScanInstruction scanInstruction);
    }
}