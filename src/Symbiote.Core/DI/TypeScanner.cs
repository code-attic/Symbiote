using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.DI
{
    public class TypeScanner
    {
        public IList<Assembly> CandidateAssemblies { get; set; }
        public IList<Predicate<Assembly>> AssemblyFilters { get; set; }
        public IList<Predicate<Type>> TypeFilters { get; set; }

        public TypeScanner()
        {
            CandidateAssemblies = new List<Assembly>();
            AssemblyFilters = new List<Predicate<Assembly>>();
            TypeFilters = new List<Predicate<Type>>();
        }

        public void AddAssembly(Assembly assembly)
        {
            CandidateAssemblies.Add(assembly);
        }

        public void AddAssemblyContaining<T>()
        {
            AddAssembly(GetAssemblyContaining<T>());
        }

        public void AddAssemblyByName(string assemblyName)
        {
            AddAssembly(AppDomain.CurrentDomain.Load(assemblyName));
        }

        public void AddAssemblyContaining(Type type)
        {
            AddAssembly(GetAssemblyContaining(type));
        }

        public void AddAssembliesFromBaseDirectory()
        {
            GetAssembliesFromBaseDirectory().ForEach(AddAssembly);
        }

        public void AddAssembliesFromPath(string path)
        {
            GetAssembliesFromPath(path).ForEach(AddAssembly);
        }

        public void AddCallingAssembly()
        {
            AddAssembly(GetCallingAssembly());
        }

        public Assembly GetAssemblyContaining<T>()
        {
            return GetAssemblyContaining(typeof(T));
        }

        public Assembly GetAssemblyByName(string assemblyName)
        {
            return AppDomain.CurrentDomain.Load(assemblyName);
        }

        public Assembly GetAssemblyContaining(Type type)
        {
            return type.Assembly;
        }

        public IEnumerable<Assembly> GetAssembliesFromBaseDirectory()
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            var assembliesFromBase = GetAssembliesFromPath(basePath);
            var assembliesFromBin = GetAssembliesFromPath(binPath);
            return assembliesFromBase.Concat(assembliesFromBin);
        }

        public IEnumerable<Assembly> GetAssembliesFromPath(string path)
        {
            if (!Directory.Exists(path))
                return new Assembly[] {};

            return Directory.GetFiles(path)
                .Where(x => x.ToLower().EndsWith(".dll") || x.ToLower().EndsWith(".exe"))
                .Select(x =>
                {
                    Assembly assembly = null;
                    try { assembly = Assembly.LoadFrom(x); }
                    catch { }
                    return assembly;
                })
                .Where(x => x != null);
        }

        public Assembly GetCallingAssembly()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var callingAssembly = new StackTrace(false)
                .GetFrames()
                .Select(x => x.GetMethod().DeclaringType.Assembly)
                .FirstOrDefault(x => x != executingAssembly);
            return callingAssembly ?? executingAssembly;
        }

        public IEnumerable<Type> GetMatchingTypes()
        {
            return
                CandidateAssemblies
                    .Where(x => AssemblyFilters.All(p => p(x)))
                    .SelectMany(x =>
                                    {
                                        try { return x.GetTypes(); }
                                        catch { return new Type[]{}; }
                                    })
                    .Where(x => !TypeFilters.Any(p => p(x)));
        }
    }
}
