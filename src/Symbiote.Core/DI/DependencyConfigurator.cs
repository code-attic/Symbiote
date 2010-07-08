using System;
using System.Collections.Generic;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.DI
{
    public class DependencyConfigurator : 
        IRequestPlugin,
        IScan
    {
        public IList<DependencyExpression> Dependencies { get; set; }
        public IList<IScanInstruction> ScanInstructions { get; set; }

        public ISupplyPlugin For(Type pluginType)
        {
            var expression = new DependencyExpression();
            Dependencies.Add(expression);
            return expression.For(pluginType);
        }

        public ISupplyPlugin For<TPlugin>()
        {
            var expression = new DependencyExpression();
            Dependencies.Add(expression);
            return expression.For<TPlugin>();
        }

        public ISupplyPlugin For(string name, Type pluginType)
        {
            var expression = new DependencyExpression();
            Dependencies.Add(expression);
            return expression.For(name, pluginType);
        }

        public ISupplyPlugin For<TPlugin>(string name)
        {
            var expression = new DependencyExpression();
            Dependencies.Add(expression);
            return expression.For<TPlugin>(name);
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
            Dependencies = new List<DependencyExpression>();
            ScanInstructions = new List<IScanInstruction>();
        }
    }
}