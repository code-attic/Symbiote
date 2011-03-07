using System.IO;

namespace Symbiote.Http.Impl.ViewProvider
{
    public interface IViewEngine
    {
        void Render<TModel>(string view, TModel model, TextWriter writer );
        void Render<TModel>(string view, string layout, TModel model, TextWriter writer );
    }
}