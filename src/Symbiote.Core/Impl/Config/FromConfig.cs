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
using System.Collections.Specialized;
using System.Configuration;

namespace Symbiote.Core.Impl.Config
{
    public class FromConfig
    {
        protected static Dictionary<Type, Func<string, object>> Conversions 
            = new Dictionary<Type, Func<string, object>>()
            {
                {typeof(bool), x => bool.Parse(x)},                                                                          
                {typeof(string), x => x},                                                                          
                {typeof(short), x => short.Parse(x)},                                                                          
                {typeof(int), x => int.Parse(x)},                                                                          
                {typeof(long), x => long.Parse(x)},                                                                          
                {typeof(decimal), x => decimal.Parse(x)},                                                                          
                {typeof(double), x => double.Parse(x)},                                                                          
                {typeof(float), x => float.Parse(x)},                                                                          
                {typeof(DateTime), x => DateTime.Parse(x)},                                                                          
                {typeof(Guid), x => Guid.Parse(x)},                                                                          
            };

        protected static NameValueCollection ConfigCollection { get; set; }
        protected static ConnectionStringSettingsCollection ConnectionStrings { get; set; }

        public static T GetValue<T>(string key)
        {
            return (T) Conversions[typeof (T)](ConfigCollection[key]);
        }

        public static TSection GetSection<TSection>(string sectionName)
        {
            return (TSection) ConfigurationManager.GetSection(sectionName);
        }

        public static TValue GetSectionValue<TSection, TValue>(string sectionName, Func<TSection, TValue> sectionValue)
        {
            var section = (TSection) ConfigurationManager.GetSection(sectionName);
            return sectionValue(section);
        }

        public static string GetConnectionString(string connectionName)
        {
            return ConnectionStrings[connectionName].ConnectionString;
        }

        public static string GetProvider(string connectionName)
        {
            return ConnectionStrings[connectionName].ProviderName;
        }

        static FromConfig()
        {
            ConfigCollection = ConfigurationManager.AppSettings;
            ConnectionStrings = ConfigurationManager.ConnectionStrings;
        }
    }
}
