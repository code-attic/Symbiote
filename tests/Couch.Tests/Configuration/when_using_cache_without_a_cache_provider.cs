﻿using System;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Couch.Config;
using Symbiote.Core.Cache;
using Symbiote.StructureMapAdapter;
using Symbiote.Couch;
using StructureMap;

namespace Couch.Tests.Configuration
{
    public class when_using_cache_without_a_cache_provider : with_couch_configuration
    {
        private static Exception exception;

        private Because of = () =>
                                 {
                                     ObjectFactory.EjectAllInstancesOf<ICacheProvider>();

                                     exception = Catch.Exception(() => Assimilate.Core<StructureMapAdapter>().Couch( x => x.Https().Port( 1234 ).Server( "couchdb" ).TimeOut( 1000 ).Cache() ));
                                 };

        private It should_produce_Couch_configuration_exception = () => exception.ShouldBe(typeof(CouchConfigurationException));
        private It should_have_clear_exception_message = () => exception.Message.ShouldEqual("You must have an implementation of ICacheProvider configured to use caching in Couch. Consider referencing Symbiote.Eidetic and adding the .Eidetic() call before this in your assimilation to utilize memcached or memcachedb as the cache provider for Couch.");
    }
}