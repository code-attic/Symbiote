using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using StructureMap;
using Symbiote.Core.Extensions;

namespace Symbiote.Relax.Impl
{
    public abstract class BaseDocumentRepository<TModel, TKey, TRev>
        : IDocumentRepository<TModel, TKey, TRev>
        where TModel : class, ICouchDocument<TKey, TRev>
    {
        protected BaseDocumentRepository<TKey, TRev> _repository;

        public void Dispose()
        {
            _repository.Dispose();
        }

        public virtual void CreateDatabase()
        {
            _repository.CreateDatabase<TModel>();
        }

        public virtual void DeleteDatabase()
        {
            _repository.DeleteDatabase<TModel>();
        }

        public void DeleteDocument(TKey id)
        {
            _repository.DeleteDocument<TModel>(id);
        }

        public virtual bool DatabaseExists()
        {
            return _repository.DatabaseExists<TModel>();
        }

        public virtual IList<string> DatabaseList
        {
            get { return _repository.DatabaseList; }
        }

        public virtual TModel Get(TKey id, TRev revision)
        {
            return _repository.Get<TModel>(id, revision);
        }

        public virtual TModel Get(TKey id)
        {
            return _repository.Get<TModel>(id);
        }

        public virtual IList<TModel> GetAll()
        {
            return _repository.GetAll<TModel>();
        }

        public virtual IList<TModel> GetAll(int pageSize, int pageNumber)
        {
            return _repository.GetAll<TModel>(pageSize, pageNumber);
        }

        public virtual void Save(TModel model)
        {
            _repository.Save(model);
        }

        public virtual void Save(IEnumerable<TModel> list)
        {
            _repository.Save(list);
        }

        public virtual void HandleUpdates(int since, Action<ChangeRecord> onUpdate, AsyncCallback updatesInterrupted)
        {
            _repository.HandleUpdates<TModel>(since, onUpdate, updatesInterrupted);
        }

        public virtual void StopChangeStreaming()
        {
            _repository.StopChangeStreaming<TModel>();
        }

        public BaseDocumentRepository(IDocumentRepository<TKey, TRev> repository)
        {
            _repository = repository as BaseDocumentRepository<TKey, TRev>;
            if(!_repository.DatabaseExists<TModel>())
            {
                _repository.CreateDatabase<TModel>();
            }
        }

        protected BaseDocumentRepository(string configurationName)
        {
            var configuration = ObjectFactory.GetNamedInstance<ICouchConfiguration>(configurationName);
            _repository = new DocumentRepository<TModel, TKey, TRev>(configurationName) as BaseDocumentRepository<TKey, TRev>;
            if (!_repository.DatabaseExists<TModel>())
            {
                _repository.CreateDatabase<TModel>();
            }
        }
    }

    public abstract class BaseDocumentRepository<TKey, TRev>
        : IDocumentRepository<TKey, TRev>
    {
        protected ICouchConfiguration _configuration;
        protected ConcurrentDictionary<string, bool> _databaseExists = new ConcurrentDictionary<string, bool>();
        protected ConcurrentDictionary<Type, CouchCommand> _continuousUpdateCommands =
            new ConcurrentDictionary<Type, CouchCommand>();

        protected virtual CouchURI BaseURI<TModel>()
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var database = _configuration.GetDatabaseNameForType<TModel>();
            var baseURI = CouchURI.Build(
                _configuration.Protocol, 
                _configuration.Server, 
                _configuration.Port, 
                database
                );
            EnsureDatabaseExists(database, baseURI);
            return baseURI;
        }

        protected void EnsureDatabaseExists(string database, CouchURI baseURI)
        {
            var dbCreated = false;
            var shouldCheckCouch = false;
            try
            {
                var command = new CouchCommand(_configuration);
                shouldCheckCouch = !_databaseExists.TryGetValue(database, out dbCreated);
                if (shouldCheckCouch && !dbCreated)
                {
                    var response = command.Get(baseURI);
                    dbCreated = !string.IsNullOrEmpty(response) && !response.StartsWith("{\"error\"");
                }
                if (!dbCreated)
                {
                    command.Put(baseURI);
                    _databaseExists[database] = true;
                }
            }
            catch(Exception ex)
            {
                "An exception occurred while trying to check for the existence of database {0} at uri {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository>(database, baseURI.ToString(), ex);
                throw;
            }
        }

