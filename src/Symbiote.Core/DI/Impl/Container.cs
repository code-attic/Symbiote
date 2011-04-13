using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Symbiote.Core.DI.Impl
{
    public struct DependencyKey
    {
        public Type Type;
        public string Name;

        public DependencyKey( Type type, string name )
        {
            Type = type;
            Name = name;
        }
    }

    public struct ConstructorDefinition
    {
        public Type Type;
        public string Name;
        public ConstructorInfo Constructor;
        public ParameterInfo[] Parameters;
    }

    public interface ICreateInstanceFactory
    {
        Func<object> CompileExpressionToFunc( ConstructorDefinition constructor );
        Func<T> CompileExpressionToFunc<T>( ConstructorDefinition constructor );
    }

    public interface IProvideInstance
    {
        object Get();
        T Get<T>() where T : class;
    }

    public class InstanceFactory<TInstance> :
        IProvideInstance
        where TInstance : class
    {
        public Func<TInstance> Factory { get; set; }

        public object Get()
        {
            return Get<TInstance>();
        }

        public T Get<T>()
            where T : class
        {
            return Factory() as T;
        }

        public InstanceFactory( Func<TInstance> factory )
        {
            Factory = factory;
        }
    }

    public class SingletonFactory<TInstance> : 
        IProvideInstance
        where TInstance : class
    {
        public Func<TInstance> Factory { get; set; }
        public TInstance Instance { get; set; }

        public object Get()
        {
            return Get<TInstance>();
        }

        public T Get<T>()
            where T : class
        {
            Instance = Instance ?? Factory();
            return Instance as T;
        }

        public SingletonFactory( Func<TInstance> factory, TInstance instance )
        {
            Factory = factory;
            Instance = instance;
        }
    }

    public class StaticInstanceProvider<TInstance> :
        IProvideInstance
        where TInstance : class
    {
        public TInstance Instance { get; set; }

        public object Get()
        {
            return Get<TInstance>();
        }

        public T Get<T>()
            where T : class
        {
            return Instance as T;    
        }

        public StaticInstanceProvider( TInstance instance )
        {
            Instance = instance;
        }
    }

    public class InstanceFactoryLambda :
        ICreateInstanceFactory
    {
        public ConcurrentDictionary<DependencyKey, ConstructorDefinition> ConstructorCache { get; set; }
        public Func<ConstructorDefinition, object> Factory { get; set; }

        public ConstructorDefinition GetDefinition( DependencyKey key )
        {
            return ConstructorCache[key];
        }

        public Expression BuildExpression( ConstructorDefinition constructor )
        {
            var args = constructor
                .Parameters
                .Select( x => GetDefinition( new DependencyKey( x.ParameterType, null ) ) )
                .Select( BuildExpression );

            return Expression.New( constructor.Constructor, args );
        }

        public Func<T> CompileExpressionToFunc<T>( ConstructorDefinition constructor )
        {
            var body = BuildExpression( constructor );
            return Expression.Lambda<Func<T>>( body, false, null ).Compile();
        }

        public Func<object> CompileExpressionToFunc( ConstructorDefinition constructor )
        {
            var body = BuildExpression( constructor );
            return Expression.Lambda<Func<object>>( body, false, null ).Compile();
        }
    }

    public class Container :
        IDependencyAdapter
    {
        public ConcurrentDictionary<DependencyKey, ConstructorDefinition> ConstructorCache { get; set; }
        public ConcurrentDictionary<DependencyKey, ICreateInstanceFactory> InstanceFactories { get; set; }
        public ICreateInstanceFactory FactoryProvider { get; set; }
        
        public object GetInstance( Type serviceType )
        {
            throw new NotImplementedException();
        }

        public object GetInstance( Type serviceType, string key )
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAllInstances( Type serviceType )
        {
            throw new NotImplementedException();
        }

        public T GetInstance<T>()
        {
            throw new NotImplementedException();
        }

        public T GetInstance<T>( string key )
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Type> RegisteredPluginTypes
        {
            get { throw new NotImplementedException(); }
        }

        public Type GetDefaultTypeFor<T>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Type> GetTypesRegisteredFor<T>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Type> GetTypesRegisteredFor( Type type )
        {
            throw new NotImplementedException();
        }

        public bool HasPluginFor<T>()
        {
            throw new NotImplementedException();
        }

        public void Register( IDependencyDefinition dependency )
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Scan( IScanInstruction scanInstruction )
        {
            throw new NotImplementedException();
        }
    }
}
