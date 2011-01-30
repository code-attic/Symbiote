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
using Symbiote.Couch.Impl.Model;

namespace Symbiote.Couch.Impl
{
    public class DocumentHierarchyWatcher : IObserver<Tuple<object, string, object>>
    {
        public List<object> Documents { get; set; }
        public bool Done { get; set; }

        #region IObserver<Tuple<object,string,object>> Members

        public void OnNext( Tuple<object, string, object> value )
        {
            var parent = value.Item1 as BaseDocument;
            var child = value.Item3 as BaseDocument;
            var property = value.Item2;

            if ( parent != null )
            {
                var childIdArray = new object[] {};
                var childIds = new List<object> {child.GetDocumentIdAsJson()};
                if ( parent.RelatedDocumentIds.TryGetValue( property, out childIdArray ) )
                {
                    childIds.AddRange( childIdArray );
                }
                parent.RelatedDocumentIds[property] = childIds.ToArray();
                child.ParentId = parent.GetDocumentIdAsJson();
            }
            Documents.Add( child );
        }

        public void OnError( Exception error )
        {
        }

        public void OnCompleted()
        {
            Done = true;
        }

        #endregion

        public DocumentHierarchyWatcher()
        {
            Documents = new List<object>();
        }
    }
}