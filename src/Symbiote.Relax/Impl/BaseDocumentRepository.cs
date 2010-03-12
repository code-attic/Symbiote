using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
            _repository.StopChangeStreaming();
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
        protected bool _preAuth;
        private bool _pollForChanges;
        protected ICouchConfiguration _configuration;
        protected Dictionary<string, bool> _databaseExists = new Dictionary<string, bool>();
        protected ReaderWriterLockSlim _databaseExistsLock = new ReaderWriterLockSlim();

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
            _databaseExistsLock.EnterReadLock();
            try
            {
                shouldCheckCouch = !_databaseExists.TryGetValue(database, out dbCreated);
            }
            finally
            {
                _databaseExistsLock.ExitReadLock();
            }
            if(shouldCheckCouch && !dbCreated)
            {
                var response = Get(baseURI);
                dbCreated = !string.IsNullOrEmpty(response) && !response.StartsWith("{\"error\"");
            }
            if(!dbCreated)
            {
                Put(baseURI);
                _databaseExistsLock.EnterWriteLock();
                try
                {
                    _databaseExists[database] = true;
                }
                finally
                {
                    _databaseExistsLock.ExitWriteLock();
                }
            }
        }

        public virtual void CreateDatabase<TModel>()
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            Put(BaseURI<TModel>());
            var database = _configuration.GetDatabaseNameForType<TModel>();
            _databaseExistsLock.EnterWriteLock();
            try
            {
                _databaseExists[database] = true;
            }
            finally
            {
                _databaseExistsLock.ExitWriteLock();
            }
        }

        public virtual void DeleteDatabase<TModel>()
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            Delete(BaseURI<TModel>());
            var database = _configuration.GetDatabaseNameForType<TModel>();
            _databaseExistsLock.EnterWriteLock();
            try
            {
                _databaseExists[database] = false;
            }
            finally
            {
                _databaseExistsLock.ExitWriteLock();
            }
        }

        public virtual bool DatabaseExists<TModel>()
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var response = Get(BaseURI<TModel>());
            var exists = !string.IsNullOrEmpty(response) && !response.StartsWith("{\"error\"");
            _databaseExistsLock.EnterWriteLock();
            var database = _configuration.GetDatabaseNameForType<TModel>();
            try
            {
                _databaseExists[database] = exists;
            }
            finally
            {
                _databaseExistsLock.ExitWriteLock();
            }
            return exists;
        }

        protected virtual string Post(CouchURI uri)
        {
            return GetResponse(uri, "POST", "");
        }

        protected virtual string Post(CouchURI uri, string body)
        {
            return GetResponse(uri, "POST", body);
        }

        protected virtual string Put(CouchURI uri)
        {
            return GetResponse(uri, "PUT", "");
        }

        protected virtual string Put(CouchURI uri, string body)
        {
            return GetResponse(uri, "PUT", body);
        }

        protected virtual string Get(CouchURI uri)
        {
            return GetResponse(uri, "GET", "");
        }

        protected virtual string Delete(CouchURI uri)
        {
            return GetResponse(uri, "DELETE", "");
        }

        protected virtual string GetResponse(CouchURI uri, string method, string body)
        {
            var request = WebRequest.Create(uri.ToString());
            request.Method = method;
            request.Timeout = _configuration.TimeOut;
            request.PreAuthenticate = _preAuth;

            if (!string.IsNullOrEmpty(body))
            {
                var bytes = UTF8Encoding.UTF8.GetBytes(body);
                request.ContentType = "application/json";
                request.ContentLength = bytes.Length;

                var writer = request.GetRequestStream();
                writer.Write(bytes, 0, bytes.Length);
                writer.Close();
            }

            var result = "";

            try
            {
                var response = request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                    response.Close();
                }
            }
            catch (Exception e)
            {
            }
            return result;
        }

        protected virtual void GetContinuousResponse<TModel>(int since, Action<ChangeRecord> callback)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>().Changes(Feed.Continuous, since);
            var request = WebRequest.Create(uri.ToString());
            request.Method = "GET";
            request.Timeout = int.MaxValue;
            request.PreAuthenticate = _preAuth;

            var result = "";

            try
            {
                var response = request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    while (_pollForChanges)
                    {
                        result = reader.ReadLine();
                        if (!string.IsNullOrEmpty(result))
                        {
                            var change = result.FromJson<ChangeRecord>();
                            change.Document = GetResponse(BaseURI<TModel>().Key(change.Id), "GET", "");
                            callback.BeginInvoke(change, null, null);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                callback = null;
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
                var json = Get(uri);
                return new List<string>(json.FromJson<string[]>());
            }
        }

        public virtual TModel Get<TModel>(TKey id, TRev revision)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>().KeyAndRev(id, revision);
            var json = Get(uri);

            TModel model = default(TModel);
            try
            {
                model = json.FromJson<TModel>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return model;
        }

        public virtual TModel Get<TModel>(TKey id)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>().Key(id);
            var json = Get(uri);

            TModel model = default(TModel);
            try
            {
                model = json.FromJson<TModel>();
            }
            catch (Exception e)
            {
            }
            return model;
        }

        public virtual IList<TModel> GetAll<TModel>()
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>()
                .ListAll()
                .IncludeDocuments();
            var json = Get(uri);
            
            List<TModel> list = new List<TModel>();
            try
            {
                var view = (json.FromJson<ViewResult<TModel, TKey, TRev>>());
                list = view.GetList().ToList();
            }
            catch (Exception e)
            {
            }
            return list;
        }

        public virtual IList<TModel> GetAll<TModel>(int pageSize, int pageNumber)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>()
                .ListAll()
                .IncludeDocuments()
                .Skip((pageNumber - 1)*pageSize)
                .Limit(pageSize);

            var json = Get(uri);
            List<TModel> list = new List<TModel>();
            try
            {
                var view = (json.FromJson<ViewResult<TModel, TKey, TRev>>());
                list = view.GetList().ToList();
            }
            catch (Exception e)
            {
            }
            return list;
        }

        public virtual void Save<TModel>(TModel model)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>()
                .Key(model.Id);

            var body = model.ToJson();
            var updatedJSON = Put(uri, body);
            var updated = updatedJSON.FromJson<SaveResponse>();
            model.UpdateRevision(updated.Revision);
        }

        public virtual void Save<TModel>(IEnumerable<TModel> list)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            var uri = BaseURI<TModel>().BulkInsert();

            var documentList = new BulkPersist<TModel, TKey, TRev>(true, false, list);

            var body = documentList.ToJson();
            var updatedJSON = Post(uri, body);
            var updated = updatedJSON.FromJson<SaveResponse[]>();

            list
                .ToList()
                .ForEach(x =>
                             {
                                 var update = updated.FirstOrDefault(y => y.Id == x.Id.ToString());
                                 if (update != null)
                                     x.UpdateRevision(update.Revision);
                             });
        }

        public virtual void HandleUpdates<TModel>(int since, Action<ChangeRecord> onUpdate, AsyncCallback updatesInterrupted)
            where TModel : class, ICouchDocument<TKey, TRev>
        {
            _pollForChanges = true;
            Action<int, Action<ChangeRecord>> proxy = GetContinuousResponse<TModel>;
            proxy.BeginInvoke(since, onUpdate, updatesInterrupted, null);
        }

        public virtual void StopChangeStreaming()
        {
            _pollForChanges = false;
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