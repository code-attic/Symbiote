// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */

using System.IO;
using System.Linq;
using Symbiote.Http.Config;

namespace Symbiote.Http.ViewAdapter
{
    public class DefaultViewLocator : IViewLocator
    {
        public HttpWebConfiguration Configuration { get; set; }

        public string GetDefaultLayout<TModel>( string extension )
        {
            return GetViewPath<TModel>( Configuration.DefaultLayoutTemplate, extension );
        }

        public string GetViewPath( string viewName, string extension )
        {
            var potentialPaths = Configuration.PathSources.Select( x => string.Join(".", Path.Combine( Configuration.BaseContentPath, x, viewName ), extension ) );
            return potentialPaths.FirstOrDefault( File.Exists );
        }

        public string GetViewPath<TModel>( string viewName, string extension )
        {
            var viewPath = typeof(TModel).Name;
            var potentialPaths = Configuration.PathSources.Select( x => string.Join(".", Path.Combine( Configuration.BaseContentPath, x, viewPath, viewName ), extension ) );
            return potentialPaths.FirstOrDefault( File.Exists ) ?? GetViewPath( viewName, extension );
        }

        public DefaultViewLocator( HttpWebConfiguration configuration )
        {
            Configuration = configuration;
        }
    }
}