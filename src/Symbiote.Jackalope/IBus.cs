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

using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Impl;
using Symbiote.Jackalope.Impl;
using Symbiote.Jackalope.Impl.Endpoint;
using Symbiote.Jackalope.Impl.Routes;

namespace Symbiote.Jackalope
{
    public interface IBus : IDisposable
    {
        IQueueStreamCollection QueueStreams { get; }
        void AddEndPoint(Action<IEndPoint> endpointConfiguration);
        void AutoRouteFromSource<T>(IObservable<T> source)
            where T : class;
        void BindQueue(string queueName, string exchangeName, params string[] routingKeys);
        bool ClearQueue(string queueName);
        void DefineRouteFor<T>(Action<IDefineRoute<T>> routeDefinition);
        void DestroyQueue(string queueName);
        void DestroyExchange(string exchangeName);
        void Subscribe(string queueName);
        void Send<T>(T body)
            where T : class;
        void Send<T>(string exchangeName, T body)
            where T : class;
        void Send<T>(string exchangeName, T body, string routingKey)
            where T : class;
        void Unsubscribe(string queueName);
    }
}