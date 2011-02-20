using System;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Saga;

namespace Core.Tests.DecisionTree
{
    public class ThingySaga : Saga<Thingy>
    {
        public static bool Hit1 { get; set; }
        public static bool Hit2 { get; set; }
        public static bool Hit3 { get; set; }

        public override Action<StateMachine<Thingy>> Setup()
        {
            return x => 
                       { 
                           x.When( t => t.Flag1 )
                               .On<ProcessThingy>( t => 
                               {
                                   Hit1 = true;
                                   return e => e.Acknowledge();
                               } );

                           x.When(t => t.Flag2)
                               .On<ProcessThingy>(t =>
                               {
                                   Hit2 = true;
                                   return e => e.Acknowledge();
                               });

                           x.When(t => t.Flag3)
                               .On<ProcessThingy>(t =>
                               {
                                   Hit3 = true;
                                   return e => e.Acknowledge();
                               });
                       };
        }

        public ThingySaga( StateMachine<Thingy> stateMachine ) : base( stateMachine )
        {
        }
    }
}