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
    public abstract class BaseDocumentRepository
        : BaseCouchDbController, IDocumentRepository
    {
        protected ConcurrentDictionary<Type, ICouchCommand> _continuousUpdateCommands =
            new ConcurrentDictionary<Type, ICouchCommand>();
        
        public virtual void DeleteDocument<TModel>(object id)
            where TModel : class, IHandleJsonDocumentId, IHandleJsonDocumentRevision
        {
            var doc = Get<TModel>(id);

            if(doc != null)
            {
                var uri = BaseURI<TModel>();
                try
                {
                    var command = _commandFactory.GetCommand();
                    uri = uri.KeyAndRev(doc.GetIdAsJson(), doc.GetRevAsJson());
                    command.Delete(uri);
                }
                catch (Exception ex)
                {
                    "An exception occurred trying to delete a document of type {0} with id {1} at {2}. \r\n\t {3}"
                        .ToError<IDocumentRepository>(typeof(TModel).FullName, id.ToString(), uri.ToString(), ex);
                    throw;
                }
            }
        }

        public virtual void DeleteDocument<TModel>(object id, object rev)
            where TModel : class, IHandleJsonDocumentId, IHandleJsonDocumentRevision
        {
            var uri = BaseURI<TModel>();
            try
            {
                var command = _commandFactory.GetCommand();
                uri = uri.KeyAndRev(id, rev);
                command.Delete(uri);
            }
            catch (Exception ex)
            {
                "An exception occurred trying to delete a document of type {0} with id {1} at {2}. \r\n\t {3}"
                    .ToError<IDocumentRepository>(typeof(TModel).FullName, id.ToString(), uri.ToString(), ex);
                throw;
            }
        }

        public virtual TModel Get<TModel>(object id, object revision)
            where TModel : class, IHandleJsonDocumentId, IHandleJsonDocumentRevision
        {
            var uri = BaseURI<TModel>().KeyAndRev(id, revision);
            
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
                    .ToError<IDocumentRepository>(
                        typeof(TModel).FullName, 
                        id.ToString(), 
                        revision.ToString(),
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual TModel Get<TModel>(object id)
            where TModel : class, IHandleJsonDocumentId, IHandleJsonDocumentRevision
        {
            var uri = BaseURI<TModel>().Key(id);
            
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
                    .ToError<IDocumentRepository>(
                        typeof(TModel).FullName,
                        id.ToString(),
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual IList<TModel> GetAll<TModel>()
            where TModel : class, IHandleJsonDocumentId, IHandleJsonDocumentRevision
        {
            var uri = BaseURI<TModel>()
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
                    .ToError<IDocumentRepository>(
                        typeof(TModel).FullName,
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual IList<TModel> GetAll<TModel>(int pageSize, int pageNumber)
            where TModel : class, IHandleJsonDocumentId, IHandleJsonDocumentRevision
        {
            var uri = BaseURI<TModel>()
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
                    .ToError<IDocumentRepository>(
                        typeof(TModel).FullName,
                        uri.ToString(),
                        ex);
                throw;
            }
        }

        public virtual void Save<TModel>(TModel model)
            where TModel : class, IHandleJsonDocumentId, IHandleJsonDocumentRevision
        {
            var uri = BaseURI<TModel>()
                .Key(model.GetIdAsJson());

            try
            {
                var body = model.ToJson(false);
                var command = _commandFactory.GetCommand();
                var updatedJSON = command.Put(uri, body);
                var updated = updatedJSON.FromJson<SaveResponse>();
                model.UpdateRevFromJson(updated.Revision.ToJson(false));
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

        public virtual void SaveAll<TModel>(IEnumerable<TModel> list)
            where TModel : class, IHandleJsonDocumentId, IHandleJsonDocumentRevision
        {
            var uri = BaseURI<TModel>().BulkInsert();
            
            try
            {
                var documentList = new BulkPersist<TModel>(true, false, list);
                var command = _commandFactory.GetCommand();
                var body = documentList.ToJson(false);
                var updatedJson = command.Post(uri, body);
                var updated = updatedJson.FromJson<SaveResponse[]>();
                list
                    .ToList()
                    .ForEach(x =>
                                 {
                                     var update = updated.FirstOrDefault(y => y.Id.ToJson() == x.GetIdAsJson());
                                     if (update != null)
                                         x.UpdateRevFromJson(update.Revision);
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
            where TModel : class, IHandleJsonDocumentId, IHandleJsonDocumentRevision
        {
            var command = _commandFactory.GetCommand();
            Action<CouchUri, int, Action<ChangeRecord>> proxy = command.GetContinuousResponse;
            proxy.BeginInvoke(BaseURI<TModel>(), since, onUpdate, updatesInterrupted, null);
            _continuousUpdateCommands[typeof(TModel)] = command;
        }

        public virtual void StopChangeStreaming<TModel>()
            where TModel : class, IHandleJsonDocumentId, IHandleJsonDocumentRevision
        {
            ICouchCommand command;
            var key = typeof (TModel);
            if(_continuousUpdateCommands.TryGetValue(key, out command))
            {
                command.StopContinousResponse();
                _continuousUpdateCommands.TryRemove(key, out command);
            }
        }

        protected BaseDocumentRepository(ICouchConfiguration configuration, ICouchCommandFactory commandFactory)
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