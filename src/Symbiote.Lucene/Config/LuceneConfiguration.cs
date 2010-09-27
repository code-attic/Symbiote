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

using System.Collections.Generic;
using Symbiote.Lucene.Impl;

namespace Symbiote.Lucene.Config
{
    public class LuceneConfiguration : ILuceneConfiguration
    {
        public int MemoryBufferLimit { get; set; }
        public long WriterLockTimeout { get; set; }
        public string IndexPath { get; set; }
        public IDictionary<string, string> DirectoryPaths { get; set; }
        public IDictionary<string, IDirectoryFactory> DirectoryFactories { get; set; }
        public IDictionary<string, IAnalyzerFactory> AnalyzerFactories { get; set; }

        public LuceneConfiguration()
        {
            MemoryBufferLimit = 512;
            WriterLockTimeout = 30;
            DirectoryPaths = new Dictionary<string, string>();
            DirectoryFactories = new Dictionary<string, IDirectoryFactory>();
            AnalyzerFactories = new Dictionary<string, IAnalyzerFactory>();
            IndexPath = @"/index";
        }
    }
}
