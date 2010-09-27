/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Collections.Concurrent;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using ObservableExtensions = System.ObservableExtensions;
using System;

namespace Symbiote.Lucene.Impl
{
    public class DocumentQueue 
        : IDocumentQueue
    {
        public IndexWriter IndexWriter { get; set; }
        public ConcurrentQueue<Document> Documents { get; set; }
        public object Lock { get; set; }

        public void PushDocument(Document document)
        {
            Documents.Enqueue(document);
        }

        public void IndexDocument(Document document)
        {
            lock(Lock)
            {
                this.IndexWriter.AddDocument(document);
            }
        }

        public DocumentQueue(IndexWriter indexWriter)
        {
            Lock = new object();
            IndexWriter = indexWriter;
            Documents = new ConcurrentQueue<Document>();
            Documents.ToObservable().Subscribe(x => IndexDocument(x));
        }
    }
}