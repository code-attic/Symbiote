using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Utility;
using Symbiote.Messaging;

namespace Messaging.Tests.Pipes.Functional
{
    public interface ITransformProvider
    {
        IEnvelope<TMessage> Transform<TMessage>( string pipeline, IEnvelope<TMessage> envelope);
    }

    public interface IPipelineRegistry
    {
        IPipelineRegistry DefineForEndpoint();
    }

    public class when_creating_pipeline
    {
        protected static string Message = "hi";
        protected static string Result = "";

        private Because of = () =>
        {
            var pipe =
                Symbiote.Core.Utility.Pipeline
                    .Start<string, byte[]>( x => Encoding.UTF8.GetBytes( x ) )
                    .Then<string, byte[], IEnumerable, byte[]>((r,x) => x)
                    .Then( x =>
                    {
                        var t = x[1];
                        x[1] = x[0];
                        x[0] = t;
                        return x;
                    } )
                    .Then( x => Encoding.UTF8.GetString( x ) );

            Result = pipe( Message );
        };

        private It should_return_the_original_string_in_reverse = () => Result.ShouldEqual( "ih" );
    }
}
