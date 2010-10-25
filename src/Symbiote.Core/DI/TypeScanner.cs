/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.Extensions;

#if SILVERLIGHT
using System.Windows;
#endif

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

        public void AddAssemblyContaining(Type type)
        {
            AddAssembly(GetAssemblyContaining(type));
        }

        public void AddAssembliesFromBaseDirectory()
        {
            GetAssembliesFromBaseDirectory().ForEach(AddAssembly);
        }

#if !SILVERLIGHT
        public void AddAssembliesFromPath(string path)
        {
            GetAssembliesFromPath(path).ForEach(AddAssembly);
        }
#endif


        public void AddCallingAssembly()
        {
            AddAssembly(GetCallingAssembly());
        }

        public Assembly GetAssemblyContaining<T>()
        {
            return GetAssemblyContaining(typeof(T));
        }

        public Assembly GetAssemblyContaining(Type type)
        {
            return type.Assembly;
        }

        public IEnumerable<Assembly> GetAssembliesFromBaseDirectory()
        {
#if !SILVERLIGHT
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            var assembliesFromBase = GetAssembliesFromPath(basePath);
            var assembliesFromBin = GetAssembliesFromPath(binPath);
            return assembliesFromBase.Concat(assembliesFromBin);
#endif
#if SILVERLIGHT
            return Deployment
                .Current
                .Parts
                .Select(x => Application.GetResourceStream(new Uri(x.Source, UriKind.Relative)))
                .Select(x => new AssemblyPart().Load(x.Stream));
#endif
        }

#if !SILVERLIGHT
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
#endif


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
