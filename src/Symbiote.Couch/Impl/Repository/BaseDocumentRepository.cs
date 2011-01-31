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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Symbiote.Couch.Config;
using Symbiote.Couch.Impl.Commands;
using Symbiote.Couch.Impl.Http;
using Symbiote.Couch.Impl.Json;
using Symbiote.Couch.Impl.Model;

namespace Symbiote.Couch.Impl.Repository
{
    public abstract class BaseDocumentRepository : IDocumentRepository
    {
        protected ConcurrentDictionary<string, IHttpAction> _continuousUpdateCommands =
            new ConcurrentDictionary<string, IHttpAction>();

        protected ICouchConfiguration configuration { get; set; }
        protected CouchCommandFactory commandFactory { get; set; }

        public virtual void DeleteAttachment<TModel>( TModel model, string attachmentName )
            where TModel : IHaveAttachments
        {
            var command = commandFactory.CreateDeleteAttachmentCommand();
            var response = command.DeleteAttachment( model, attachmentName );
        }

        public virtual void DeleteDocument<TModel>( object id )
        {
            var deleteCommand = commandFactory.CreateDeleteCommand();
            var getCommand = commandFactory.CreateGetDocumentCommand();

            var getResult = getCommand.GetDocument<TModel>( id );
            var doc = getResult.GetResultAs<TModel>();
            deleteCommand.DeleteDocument( doc );
        }

        public virtual void DeleteDocument<TModel>( object id, string rev )
        {
            var deleteCommand = commandFactory.CreateDeleteCommand();
            deleteCommand.DeleteDocument<TModel>( id, rev );
        }

        public virtual IList<TModel> FromView<TModel>( string designDocument, string viewName, Action<ViewQuery> query )
        {
            var command = commandFactory.CreateGetFromViewCommand();
            var response = command.GetFromView<TModel>( designDocument, viewName, query );
            return response.GetResultAs<ViewResult<TModel>>().GetList().ToList();
        }

        public virtual TModel Get<TModel>( object id, string revision )
        {
            var command = commandFactory.CreateGetDocumentCommand();
            var result = command.GetDocument<TModel>( id, revision );
            return result.GetResultAs<TModel>();
        }

        public virtual TModel Get<TModel>( object id )
        {
            var command = commandFactory.CreateGetDocumentCommand();
            var result = command.GetDocument<TModel>( id );
            return result.GetResultAs<TModel>();
        }

        public virtual IList<TModel> GetAll<TModel>()
        {
            var command = commandFactory.CreateGetAllDocumentsCommand();
            var result = command.GetDocuments<TModel>();
            return result.GetResultAs<ViewResult<TModel>>().GetList().ToList();
        }

        public virtual IList<TModel> GetAll<TModel>( int pageSize, int pageNumber )
        {
            var command = commandFactory.CreateGetDocumentsPagedCommand();
            var result = command.GetDocumentsPaged<TModel>( pageSize, pageNumber );
            return result.GetResultAs<ViewResult<TModel>>().GetList().ToList();
        }

        public virtual IList<TModel> GetAllByKeys<TModel>( object[] ids )
        {
            var command = commandFactory.CreateGetDocumentsByIdsCommand();
            var result = command.GetDocuments<TModel>( ids );
            return result.GetResultAs<ViewResult<TModel>>().GetList().ToList();
        }

        public virtual IList<TModel> GetAllBetweenKeys<TModel>( object startingWith, object endingWith )
        {
            var command = commandFactory.CreateGetDocumentsInRangeCommand();
            var result = command.GetDocumentsInRange<TModel>( startingWith, endingWith );
            return result.GetResultAs<ViewResult<TModel>>().GetList().ToList();
        }

        public IList<TModel> GetAllByCriteria<TModel>( Expression<Func<TModel, bool>> criteria )
        {
            var command = commandFactory.CreateQueryCommand();
            var ids = command.GetDocumentIdsForQuery( criteria );
            return GetAllByKeys<TModel>( ids );
        }

        public virtual Tuple<string, byte[]> GetAttachment<TModel>( object id, string attachmentName )
            where TModel : IHaveAttachments
        {
            var command = commandFactory.CreateGetAttachmentCommand();
            var result = command.GetAttachment<TModel>( id, attachmentName );
            return result;
        }

        public virtual void Save<TModel>( TModel model )
        {
            var command = commandFactory.CreateSaveDocumentCommand();
            var result = command.Save( model );
        }

        public virtual void SaveAll<TModel>( IEnumerable<TModel> list )
        {
            var command = commandFactory.CreateSaveDocumentsCommand();
            var result = command.SaveAll( list );
        }

        public virtual void SaveAttachment<TModel>( TModel model, string attachmentName, string contentType,
                                                    byte[] content )
            where TModel : IHaveAttachments
        {
            var command = commandFactory.CreateSaveAttachmentCommand();
            var result = command.SaveAttachment( model, attachmentName, contentType, content );
        }

        public virtual void HandleUpdates<TModel>( int since, Action<string, ChangeRecord> onUpdate,
                                                   AsyncCallback updatesInterrupted )
        {
            var database = configuration.GetDatabaseNameForType<TModel>();
            HandleUpdates( database, since, onUpdate, updatesInterrupted );
        }

        public virtual void HandleUpdates( string database, int since, Action<string, ChangeRecord> onUpdate,
                                           AsyncCallback updatesInterrupted )
        {
            var command = commandFactory.CreateStreamCommand();
            _continuousUpdateCommands[database] = command.BeginStreaming( database, since, onUpdate, updatesInterrupted );
        }

        public virtual void StopChangeStreaming<TModel>()
        {
            var database = configuration.GetDatabaseNameForType<TModel>();
            StopChangeStreaming( database );
        }

        public virtual void StopChangeStreaming( string database )
        {
            IHttpAction command;
            if ( _continuousUpdateCommands.TryGetValue( database, out command ) )
            {
                command.StopContinousResponse();
                _continuousUpdateCommands.TryRemove( database, out command );
            }
        }

        public void Dispose()
        {
        }

        protected BaseDocumentRepository( ICouchConfiguration configuration )
        {
            this.configuration = configuration;
            commandFactory = new CouchCommandFactory( configuration );
        }
    }
}