        public virtual void CreateDatabase<TModel>()
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            string database = "";
            var uri = BaseURI<TModel>();
            try
            {
                database = _configuration.GetDatabaseNameForType<TModel>();
                var command = new CouchCommand(_configuration);
                command.Put(uri);
                _databaseExists[database] = true;
            }
            catch (Exception ex)
            {
                "An exception occurred trying to create the database {0} at uri {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository>(database, uri.ToString(), ex);
                throw;
            }
        }

        public void DeleteDocument<TModel>(TKey id)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>();
            try
            {
                var command = new CouchCommand(_configuration);
                uri = uri.Key(id);
                command.Delete(uri);
            }
            catch (Exception ex)
            {
                "An exception occurred trying to delete a document of type {0} with id {1} at {2}. \r\n\t {3}"
                    .ToError<IDocumentRepository>(typeof (TModel).FullName, id.ToString(), uri.ToString(), ex);
                throw;
            }
        }

        public virtual void DeleteDatabase<TModel>()
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var database = "";
            var uri = BaseURI<TModel>();
            try
            {
                database = _configuration.GetDatabaseNameForType<TModel>();
                var command = new CouchCommand(_configuration);
                command.Delete(uri);
                _databaseExists[database] = false;
            }
            catch (Exception ex)
            {
                "An exception occurred trying to delete the database {0} at {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository>(database, uri.ToString(), ex);
                throw;
            }
        }

        public virtual bool DatabaseExists<TModel>()
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>();
            var database = "";
            var exists = false;
            try
            {
                database = _configuration.GetDatabaseNameForType<TModel>();
                var command = new CouchCommand(_configuration);
                var response = command.Get(uri);
                exists = !string.IsNullOrEmpty(response) && !response.StartsWith("{\"error\"");
                _databaseExists[database] = exists;
                return exists;
            }
            catch (Exception ex)
            {
                "An exception occurred checking for the existence of database {0} at {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository>(database, uri.ToString(), ex);
                throw;
            }
        }

        public IList<string> DatabaseList
        {
            get
            {
                var uri = CouchURI.Build(
                    _configuration.Protocol,
                    _configuration.Server,
                    _configuration.Port,
                    "_all_dbs");    
                var command = new CouchCommand(_configuration);
                var json = command.Get(uri);
                return new List<string>(json.FromJson<string[]>());
            }
        }

        public virtual TModel Get<TModel>(TKey id, TRev revision)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>().KeyAndRev(id, revision);
            
            try
            {
                TModel model = default(TModel);
                var command = new CouchCommand(_configuration);
                var json = command.Get(uri);
                model = json.FromJson<TModel>();
                return model;
            }
            catch (Exception ex)
            {
                "An exception occurred trying to retrieve a document of type {0} with id {1} and rev {2} at {3}. \r\n\t {4}"
                    .ToError<IDocumentRepository>(
                        typeof(TModel).FullName, 
                        id.ToString(), 
                        revision.ToString(),
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual TModel Get<TModel>(TKey id)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>().Key(id);
            
            try
            {
                TModel model = default(TModel);
                var command = new CouchCommand(_configuration);
                var json = command.Get(uri);
                model = json.FromJson<TModel>();
                return model;
            }
            catch (Exception ex)
            {
                "An exception occurred trying to retrieve a document of type {0} with id {1} at {2}. \r\n\t {3}"
                    .ToError<IDocumentRepository>(
                        typeof(TModel).FullName,
                        id.ToString(),
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual IList<TModel> GetAll<TModel>()
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>()
                .ListAll()
                .IncludeDocuments();
            
            try
            {
                var command = new CouchCommand(_configuration);
                var json = command.Get(uri);
                List<TModel> list = new List<TModel>();
                var view = (json.FromJson<ViewResult<TModel, TKey, TRev>>());
                list = view.GetList().ToList();
                return list;
            }
            catch (Exception ex)
            {
                "An exception occurred trying to retrieve all documents of type {0} at {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository>(
                        typeof(TModel).FullName,
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual IList<TModel> GetAll<TModel>(int pageSize, int pageNumber)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>()
                .ListAll()
                .IncludeDocuments()
                .Skip((pageNumber - 1)*pageSize)
                .Limit(pageSize);
            
            try
            {
                var command = new CouchCommand(_configuration);
                var json = command.Get(uri);
                List<TModel> list = new List<TModel>();
                var view = (json.FromJson<ViewResult<TModel, TKey, TRev>>());
                list = view.GetList().ToList();
                return list;
            }
            catch (Exception ex)
            {
                "An exception occurred trying to retrieve all documents of type {0} at {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository>(
                        typeof(TModel).FullName,
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual void Save<TModel>(TModel model)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>()
                .Key(model.Id);

            try
            {
                var body = model.ToJson();
                var command = new CouchCommand(_configuration);
                var updatedJSON = command.Put(uri, body);
                var updated = updatedJSON.FromJson<SaveResponse>();
                model.UpdateRevision(updated.Revision);
            }
            catch (Exception ex)
            {
                "An exception occurred trying to save a document of type {0} at {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository>(
                        typeof(TModel).FullName,
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual void Save<TModel>(IEnumerable<TModel> list)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>().BulkInsert();
            
            try
            {
                var documentList = new BulkPersist<TModel, TKey, TRev>(true, false, list);
                var command = new CouchCommand(_configuration);
                var body = documentList.ToJson();
                var updatedJson = command.Post(uri, body);
                var updated = updatedJson.FromJson<SaveResponse[]>();
                list
                    .ToList()
                    .ForEach(x =>
                                 {
                                     var update = updated.FirstOrDefault(y => y.Id == x.Id.ToString());
                                     if (update != null)
                                         x.UpdateRevision(update.Revision);
                                 });
            }
            catch (Exception ex)
            {
                "An exception occurred trying to save a document of type {0} at {1}. \r\n\t {2}"
                    .ToError<IDocumentRepository>(
                        typeof(TModel).FullName,
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual void HandleUpdates<TModel>(int since, Action<ChangeRecord> onUpdate, AsyncCallback updatesInterrupted)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var command = new CouchCommand(_configuration);
            Action<CouchURI, int, Action<ChangeRecord>> proxy = command.GetContinuousResponse;
            proxy.BeginInvoke(BaseURI<TModel>(), since, onUpdate, updatesInterrupted, null);
            _continuousUpdateCommands[typeof(TModel)] = command;
        }

        public virtual void StopChangeStreaming<TModel>()
        {
            CouchCommand command;
            var key = typeof (TModel);
            if(_continuousUpdateCommands.TryGetValue(key, out command))
            {
                command.StopContinousResponse();
                _continuousUpdateCommands.TryRemove(key, out command);
            }
        }

        public BaseDocumentRepository(ICouchConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected BaseDocumentRepository(string configurationName)
        {
            _configuration = ObjectFactory.GetNamedInstance<ICouchConfiguration>(configurationName);
        }

        public void Dispose()
        {

        }
    }
}