(* 
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
*)

namespace Symbiote.Messaging

open System
open System.Reflection
open System.Linq
open System.Collections.Generic
open Symbiote.Core
open Symbiote.Core.DI
open Symbiote.Core.Extensions
open Symbiote.Messaging
open Symbiote.Messaging.Impl
open Symbiote.Messaging.Impl.Actors
open Symbiote.Messaging.Impl.Channels
open Symbiote.Messaging.Impl.Dispatch

[<System.Runtime.CompilerServices.Extension>]
module AssimilationExtensions =
    let referencePredicate (a: AssemblyName) = a.FullName.Contains("Symbiote.Messaging") 
    
    let assemblyPredicate (a: Assembly) = 
        let func = new Func<AssemblyName,bool>(referencePredicate)
        a.GetReferencedAssemblies().Any(func)

    let getHandlerTuple (handlerType: Type) =
        let messageType = handlerType.GetGenericArguments().[0]
        let dispatchInterface = typedefof<IDispatchMessage<_>>.MakeGenericType messageType
        let dispatchType = typedefof<MessageDispatcher<_>>.MakeGenericType messageType
        (dispatchInterface, dispatchType)

    let getActorHandlerTuple (handlerType: Type) =
        let messageType = handlerType.GetGenericArguments().[1]
        let actorType = handlerType.GetGenericArguments().[0]
        let dispatchInterface = typedefof<IDispatchMessage<_,_>>.MakeGenericType(actorType, messageType)
        let dispatchType = typedefof<MessageDispatcher<_,_>>.MakeGenericType(actorType, messageType)
        (dispatchInterface, dispatchType)

    let configuration(assimilate : IAssimilate) =
            assimilate.Dependencies(fun x ->
                x.Scan(fun s -> 
                    Array.toSeq (AppDomain.CurrentDomain.GetAssemblies())
                    |> Seq.filter assemblyPredicate
                    |> Seq.iter s.Assembly
            
                    s.ConnectImplementationsToTypesClosing typedefof<IHandle<_>>
                    s.ConnectImplementationsToTypesClosing typedefof<IHandle<_,_>>
                )
                x.For<IAgency>().Use<Agency>().AsSingleton() |> ignore
                x.For(typedefof<IAgent<_>>).Use(typedefof<DefaultAgent<_>>) |> ignore
                x.For(typedefof<IActorStore<_>>).Use(typedefof<NullActorStore<_>>) |> ignore
                x.For(typedefof<IActorFactory<_>>).Use(typedefof<DefaultActorFactory<_>>) |> ignore
                x.For(typedefof<IChannelFactory<_>>).Use(typedefof<DefaultChannelFactory<_>>) |> ignore
                x.For<IActorCache>().Use<InMemoryActorCache>() |> ignore
                x.For<IChannel>().Use<LocalChannel>() |> ignore
                x.For<IChannelManager>().Use<ChannelManager>() |> ignore
                x.For<IBus>().Use<Bus>() |> ignore
                x.For<IDispatcher>().Use<DispatchManager>() |> ignore
            ) |> ignore

            let simpleDispatchInterface = typedefof<IDispatchMessage>

            let handlers = 
                Assimilate.Assimilation.DependencyAdapter.RegisteredPluginTypes
                |> Seq.filter (fun x -> typedefof<IHandle>.IsAssignableFrom(x) || x.IsAssignableFrom(typedefof<IHandle>))

            Assimilate.Dependencies(fun d ->
                handlers
                |> Seq.collect (fun x ->
                    x.GetInterfaces()
                    |> Seq.filter (fun i -> i.Name = "IHandle`1" || i.Name = "IHandle`2")
                    |> Seq.map (fun i ->
                        match i.Name with
                        | "IHandle`1" -> getHandlerTuple i
                        | "IHandle`2" -> getActorHandlerTuple i
                    )
                )
                |> Seq.iter (fun pair -> 
                    match pair with
                    | i, t ->  
                        d.For(i).Use(t) |> ignore
                        d.For(simpleDispatchInterface).Use(t) |> ignore
                    ) |> ignore
            ) |> ignore

            assimilate

    [<System.Runtime.CompilerServices.Extension>]
    let Messaging(assimilate: IAssimilate) =
        configuration assimilate


    type IAssimilate with
        member x.Messaging(assimilate : IAssimilate) =
            configuration assimilate
