using System;
using System.IO;
using Nancy.ViewEngines;

namespace Symbiote.Nancy.Impl
{
    public class SymbioteViewLocator
        : IViewLocator
    {
        public ViewLocationResult GetTemplateContents( string viewTemplate )
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var relativePath = viewTemplate;
            var path = Path.Combine( basePath, relativePath );
            using (var fs = File.OpenRead(path))
            {
                var stream = new MemoryStream();
                fs.CopyTo(stream);
                stream.Position = 0;
                return new ViewLocationResult(path, new StreamReader(stream));
            }
        }
    }
}