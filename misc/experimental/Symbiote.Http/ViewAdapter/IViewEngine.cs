using System.IO;

namespace Symbiote.Http.ViewAdapter
{
    public interface IViewEngine
    {
        void Render<TModel>(string view, TModel model, TextWriter writer );
        void Render<TModel>(string view, string layout, TModel model, TextWriter writer );
    }
}