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
using NHaml;

namespace Symbiote.Http.Impl.ViewProvider.NHamlAdapter
{
    public class CompiledTemplateWrapper
    {
        public CompiledTemplate Template { get; set; }
        public DateTime CompiledAt { get; set; }
        public string LayoutFile { get; set; }
        public string TemplateFile { get; set; }
        public static int Count { get; set; }

        public bool Expired
        {
            get 
            {
                var templateExpired = TemplateFile != null 
                                          ? new FileInfo( TemplateFile ).LastWriteTimeUtc > CompiledAt
                                          : false;

                var layoutExpired = LayoutFile != null 
                                        ? new FileInfo( LayoutFile ).LastWriteTimeUtc > CompiledAt
                                        : false;

                return templateExpired || layoutExpired;
            }
        }

        public IView CreateInstance<TModel>( TModel model )
        {
            RecompileIfExpired();
            var instance = Template.CreateInstance() as IView;
            instance.SetModel( model );
            return instance;
        }

        public void RecompileIfExpired()
        {
            if(Expired)
            {
                Template.Recompile();
                CompiledAt = DateTime.UtcNow;
            }
        }

        public CompiledTemplateWrapper( CompiledTemplate template )
        {
            Template = template;
        }

        public static CompiledTemplateWrapper Create<TModel>(TemplateEngine engine, string templatePath, string layoutPath)
        {
            ++Count;
            Console.WriteLine( Count );
            var type = typeof( TModel );
            var templateType = typeof( NHamlView<> ).MakeGenericType( type );
            var templates = new List<string>();
            if ( !string.IsNullOrEmpty( layoutPath ) )
                templates.Add( layoutPath );
            templates.Add( templatePath );

            var template = engine.Compile( templates , templateType );
            var wrapper = new CompiledTemplateWrapper( template ) 
                              { 
                                  TemplateFile = templatePath,
                                  LayoutFile = layoutPath,
                                  CompiledAt = DateTime.UtcNow
                              };
            return wrapper;
        }
    }
}