using System.Web;
using Symbiote.Hibernate;

namespace Symbiote.Web.Impl
{
    public class HttpContextAdapter : ISessionContext
    {
        #region IContext Members

        public bool Contains(string key)
        {
            return HttpContext.Current.Items.Contains(key);
        }

        public void Set(string key, object value)
        {
            HttpContext.Current.Items[key] = value;
        }

        public object Get(string key)
        {
            return HttpContext.Current.Items[key];
        }

        #endregion
    }
}