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
using Symbiote.Core;

namespace Symbiote.Couch.Config
{
    public class CouchConfigurator
    {
        private CouchConfiguration _config = new CouchConfiguration();

        public CouchConfigurator BreakDocumentGraphsIntoSeperateDocuments()
        {
            _config.BreakDownDocumentGraphs = true;
            return this;
        }

        public CouchConfigurator Cache()
        {
            _config.Cache = true;
            _config.CacheExpiration = DateTime.MaxValue;
            return this;
        }

        public CouchConfigurator Cache( DateTime expiration )
        {
            _config.Cache = true;
            _config.CacheExpiration = expiration;
            return this;
        }

        public CouchConfigurator Cache( TimeSpan timeLimit )
        {
            _config.Cache = true;
            _config.CacheLimit = timeLimit;
            return this;
        }

        public CouchConfigurator ExcludeTypeSpecificationFromJson()
        {
            _config.IncludeTypeSpecification = false;
            return this;
        }

        public CouchConfigurator FailedGetShouldThrowException()
        {
            _config.Throw404Exceptions = true;
            return this;
        }

        public CouchConfigurator Https()
        {
            _config.Protocol = "https";
            return this;
        }

        public CouchConfigurator Port( int port )
        {
            _config.Port = port;
            return this;
        }

        public CouchConfigurator Preauthorize( string username, string password )
        {
            _config.Preauthorize = true;
            _config.User = username;
            _config.Password = password;
            return this;
        }

        public CouchConfigurator CouchQueryServiceUrl( string url )
        {
            _config.CouchQueryServiceUrl = url;
            return this;
        }

        public CouchConfigurator Server( string server )
        {
            _config.Server = server;
            return this;
        }

        public CouchConfigurator TimeOut( int timeOut )
        {
            _config.TimeOut = timeOut;
            return this;
        }

        public CouchConfigurator DefaultDatabase( string databaseName )
        {
            _config.DefaultDatabaseName = databaseName;
            return this;
        }

        public CouchConfigurator AssignDatabaseToType<T>( string databaseName )
        {
            _config.SetDatabaseNameForType<T>( databaseName );
            return this;
        }

        public CouchConfigurator UseDatabaseTypeResolver<T>()
            where T : IResolveDatabaseNames
        {
            _config.DatabaseResolver = Assimilate.GetInstanceOf<T>();
            return this;
        }

        public CouchConfigurator WithConventions( string idProperty, string revisionProperty )
        {
            _config.Conventions.IdPropertyName = idProperty;
            _config.Conventions.RevisionPropertyName = revisionProperty;
            return this;
        }

        public ICouchConfiguration GetConfiguration()
        {
            return _config;
        }
    }
}