using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Symbiote.Core.Config
{
    public class FromConfig
    {
        protected static Dictionary<Type, Func<string, object>> Conversions 
            = new Dictionary<Type, Func<string, object>>()
            {
                {typeof(bool), x => bool.Parse(x)},                                                                          
                {typeof(string), x => x},                                                                          
                {typeof(short), x => int.Parse(x)},                                                                          
                {typeof(int), x => int.Parse(x)},                                                                          
                {typeof(long), x => int.Parse(x)},                                                                          
                {typeof(decimal), x => int.Parse(x)},                                                                          
                {typeof(double), x => int.Parse(x)},                                                                          
                {typeof(float), x => int.Parse(x)},                                                                          
                {typeof(DateTime), x => int.Parse(x)},                                                                          
                {typeof(Guid), x => int.Parse(x)},                                                                          
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
