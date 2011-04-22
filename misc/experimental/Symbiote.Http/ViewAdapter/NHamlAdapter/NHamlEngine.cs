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
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NHaml;
using NHaml.Compilers.CSharp4;
using NHaml.TemplateResolution;
using Symbiote.Core.Collections;
using Symbiote.Http.Config;
using System.Linq;

namespace Symbiote.Http.ViewAdapter.NHamlAdapter 
{
    public class NHamlEngine : ITemplateContentProvider, IViewEngine
    {
        public readonly string EXTENSION = "haml";
        public TemplateEngine TemplateEngine { get; set; }
        public Type BaseTemplateType { get; set; }
        public ExclusiveConcurrentDictionary<string, CompiledTemplateWrapper> CompiledTemplates { get; set; }
        public IViewLocator ViewLocator { get; set; }
        public IList<string> PathSources { get; set; }
        public object Lock { get; set; }
        
        public IViewSource GetViewSource( string templateFile )
        {
            var info = new FileInfo( templateFile );
            return new FileViewSource( info );
        }

        public IViewSource GetViewSource( string templatePath, IList<IViewSource> parentViewSourceList )
        {
            var file = Path.HasExtension( templatePath ) ? templatePath : string.Join( ".", templatePath, EXTENSION );
            var newPath = file;
            if ( parentViewSourceList.Count > 0 )
            {
                newPath = parentViewSourceList
                    .Select( x => 
                    { 
                        var path = x.Path;
                        var directory = path.Replace( Path.GetFileName( path ), "" );
                        var templateFile = Path.Combine( directory, file );
                        if ( File.Exists( templateFile ) )
                            return templateFile;
                        return null;
                    } ).FirstOrDefault( x => x != null) ?? newPath;

            }
            return GetViewSource( newPath );
        }

        public void AddPathSource( string pathSource )
        {
            PathSources.Add( pathSource );
        }

        private void InitializeTemplateEngine()
        {
            foreach ( var referencedAssembly in Assembly.GetEntryAssembly().GetReferencedAssemblies() )
            {
                TemplateEngine.Options.AddReference( Assembly.Load(referencedAssembly).Location );
            } 
            TemplateEngine.Options.AddReference( typeof( Owin.Impl.Owin ).Assembly.Location );
            TemplateEngine.Options.AddReference( typeof( TemplateEngine ).Assembly.Location );

            TemplateEngine.Options.TemplateBaseType = BaseTemplateType;
            TemplateEngine.Options.TemplateContentProvider = this;
            TemplateEngine.Options.IndentSize = 4;
            TemplateEngine.Options.OutputDebugFiles = true;
            TemplateEngine.Options.UseTabs = true;
            TemplateEngine.Options.TemplateCompiler = new CSharp4TemplateCompiler();
        }

        public CompiledTemplateWrapper GetCompiledTemplateFor<TModel>(string view, string layout)
        {
            Type type = typeof(TModel);
            var combineView = string.Join( "-", layout, view, type.FullName );
            return CompiledTemplates.ReadOrWrite( 
                combineView, () => Compile<TModel>( view, layout ) );
        }

        public CompiledTemplateWrapper Compile<TModel>( string view, string layout )
        {
            return CompiledTemplateWrapper.Create<TModel>( TemplateEngine, view, layout );
        }

        public void Render<TModel>(string view, TModel model, TextWriter writer )
        {
            var viewPath = ViewLocator.GetViewPath<TModel>( view, EXTENSION );
            var layoutPath = ViewLocator.GetDefaultLayout<TModel>( EXTENSION );
            if ( viewPath == null )
                return;
            CompiledTemplateWrapper template = GetCompiledTemplateFor<TModel>( viewPath, layoutPath );
            var instance = template.CreateInstance( model );
            instance.Render( writer );
        }

        public void Render<TModel>(string view, string layout, TModel model, TextWriter writer )
        {
            var viewPath = ViewLocator.GetViewPath<TModel>( view, EXTENSION );
            var layoutPath = ViewLocator.GetViewPath<TModel>( layout, EXTENSION );
            if ( viewPath == null )
                return;
            CompiledTemplateWrapper template = GetCompiledTemplateFor<TModel>(viewPath, layoutPath);
            var instance = template.CreateInstance( model );
            instance.Render( writer );
        }

        public NHamlEngine( IViewLocator viewLocator, HttpWebConfiguration configuration )
        {
            ViewLocator = viewLocator;
            PathSources = new List<string>();
            CompiledTemplates = new ExclusiveConcurrentDictionary<string, CompiledTemplateWrapper>();
            BaseTemplateType = typeof(NHamlView<>);
            TemplateEngine = new TemplateEngine();
            InitializeTemplateEngine();
        }
    }
}
