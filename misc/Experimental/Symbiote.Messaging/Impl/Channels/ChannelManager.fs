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

namespace Symbiote.Messaging.Impl.Channels

open System
open System.Collections.Concurrent
open Symbiote.Messaging
open Symbiote.Messaging.Impl.Channels
open Microsoft.Practices.ServiceLocation

type ChannelManager() =
    let channelTypes = new ConcurrentDictionary<string, Type>()
    let channels = new ConcurrentDictionary<string, IChannel>()
    let channelFactories = new ConcurrentDictionary<Type, IChannelFactory>()

    member x.ChannelTypes
        with get() = channelTypes
    member x.Channels
        with get() = channels
    member x.ChannelFactories
        with get() = channelFactories

    member x.GetChannelFactory (channelName: string) =
        let typeof = typedefof<LocalChannel>
        let channelType = x.ChannelTypes.GetOrAdd(channelName, typeof)
        match x.ChannelFactories.TryGetValue channelType with
        | true, factory -> factory
        | _ ->
            let factoryType = typedefof<IChannelFactory<_>>.MakeGenericType(channelType)
            let factory = ServiceLocator.Current.GetInstance(factoryType)
            let cast = factory :?> IChannelFactory
            x.ChannelFactories.GetOrAdd(factoryType, cast)

    interface IChannelManager with
        member x.GetChannel (channelName: string) =
            match x.Channels.TryGetValue(channelName) with
                | true, channel -> channel
                | _ -> 
                    let factory = x.GetChannelFactory channelName
                    let channel = factory.GetChannel()
                    x.Channels.TryAdd(channelName, channel) |> ignore
                    channel
