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
using System.Web.Mvc;
using NHaml;
using NHaml.Compilers.CSharp4;
using NHaml.TemplateResolution;
using Symbiote.Core.Collections;
using System.Linq;
using Symbiote.Http.Config;
using Symbiote.Core.Extensions;

namespace Symbiote.Http.Impl.ViewProvider.NHamlAdapter 
{
    public class NHamlEngine : ITemplateContentProvider, IViewEngine
    {
        public HttpWebConfiguration Configuration { get; set; }
        public TemplateEngine TemplateEngine { get; set; }
        public string DefaultMaster { get; set; }
        public IList<string> PathSources { get; set; }
        public Type BaseTemplateType { get; set; }
        public ExclusiveConcurrentDictionary<Tuple<string, Type>, CompiledTemplate> CompiledTemplates { get; set; }
        public const string typedPathTemplate = @"{0}/{1}/{2}/{3}.haml";
        public const string pathTemplate = @"{0}/{1}/{2}.haml";

        public string GetViewPath( string templateName )
        {
            return PathSources.FirstOrDefault( x => 
                File.Exists( pathTemplate.AsFormat( Configuration.BaseContentPath, x, templateName ) ) );
        }

        public string GetViewPath<TModel>( string templateName )
        {
            var viewPath = typeof(TModel).Name;
            var potentialPaths = PathSources.Select( x => Path.Combine( Configuration.BaseContentPath, x, viewPath, templateName ) + ".haml" );
            return potentialPaths.FirstOrDefault( File.Exists );
        }

        public IViewSource GetViewSource( string templateFile )
        {
            var info = new FileInfo( templateFile );
            return new FileViewSource( info );
        }

        public IViewSource GetViewSource( string templatePath, IList<IViewSource> parentViewSourceList )
        {
            return GetViewSource( templatePath );   
        }

        public void AddPathSource( string pathSource )
        {
            PathSources.Add( pathSource );
        }

        private void InitializeTemplateEngine()
        {
            DefaultMaster = "Application";
            foreach ( var referencedAssembly in Assembly.GetEntryAssembly().GetReferencedAssemblies() )
            {
                TemplateEngine.Options.AddReference( Assembly.Load(referencedAssembly).Location );
            } 
            TemplateEngine.Options.AddReference( typeof( Owin ).Assembly.Location );
            TemplateEngine.Options.AddReference( typeof( TemplateEngine ).Assembly.Location );

            TemplateEngine.Options.TemplateBaseType = BaseTemplateType;
            TemplateEngine.Options.TemplateContentProvider = this;
            TemplateEngine.Options.IndentSize = 4;
            TemplateEngine.Options.OutputDebugFiles = true;
            TemplateEngine.Options.UseTabs = true;
            TemplateEngine.Options.TemplateCompiler = new CSharp4TemplateCompiler();
        }

        public CompiledTemplate GetCompiledTemplateFor<TModel>(string view)
        {
            Type type = typeof(TModel);
            return CompiledTemplates.ReadOrWrite( Tuple.Create( view, type ),
                () => TemplateEngine.Compile( view, typeof( NHamlView<> ).MakeGenericType( type ) ) );
        }

        public void Render<TModel>(string view, TModel model, TextWriter writer )
        {
            var viewPath = GetViewPath<TModel>( view );
            CompiledTemplate template = GetCompiledTemplateFor<TModel>(viewPath);
            var instance = ( IView ) template.CreateInstance();
            instance.SetModel( model );
            instance.Render( writer );
        }

        public NHamlEngine( HttpWebConfiguration configuration )
        {
            PathSources = new List<string>();
            CompiledTemplates = new ExclusiveConcurrentDictionary<Tuple<string, Type>, CompiledTemplate>();
            BaseTemplateType = typeof(NHamlView<>);
            TemplateEngine = new TemplateEngine();
            Configuration = configuration;
            InitializeTemplateEngine();
            PathSources.Add( "Views" );
            PathSources.Add( "Shared" );
        }
    }
}
