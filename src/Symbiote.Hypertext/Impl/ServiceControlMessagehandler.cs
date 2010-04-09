using System;
using System.Collections.Generic;
using Symbiote.Daemon;
using Symbiote.Jackalope;

namespace Symbiote.Telepathy
{
    public class ServiceControlMessagehandler : IMessageHandler<ServiceControlMessage>
    {
        private IDaemon _service;
        private Dictionary<ServiceAction, Action<IDaemon, ServiceControlMessage>> _controlActions
            = new Dictionary<ServiceAction, Action<IDaemon, ServiceControlMessage>>()
                  {
                      {ServiceAction.Start, 
                          (x,y) =>
                              {
                                  x.Start();
                              }},
                      {ServiceAction.Stop, 
                          (x,y) =>
                              {
                                  x.Stop();
                              }},
                      {ServiceAction.NoOp, 
                          (x,y) =>
                              {
                            
                              }},
                  };

        public void Process(ServiceControlMessage message, IRespond respond)
        {
            var reply = new ServiceControlResponse();
            try
            {
                _controlActions[message.Action](_service, message);
                reply.Success = true;
                respond.Reply(reply);
            }
            catch (Exception ex)
            {
                reply.ExceptionOccurred = true;
                reply.Exceptions.Add(ex);
                respond.Reply(reply);
                throw;
            }
        }

        public ServiceControlMessagehandler(IDaemon service)
        {
            _service = service;
        }
    }
}