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
using Symbiote.Core.Extensions;

namespace Symbiote.Core.DI
{
    public class DependencyConfigurator :
        IRequestPlugin,
        IScan
    {
        public IList<IDependencyDefinition> Dependencies { get; set; }
        public IList<IScanInstruction> ScanInstructions { get; set; }

        #region IRequestPlugin Members

        public ISupplyPlugin<object> For( Type pluginType )
        {
            var expression = DependencyExpression.For( pluginType );
            Dependencies.Add( expression );
            return expression;
        }

        public ISupplyPlugin<TPlugin> For<TPlugin>()
        {
            var expression = DependencyExpression.For<TPlugin>();
            Dependencies.Add( expression );
            return expression;
        }

        public ISupplyPlugin<object> For( string name, Type pluginType )
        {
            var expression = DependencyExpression.For( name, pluginType );
            Dependencies.Add( expression );
            return expression;
        }

        public ISupplyPlugin<TPlugin> For<TPlugin>( string name )
        {
            var expression = DependencyExpression.For<TPlugin>( name );
            Dependencies.Add( expression );
            return expression;
        }

        #endregion

        #region IScan Members

        public void Scan( Action<IScanInstruction> scanConfigurator )
        {
            var instruction = new ScanInstruction();
            scanConfigurator( instruction );
            ScanInstructions.Add( instruction );
        }

        #endregion

        public void RegisterDependencies( IDependencyRegistry registry )
        {
            Dependencies
                .ForEach( registry.Register );

            ScanInstructions
                .ForEach( registry.Scan );
        }

        public DependencyConfigurator()
        {
            Dependencies = new List<IDependencyDefinition>();
            ScanInstructions = new List<IScanInstruction>();
        }
    }
}