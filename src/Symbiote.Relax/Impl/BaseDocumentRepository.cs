using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using StructureMap;
using Symbiote.Core.Extensions;

namespace Symbiote.Relax.Impl
{
    public abstract class BaseDocumentRepository<TModel>
        : IDocumentRepository<TModel>
        where TModel : class, ICouchDocument
    {
        protected ICouchConfiguration _configuration;
        protected ICouchCommandFactory _commandFactory;
        protected ConcurrentDictionary<string, bool> _databaseExists = new ConcurrentDictionary<string, bool>();
        protected ConcurrentDictionary<Type, ICouchCommand> _continuousUpdateCommands =
            new ConcurrentDictionary<Type, ICouchCommand>();
        
        protected virtual CouchUri BaseURI()
        {
            var database = _configuration.GetDatabaseNameForType<TModel>();
            var baseURI = CouchUri.Build(
                _configuration.Protocol, 
                _configuration.Server, 
                _configuration.Port, 
                database
                );
            EnsureDatabaseExists(database, baseURI);
            return baseURI;
        }

        protected void EnsureDatabaseExists(string database, CouchUri baseURI)
        {
            var dbCreated = false;
            var shouldCheckCouch = false;
            try
            {
                var command = _commandFactory.GetCommand();
                shouldCheckCouch = !_databaseExists.TryGetValue(database, out dbCreated);
                if (shouldCheckCouch && !dbCreated)
                {
                    command.Put(baseURI);
                    _databaseExists[database] = true;
                }
            }
            catch(WebException webEx)
            {
                if(webEx.Message.Contains("(412) Precondition Failed"))
                {
                    _databaseExists[database] = true;
                }
                else
                {
                    "An exception occurred while trying to check for the existence of database {0} at uri {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository<TModel>>(database, baseURI.ToString(), webEx);
                    throw;
                }

            }
            catch(Exception ex)
            {
                "An exception occurred while trying to check for the existence of database {0} at uri {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository<TModel>>(database, baseURI.ToString(), ex);
                throw;
            }
        }

        public virtual void CreateDatabase()
        {
            string database = "";
            var uri = BaseURI();
            try
            {
                database = _configuration.GetDatabaseNameForType<TModel>();
                var command = _commandFactory.GetCommand();
                command.Put(uri);
                _databaseExists[database] = true;
            }
            catch (Exception ex)
            {
                "An exception occurred trying to create the database {0} at uri {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository<TModel>>(database, uri.ToString(), ex);
                throw;
            }
        }

        public virtual void DeleteDocument<TKey>(TKey id)
        {
            var doc = Get<TKey>(id);

            if(doc != null)
            {
                var uri = BaseURI();
                try
                {
                    var command = _commandFactory.GetCommand();
                    uri = uri.KeyAndRev(doc.DocumentId, doc.DocumentRevision);
                    command.Delete(uri);
                }
                catch (Exception ex)
                {
                    "An exception occurred trying to delete a document of type {0} with id {1} at {2}. \r\n\t {3}"
                        .ToError<IDocumentRepository<TModel>>(typeof(TModel).FullName, id.ToString(), uri.ToString(), ex);
                    throw;
                }
            }
        }

        public virtual void DeleteDocument<TKey, TRev>(TKey id, TRev rev)
        {
            var uri = BaseURI();
            try
            {
                var command = _commandFactory.GetCommand();
                uri = uri.KeyAndRev(id, rev);
                command.Delete(uri);
            }
            catch (Exception ex)
            {
                "An exception occurred trying to delete a document of type {0} with id {1} at {2}. \r\n\t {3}"
                    .ToError<IDocumentRepository<TModel>>(typeof(TModel).FullName, id.ToString(), uri.ToString(), ex);
                throw;
            }
        }

        public virtual void DeleteDatabase()
        {
            var database = "";
            var uri = BaseURI();
            try
            {
                database = _configuration.GetDatabaseNameForType<TModel>();
                var command = _commandFactory.GetCommand();
                command.Delete(uri);
                _databaseExists[database] = false;
            }
            catch (Exception ex)
            {
                "An exception occurred trying to delete the database {0} at {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository<TModel>>(database, uri.ToString(), ex);
                throw;
            }
        }

        public virtual bool DatabaseExists()
        {
            var uri = BaseURI();
            var database = "";
            var exists = false;
            try
            {
                database = _configuration.GetDatabaseNameForType<TModel>();
                var command = _commandFactory.GetCommand();
                var response = command.Get(uri);
                exists = !string.IsNullOrEmpty(response) && !response.StartsWith("{\"error\"");
                _databaseExists[database] = exists;
                return exists;
            }
            catch (Exception ex)
            {
                "An exception occurred checking for the existence of database {0} at {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository<TModel>>(database, uri.ToString(), ex);
                throw;
            }
        }

        public IList<string> DatabaseList
        {
            get
            {
                var uri = CouchUri.Build(
                    _configuration.Protocol,
                    _configuration.Server,
                    _configuration.Port,
                    "_all_dbs");    
                var command = _commandFactory.GetCommand();
                var json = command.Get(uri);
                return new List<string>(json.FromJson<string[]>());
            }
        }

        public virtual TModel Get<TKey, TRev>(TKey id, TRev revision)
        {
            var uri = BaseURI().KeyAndRev(id, revision);
            
            try
            {
                TModel model = default(TModel);
                var command = _commandFactory.GetCommand();
                var json = command.Get(uri);
                model = json.FromJson<TModel>();
                return model;
            }
            catch (Exception ex)
            {
                "An exception occurred trying to retrieve a document of type {0} with id {1} and rev {2} at {3}. \r\n\t {4}"
                    .ToError<IDocumentRepository<TModel>>(
                        typeof(TModel).FullName, 
                        id.ToString(), 
                        revision.ToString(),
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual TModel Get<TKey>(TKey id)
        {
            var uri = BaseURI().Key(id);
            
            try
            {
                TModel model = default(TModel);
                var command = _commandFactory.GetCommand();
                var json = command.Get(uri);
                model = json.FromJson<TModel>();
                return model;
            }
            catch (Exception ex)
            {
                "An exception occurred trying to retrieve a document of type {0} with id {1} at {2}. \r\n\t {3}"
                    .ToError<IDocumentRepository<TModel>>(
                        typeof(TModel).FullName,
                        id.ToString(),
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual IList<TModel> GetAll()
        {
            var uri = BaseURI()
                .ListAll()
                .IncludeDocuments();
            
            try
            {
                var command = _commandFactory.GetCommand();
                var json = command.Get(uri);
                List<TModel> list = new List<TModel>();
                var view = (json.FromJson<ViewResult<TModel>>());
                list = view.GetList().ToList();
                return list;
            }
            catch (Exception ex)
            {
                "An exception occurred trying to retrieve all documents of type {0} at {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository<TModel>>(
                        typeof(TModel).FullName,
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual IList<TModel> GetAll(int pageSize, int pageNumber)
        {
            var uri = BaseURI()
                .ListAll()
                .IncludeDocuments()
                .Skip((pageNumber - 1)*pageSize)
                .Limit(pageSize);
            
            try
            {
                var command = _commandFactory.GetCommand();
                var json = command.Get(uri);
                List<TModel> list = new List<TModel>();
                var view = (json.FromJson<ViewResult<TModel>>());
                list = view.GetList().ToList();
                return list;
            }
            catch (Exception ex)
            {
                "An exception occurred trying to retrieve all documents of type {0} at {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository<TModel>>(
                        typeof(TModel).FullName,
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual void Save(TModel model)
        {
            var uri = BaseURI()
                .Key(model.DocumentId);

            try
            {
                var body = model.ToJson();
                var command = _commandFactory.GetCommand();
                var updatedJSON = command.Put(uri, body);
                var updated = updatedJSON.FromJson<SaveResponse>();
                model.DocumentRevision = updated.Revision;
            }
            catch (Exception ex)
            {
                "An exception occurred trying to save a document of type {0} at {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository<TModel>>(
                        typeof(TModel).FullName,
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual void Save(IEnumerable<TModel> list)
        {
            var uri = BaseURI().BulkInsert();
            
            try
            {
                //var documentList = new BulkPersist<TModel, TKey, TRev>(true, false, list);
                var documentList = new
                                       {
                                           all_or_nothing = true,
                                           non_atomic = false,
                                           docs = list
                                       };
                var command = _commandFactory.GetCommand();
                var body = documentList.ToJson(false);
                var updatedJson = command.Post(uri, body);
                var updated = updatedJson.FromJson<SaveResponse[]>();
                list
                    .ToList()
                    .ForEach(x =>
                                 {
                                     var update = updated.FirstOrDefault(y => y.Id == x.DocumentId.ToString());
                                     if (update != null)
                                         x.DocumentRevision = update.Revision;
                                 });
            }
            catch (Exception ex)
            {
                "An exception occurred trying to save a document of type {0} at {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository<TModel>>(
                        typeof(TModel).FullName,
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual void HandleUpdates(int since, Action<ChangeRecord> onUpdate, AsyncCallback updatesInterrupted)
        {
            var command = _commandFactory.GetCommand();
            Action<CouchUri, int, Action<ChangeRecord>> proxy = command.GetContinuousResponse;
            proxy.BeginInvoke(BaseURI(), since, onUpdate, updatesInterrupted, null);
            _continuousUpdateCommands[typeof(TModel)] = command;
        }

        public virtual void StopChangeStreaming()
        {
            ICouchCommand command;
            var key = typeof (TModel);
            if(_continuousUpdateCommands.TryGetValue(key, out command))
            {
                command.StopContinousResponse();
                _continuousUpdateCommands.TryRemove(key, out command);
            }
        }

        public BaseDocumentRepository(ICouchConfiguration configuration, ICouchCommandFactory commandFactory)
        {
            _configuration = configuration;
            _commandFactory = commandFactory;
        }

        protected BaseDocumentRepository(string configurationName)
        {
            _configuration = ObjectFactory.GetNamedInstance<ICouchConfiguration>(configurationName);
            _commandFactory = ObjectFactory.GetInstance<ICouchCommandFactory>();
        }

        public void Dispose()
        {

        }
    }
}