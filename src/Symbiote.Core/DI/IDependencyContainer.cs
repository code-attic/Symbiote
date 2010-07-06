using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.DI
{
    public interface IDependencyContainer : IServiceLocator
    {

    }

    public interface IDependencyRegistry
    {
        void Register(IDependencyDefinition dependency);
        void Scan(IScanInstruction scanInstruction);
    }

    public interface IDependencyAdapter :
        IDependencyContainer,
        IDependencyRegistry
    {
        
    }

    public interface IDependencyDefinition
    {
        object ConcreteInstance { get; set; }
        Type ConcreteType { get; set; }
        Type FactoryType { get; set; }
        bool HasFactory { get; set; }
        bool IsAdd { get; set; }
        bool IsNamed { get; set; }
        bool IsSingleton { get; set; }
        string PluginName { get; set; }
        Type PluginType { get; set; }
        bool HasSingleton { get; set; }
    }

    public interface IScan
    {
        void Scan(Action<IScanInstruction> scanInstruction);
    }

    public interface IScanInstruction
    {
        void Assembly(Assembly assembly);
        void Assembly(string assemblyName);
        void TheCallingAssembly();
        void AssemblyContainingType<T>();
        void AssemblyContainingType(Type type);
        void AssembliesFromPath(string path);
        void AssembliesFromPath(string path, Predicate<Assembly> assemblyFilter);
        void AssembliesFromApplicationBaseDirectory();
        void AssembliesFromApplicationBaseDirectory(Predicate<Assembly> assemblyFilter);
        void LookForRegistries();
        FindAllTypesFilter AddAllTypesOf<PLUGINTYPE>();
        FindAllTypesFilter AddAllTypesOf(Type pluginType);
        void IgnoreStructureMapAttributes();
        void Exclude(Func<Type, bool> exclude);
        void ExcludeNamespace(string nameSpace);
        void ExcludeNamespaceContainingType<T>();
        void Include(Func<Type, bool> predicate);
        void IncludeNamespace(string nameSpace);
        void IncludeNamespaceContainingType<T>();
        void ExcludeType<T>();
        void Convention<T>() where T : IRegistrationConvention, new();
        void ModifyGraphAfterScan(Action<PluginGraph> modifyGraph);
        ConfigureConventionExpression WithDefaultConventions();
        ConfigureConventionExpression ConnectImplementationsToTypesClosing(Type openGenericType);
        ConfigureConventionExpression RegisterConcreteTypesAgainstTheFirstInterface();
        ConfigureConventionExpression SingleImplementationsOfInterface();
    }

    public interface IScanDetail
    {
        
    }

    public class AssemblyScanner : IScanInstruction
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly List<IRegistrationConvention> _conventions = new List<IRegistrationConvention>();
        private readonly CompositeFilter<Type> _filter = new CompositeFilter<Type>();

        private readonly List<Action<PluginGraph>> _postScanningActions = new List<Action<PluginGraph>>();
        private readonly List<ITypeScanner> _scanners = new List<ITypeScanner>();

        public AssemblyScanner()
        {
            Convention<FamilyAttributeScanner>();
            Convention<PluggableAttributeScanner>();
        }

        public int Count { get { return _assemblies.Count; } }


        public void Assembly(Assembly assembly)
        {
            if (!_assemblies.Contains(assembly))
            {
                _assemblies.Add(assembly);
            }
        }

        public void Assembly(string assemblyName)
        {
            Assembly(AppDomain.CurrentDomain.Load(assemblyName));
        }

        public void Convention<T>() where T : IRegistrationConvention, new()
        {
            IRegistrationConvention previous = _conventions.FirstOrDefault(scanner => scanner is T);
            if (previous == null)
            {
                With(new T());
            }
        }

        public void LookForRegistries()
        {
            Convention<FindRegistriesScanner>();
        }

        public void TheCallingAssembly()
        {
            Assembly callingAssembly = findTheCallingAssembly();

            if (callingAssembly != null)
            {
                _assemblies.Add(callingAssembly);
            }
        }

        public void AssemblyContainingType<T>()
        {
            _assemblies.Add(typeof(T).Assembly);
        }

        public void AssemblyContainingType(Type type)
        {
            _assemblies.Add(type.Assembly);
        }

        public FindAllTypesFilter AddAllTypesOf<PLUGINTYPE>()
        {
            return AddAllTypesOf(typeof(PLUGINTYPE));
        }

        public FindAllTypesFilter AddAllTypesOf(Type pluginType)
        {
            var filter = new FindAllTypesFilter(pluginType);
            With(filter);

            return filter;
        }

        public void IgnoreStructureMapAttributes()
        {
            _conventions.RemoveAll(scanner => scanner is FamilyAttributeScanner);
            _conventions.RemoveAll(scanner => scanner is PluggableAttributeScanner);
        }


        public void Exclude(Func<Type, bool> exclude)
        {
            _filter.Excludes += exclude;
        }

        public void ExcludeNamespace(string nameSpace)
        {
            Exclude(type => type.IsInNamespace(nameSpace));
        }

        public void ExcludeNamespaceContainingType<T>()
        {
            ExcludeNamespace(typeof(T).Namespace);
        }

        public void Include(Func<Type, bool> predicate)
        {
            _filter.Includes += predicate;
        }

        public void IncludeNamespace(string nameSpace)
        {
            Include(type => type.IsInNamespace(nameSpace));
        }

        public void IncludeNamespaceContainingType<T>()
        {
            IncludeNamespace(typeof(T).Namespace);
        }

        public void ExcludeType<T>()
        {
            Exclude(type => type == typeof(T));
        }

        public void ModifyGraphAfterScan(Action<PluginGraph> modifyGraph)
        {
            _postScanningActions.Add(modifyGraph);
        }

        public void AssembliesFromApplicationBaseDirectory()
        {
            AssembliesFromApplicationBaseDirectory(a => true);
        }

        public void AssembliesFromApplicationBaseDirectory(Predicate<Assembly> assemblyFilter)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            AssembliesFromPath(baseDirectory, assemblyFilter);
            string binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            if (Directory.Exists(binPath))
            {
                AssembliesFromPath(binPath, assemblyFilter);
            }
        }

        public void AssembliesFromPath(string path)
        {
            AssembliesFromPath(path, a => true);
        }

        public void AssembliesFromPath(string path, Predicate<Assembly> assemblyFilter)
        {
            IEnumerable<string> assemblyPaths = Directory.GetFiles(path)
                .Where(file =>
                       Path.GetExtension(file).Equals(
                           ".exe",
                           StringComparison.OrdinalIgnoreCase)
                       ||
                       Path.GetExtension(file).Equals(
                           ".dll",
                           StringComparison.OrdinalIgnoreCase));

            foreach (string assemblyPath in assemblyPaths)
            {
                Assembly assembly = null;
                try
                {
                    assembly = System.Reflection.Assembly.LoadFrom(assemblyPath);
                }
                catch
                {
                }
                if (assembly != null && assemblyFilter(assembly)) Assembly(assembly);
            }
        }

        public void With(IRegistrationConvention convention)
        {
            _conventions.Fill(convention);
        }

        internal void ScanForAll(PluginGraph pluginGraph)
        {
            var registry = new Registry();

            pluginGraph.Types.For(_assemblies, _filter).Each(
                type =>
                {
                    _scanners.Each(x => x.Process(type, pluginGraph));
                    _conventions.Each(c => c.Process(type, registry));
                });

            registry.ConfigurePluginGraph(pluginGraph);
            _postScanningActions.Each(x => x(pluginGraph));
        }


        public bool Contains(string assemblyName)
        {
            foreach (Assembly assembly in _assemblies)
            {
                if (assembly.GetName().Name == assemblyName)
                {
                    return true;
                }
            }

            return false;
        }

        private static Assembly findTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            Assembly thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Assembly callingAssembly = null;
            for (int i = 0; i < trace.FrameCount; i++)
            {
                StackFrame frame = trace.GetFrame(i);
                Assembly assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != thisAssembly)
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }



        /// <summary>
        /// Adds the DefaultConventionScanner to the scanning operations.  I.e., a concrete
        /// class named "Something" that implements "ISomething" will be automatically 
        /// added to PluginType "ISomething"
        /// </summary>
        public ConfigureConventionExpression WithDefaultConventions()
        {
            var convention = new DefaultConventionScanner();
            With(convention);
            return new ConfigureConventionExpression(convention);
        }

        /// <summary>
        /// Scans for PluginType's and Concrete Types that close the given open generic type
        /// </summary>
        /// <example>
        /// 
        /// </example>
        /// <param name="openGenericType"></param>
        public ConfigureConventionExpression ConnectImplementationsToTypesClosing(Type openGenericType)
        {
            var convention = new GenericConnectionScanner(openGenericType);
            With(convention);
            return new ConfigureConventionExpression(convention);
        }

        /// <summary>
        /// Automatically registers all concrete types without primitive arguments
        /// against its first interface, if any
        /// </summary>
        public ConfigureConventionExpression RegisterConcreteTypesAgainstTheFirstInterface()
        {
            var convention = new FirstInterfaceConvention();
            With(convention);
            return new ConfigureConventionExpression(convention);
        }

        /// <summary>
        /// Directs the scanning to automatically register any type that is the single
        /// implementation of an interface against that interface.
        /// The filters apply
        /// </summary>
        public ConfigureConventionExpression SingleImplementationsOfInterface()
        {
            var convention = new ImplementationMap();
            With(convention);
            ModifyGraphAfterScan(convention.RegisterSingleImplementations);
            return new ConfigureConventionExpression(convention);
        }

    }

    public class ScanInstruction : 
        IScanInstruction,
        IScanDetail
    {
        
    }

    public interface IRequestPlugin
    {
        ISupplyPlugin For(Type pluginType);
        ISupplyPlugin For<TPlugin>();
        ISupplyPlugin For(string name, Type pluginType);
        ISupplyPlugin For<TPlugin>(string name);
    }

    public interface ISupplyPlugin
    {
        IPluginConfiguration Add(Type concreteType);
        IPluginConfiguration Add<TConcrete>();
        IPluginConfiguration Add<TConcrete>(TConcrete instance);
        IPluginConfiguration Use(Type concreteType);
        IPluginConfiguration Use<TConcrete>();
        IPluginConfiguration Use<TConcrete>(TConcrete instance);
        IPluginConfiguration UseFactory<TFactory>();
    }

    public interface IPluginConfiguration
    {
        IPluginConfiguration AsSingleton();
    }

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
                .ForEach(x => registry.Register(x));

            ScanInstructions
                .ForEach(x => registry.Scan(x));
        }

        public DependencyConfigurator()
        {
            Dependencies = new List<DependencyExpression>();
            ScanInstructions = new List<IScanInstruction>();
        }
    }

    public class DependencyExpression : 
        IDependencyDefinition,
        ISupplyPlugin,
        IRequestPlugin,
        IPluginConfiguration
    {
        public object ConcreteInstance { get; set; }
        public Type ConcreteType { get; set; }
        public Type FactoryType { get; set; }
        public bool HasFactory { get; set; }
        public bool IsAdd { get; set; }
        public bool IsNamed { get; set; }
        public bool IsSingleton { get; set; }
        public bool HasSingleton { get; set; }
        public string PluginName { get; set; }
        public Type PluginType { get; set; }

        public virtual IPluginConfiguration Add(Type concreteType)
        {
            ConcreteType = concreteType;
            IsAdd = true;
            return this;
        }

        public virtual IPluginConfiguration Add<TConcrete>()
        {
            ConcreteType = typeof(TConcrete);
            IsAdd = true;
            return this;
        }

        public IPluginConfiguration Add<TConcrete>(TConcrete instance)
        {
            ConcreteInstance = instance;
            IsAdd = true;
            return this;
        }

        public virtual ISupplyPlugin For(Type pluginType)
        {
            PluginType = pluginType;
            return this;
        }

        public virtual ISupplyPlugin For<TPlugin>()
        {
            PluginType = typeof (TPlugin);
            return this;
        }

        public virtual ISupplyPlugin For(string name, Type pluginType)
        {
            PluginName = name;
            IsNamed = true;
            PluginType = pluginType;
            return this;
        }

        public virtual ISupplyPlugin For<TPlugin>(string name)
        {
            PluginName = name;
            IsNamed = true;
            PluginType = typeof(TPlugin);
            return this;
        }

        public virtual IPluginConfiguration Use(Type concreteType)
        {
            ConcreteType = concreteType;
            return this;
        }

        public virtual IPluginConfiguration Use<TConcrete>()
        {
            ConcreteType = typeof (TConcrete);
            return this;
        }

        public virtual IPluginConfiguration Use<TConcrete>(TConcrete instance)
        {
            ConcreteInstance = instance;
            HasSingleton = true;
            return this;
        }
        
        public virtual IPluginConfiguration UseFactory<TFactory>()
        {
            HasFactory = true;
            FactoryType = typeof (TFactory);
            return this;
        }

        public virtual IPluginConfiguration AsSingleton()
        {
            IsSingleton = true;
            return this;
        }
    }
}
