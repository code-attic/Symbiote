using System;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Actors;
using Symbiote.Messaging.Impl.Saga;
using Symbiote.StructureMap;

namespace Messaging.Tests.FSM
{
    public class Barrista
    {
        public bool WaitingForOrder { get; set; }
        public bool MakingOrder { get; set; }
        public int CurrentOrder { get; set; }

    }

    public class CashierKeyAccessor
        : IKeyAccessor<Cashier>
    {
        public string GetId( Cashier actor )
        {
            return actor.Name;
        }

        public void SetId<TKey>( Cashier actor, TKey id )
        {
            actor.Name = id.ToString();
        }
    }

    public class Cashier
    {
        public string Name { get; set; }
        public bool Available { get; set; }
        public bool WaitingOnCustomer { get; set; }
        public bool WaitingForPayment { get; set; }
        public int CurrentOrder { get; set; }

        public void TakeNewOrder(string customer, string item, string size)
        {

            Available = false;
            WaitingOnCustomer = true;
        }

        public void AddOrderItem(string item, string size)
        {

        }

        public void FinishOrder()
        {

            WaitingOnCustomer = false;
            WaitingForPayment = true;
        }

        public void ProcessPayment()
        {

            Available = true;
            WaitingOnCustomer = false;
        }

        public Cashier()
        {
            Available = true;
        }
    }

    public class Customer
    {
        public bool WaitingOnOrder { get; set; }


    }

    public class NewOrder
    {
        public string Cashier { get; set; }
        public string Customer { get; set; }
        public string Item { get; set; }
        public string Size { get; set; }
    }

    public class OrderAnotherItem
    {
        public string Cashier { get; set; }
        public string Item { get; set; }
        public string Size { get; set; }
    }

    public class RequestOrderPayment
    {
        public string Customer { get; set; }
        public decimal Total { get; set; }
    }

    public class PayForOrder
    {
        
    }

    public class CompleteOrder
    {
        
    }

    public class Hi
    {
        public string To { get; set; }
    }

    public class CashierSaga :
        Saga<Cashier>
    {
        public override Action<StateMachine<Cashier>> Setup()
        {
            return ( machine ) =>
            {
                machine
                    .When( cashier => cashier.Available )
                    .On<NewOrder>(
                        ( cashier, envelope ) => cashier.TakeNewOrder( 
                            envelope.Message.Customer, 
                            envelope.Message.Item, 
                            envelope.Message.Size ) );

                machine
                    .When( cashier => cashier.WaitingForPayment )
                    .On<OrderAnotherItem>(
                        ( cashier, envelope ) => cashier.AddOrderItem( 
                            envelope.Message.Item, 
                            envelope.Message.Size ) )
                    .On<CompleteOrder>(
                        ( cashier, envelope ) => cashier.FinishOrder()
                    );

                machine
                    .Unconditionally()
                    .On<Hi>( cashier => { } );
            };
        }

        public CashierSaga( StateMachine<Cashier> stateMachine ) : base( stateMachine ) {}
    }

    public abstract class with_context
    {
        public static IBus Bus { get; set; }
        public static IAgency Agency { get; set; }
        public static IAgent<Cashier> Cashiers { get; set; }

        private Establish context = () =>
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Messaging();

            Bus = Assimilate.GetInstanceOf<IBus>();
            Bus.AddLocalChannel( x =>
            {
                x.CorrelateBy<NewOrder>( m => m.Cashier );
                x.CorrelateBy<Hi>( m => m.To );
            } );
            Agency = Assimilate.GetInstanceOf<IAgency>();
            Cashiers = Agency.GetAgentFor<Cashier>();
        };    
    }

    public class when_creating_an_fsm
        : with_context
    {
        public static Cashier cashier { get; set; }

        private Because of = () =>
        {
            Bus.Publish( new NewOrder()
            {
                Cashier = "Um",
                Customer = "test",
                Item = "Chugolade",
                Size = "Grande"
            } );

            cashier = Cashiers.GetActor( "Um" );

            Thread.Sleep(5);
        };

        private It should_have_changed_state_to_waiting_on_customer = () => cashier.WaitingOnCustomer.ShouldBeTrue();
        private It should_not_be_available_for_new_orders = () => cashier.Available.ShouldBeFalse();
    }
}
