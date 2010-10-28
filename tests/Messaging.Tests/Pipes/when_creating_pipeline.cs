using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Messaging.Tests.Pipes
{
    public interface IPipeline
    {
        
    }

    public interface IPipe
    {
        object ProcessAnonymous( object input );
    }

    public interface IPipe<TIn, TOut>
    {
        TOut Process( TIn input );
        IPipe<TIn, TNewOut> Add<TNewOut>( IPipe<TOut, TNewOut> pipe );
    }

    public abstract class Pipeline<TIn, TOut>
        : IPipe<TIn, TOut>
    {
        public TOut Process( TIn input )
        {
            
        }

        public IPipe<TIn, TNewOut> Add<TNewOut>( IPipe<TOut, TNewOut> pipe )
        {
            
        }
    }



    public class Pipeline
    {
        protected List<IPipe> Steps { get; set; }

        

        public Pipeline()
        {
            Steps = new List<IPipe>();
        }
    }

    class when_creating_pipeline
    {
    }
}
