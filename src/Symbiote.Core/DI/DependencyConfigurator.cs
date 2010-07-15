using System;
using System.Collections.Generic;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.DI
{
    public class DependencyConfigurator : 
        IRequestPlugin,
        IScan
    {
        public IList<IDependencyDefinition> Dependencies { get; set; }
        public IList<IScanInstruction> ScanInstructions { get; set; }

        public ISupplyPlugin<object> For(Type pluginType)
        {
            var expression = DependencyExpression.For(pluginType);
            Dependencies.Add(expression);
            return expression;
        }

        public ISupplyPlugin<TPlugin> For<TPlugin>()
        {
            var expression = DependencyExpression.For<TPlugin>();
            Dependencies.Add(expression);
            return expression;
        }

        public ISupplyPlugin<object> For(string name, Type pluginType)
        {
            var expression = DependencyExpression.For(name, pluginType);
            Dependencies.Add(expression);
            return expression;
        }

        public ISupplyPlugin<TPlugin> For<TPlugin>(string name)
        {
            var expression = DependencyExpression.For<TPlugin>(name);
            Dependencies.Add(expression);
            return expression;
        }
        
        public void Scan(Action<IScanInstruction> scanConfigurator)
        {
            var instruction = new ScanInstruction();
            scanConfigurator(instruction);
            ScanInstructions.Add(instruction);
        }

        public void RegisterDependencies(IDependencyRegistry registry)
        {
            Dependencies
                .ForEach(registry.Register);

            ScanInstructions
                .ForEach(registry.Scan);
        }

        public DependencyConfigurator()
        {
            Dependencies = new List<IDependencyDefinition>();
            ScanInstructions = new List<IScanInstruction>();
        }
    }
}