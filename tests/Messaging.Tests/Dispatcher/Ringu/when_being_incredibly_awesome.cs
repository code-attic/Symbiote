using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Utility;
using Symbiote.Messaging.Impl.Actors;
using Symbiote.Messaging.Impl.Envelope;
using Symbiote.Messaging.Impl.Transform;
using Symbiote.Core.Extensions;
using Symbiote.Core.Serialization;
using Symbiote.Messaging;
using Symbiote.StructureMap;

namespace Messaging.Tests.Dispatcher.Ringu
{
    public class when_being_incredibly_awesome
    {
        protected static VolatileRingBuffer buffer { get; set; }
        protected static ConcurrentBag<Message> Results { get; set; }
        protected static Stopwatch watch;
        protected static int Capacity = 100000;
        protected static int Qty = 1000000;

        private Because of = () =>
        {
            Results = new ConcurrentBag<Message>();
            buffer = new VolatileRingBuffer(Capacity);

            buffer.AddTransform(x =>
            {
                (x as Message).Text = "Ring";
                return x;
            });
            buffer.AddTransform(x =>
            {
                Results.Add(x as Message);
                (x as Message).Text = "Done";
                return x;
            });

            watch = Stopwatch.StartNew();
            buffer.Start();
            Enumerable.Range(0, Qty).ForEach(x =>
            {
                var message = new Message("Nothing");
                buffer.Write(message);
            } );

            Thread.Sleep( 120 );

            buffer.Stop();
            watch.Stop();
        };

        private It should_have_transformed_last_item = () => (buffer.Ring[2] as Message).Text.ShouldEqual("Done");
        private It should_have_transformed_first_item = () => (buffer.Ring[0] as Message).Text.ShouldEqual("Done");
        private It should_have_created_a_million_results = () => Results.Count.ShouldEqual(Qty);
        private It should_run_in_under_a_second = () => watch.ElapsedMilliseconds.ShouldBeLessThan(1);
    }

    [DataContract]
    public class Message
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }
        [DataMember(Order = 2)]
        public string Text { get; set; }
        [DataMember(Order = 3)]
        public long Created { get; set; }
        [DataMember(Order = 4)]
        public int Number { get; set; }

        public Message(string text)
        {
            Text = text;
        }
    }

    public class with_buffer_array_setup
    {
        protected static IAgency agency { get; set; }
        private Establish context = () =>
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Messaging();

            agency = Assimilate.GetInstanceOf<IAgency>();
        };
    }

    public class MyBuilder
    {
        public string Id { get; set; }
        public string Content { get; set; }

        public void AddSegment(string segment)
        {
            Content += segment;
        }

        public MyBuilder()
        {
            Content = "";
        }
    }

    public class SegmentMessage
    {
        public string Content { get; set; }

        public SegmentMessage() { }
    }

    public class BuilderKeyAccessor : IKeyAccessor<MyBuilder>
    {
        public string GetId(MyBuilder actor)
        {
            return actor.Id;
        }

        public void SetId<TKey>(MyBuilder actor, TKey id)
        {
            actor.Id = id.ToString();
        }
    }

    public class MyBuilderFactory
        : IActorFactory<MyBuilder>
    {
        public MyBuilder CreateInstance<TKey>(TKey id)
        {
            return new MyBuilder();
        }
    }

    public class SegmentMessageHandler
        : IHandle<MyBuilder, SegmentMessage>
    {
        public void Handle(MyBuilder actor, IEnvelope<SegmentMessage> envelope)
        {
            actor.AddSegment(envelope.Message.Content);
        }
    }

    public class when_having_fun_with_word_building
        : with_buffer_array_setup
    {
        protected static VolatileRingBufferArray bufferArray { get; set; }
        protected static List<MyBuilder> builders { get; set; }

            Because of = () =>
            {
                builders = new List<MyBuilder>();
                bufferArray = new VolatileRingBufferArray(10000, 1000);
                var messages = new List<string[]>
                {
                    new[] { "first", "t" },
                    new[] { "second", "i" },
                    new[] { "third", "h" },
                    new[] { "fourth", "t" },
                    new[] { "fifth", "w" },
                    new[] { "sixth", "e" },
                    new[] { "sixth", "n" },
                    new[] { "second", "s" },
                    new[] { "third", "o" },
                    new[] { "fourth", "h" },
                    new[] { "fifth", "o" },
                    new[] { "first", "h" },
                    new[] { "first", "i" },
                    new[] { "second", " " },
                    new[] { "third", "w" },
                    new[] { "fourth", "e" },
                    new[] { "fifth", "r" },
                    new[] { "sixth", "d" },
                    new[] { "first", "s" },
                    new[] { "third", " " },
                    new[] { "fourth", " " },
                    new[] { "fifth", "l" },
                    new[] { "sixth", "s" },
                    new[] { "first", " " },
                    new[] { "fifth", "d" },
                    new[] { "sixth", "." },
                    new[] { "fifth", " " },
                };

                var handler = new SegmentMessageHandler();

                bufferArray.AddTransform(x =>
                {
                    var envelope = x as IEnvelope<SegmentMessage>;
                    var actor = agency.GetAgentFor<MyBuilder>().GetActor(envelope.CorrelationId);
                    handler.Handle( actor, envelope );
                    return null;
                });

                bufferArray.Start();

                messages.ForEach( x =>
                {
                    var envelope = new Envelope<SegmentMessage>( new SegmentMessage()
                    {
                        Content = x[1]
                    } );
                    envelope.CorrelationId = x[0];
                    bufferArray.Write( x[0], envelope );
                } );
                
                Thread.Sleep( 100 );
                bufferArray.Stop();

                var agent = agency.GetAgentFor<MyBuilder>();
                builders = new[] { "first", "second", "third", "fourth", "fifth", "sixth" }
                    .Select( agent.GetActor )
                    .ToList();
                var count = builders.Count;
            };

        private It should_not_be_a_stupid_piece_of_crap = () => 
            builders.Select(x => x.Content).ShouldContain( "this ", "is ", "how ", "the ", "world ", "ends." );
    }

    public class Account
    {
        public string Id { get; set; }
        public decimal Balance { get; set; }
        public List<decimal > TransactionHistory { get; set; }

        public void ChangeBalanceBy(decimal amount)
        {
            Balance += amount;
            TransactionHistory.Add( amount );
        }

        public Account()
        {
            TransactionHistory = new List<decimal>();
        }
    }

    public class Transaction
    {
        public string AccountId { get; set; }
        public decimal Amount { get; set; }
        public Transaction() {}

        public Transaction( string accountId, decimal amount )
        {
            AccountId = accountId;
            Amount = amount;
        }
    }

    public class AccountKeyAccessor : IKeyAccessor<Account>
    {
        public string GetId(Account actor)
        {
            return actor.Id;
        }

        public void SetId<TKey>(Account actor, TKey id)
        {
            actor.Id = id.ToString();
        }
    }

    public class AcountFactory
        : IActorFactory<Account>
    {
        public Account CreateInstance<TKey>(TKey id)
        {
            return new Account() { Id = id.ToString() };
        }
    }

    public class TransactionHandler
        : IHandle<Account, Transaction>
    {
        public void Handle( Account actor, IEnvelope<Transaction> envelope )
        {
            actor.ChangeBalanceBy( envelope.Message.Amount );
        }
    }

    public class RingBufferList
    {
        public ExclusiveConcurrentDictionary<string, VolatileRingBuffer> Buffers { get; set; }
        public Func<object, object> Callback { get; set; }

        public void Write<T>(string id, T value)
        {
            var buffer = Buffers.ReadOrWrite( id, CreateBufferForActor );
            buffer.Write( value );
        }

        public VolatileRingBuffer CreateBufferForActor()
        {
            var buffer = new VolatileRingBuffer( 1000 );
            buffer.AddTransform( Callback );
            buffer.Start();
            return buffer;
        }

        public void Start()
        {
            Buffers.Values.ForEach( x => x.Start() );
        }

        public void Stop()
        {
            Buffers.Values.ForEach(x => x.Stop());
        }

        public RingBufferList( Func<object, object> callback )
        {
            Buffers = new ExclusiveConcurrentDictionary<string, VolatileRingBuffer>();
            Callback = callback;
        }
    }

    public class when_playing_bank_teller
        : with_buffer_array_setup
    {
        protected static VolatileRingBufferArray bufferArray { get; set; }
        //protected static RingBufferList bufferArray { get; set; }
        protected static List<Account> accounts { get; set; }
        protected static Stopwatch watch;
        protected static int TotalAccounts = 60;
        protected static int Transactions = 10000;

        Because of = () =>
        {
            accounts = new List<Account>();
            bufferArray = new VolatileRingBufferArray(TotalAccounts, 5000);
            var handler = new TransactionHandler();
            
            bufferArray.AddTransform(x =>
            {
                var envelope = x as IEnvelope<Transaction>;
                var actor = agency.GetAgentFor<Account>().GetActor(envelope.CorrelationId);
                handler.Handle(actor, envelope);
                return null;
            });

            bufferArray.Start();
            watch = Stopwatch.StartNew();
            //create 10,000 1.00 transactions for each 1000 accounts
            var txList = Enumerable.Range(0, TotalAccounts)
                .Select( x =>
                {
                    var correlationId = x.ToString();
                    var env = new Envelope<Transaction>( new Transaction( correlationId, 1.00m ) );
                    env.CorrelationId = correlationId;
                    return env;
                } );

            txList.AsParallel().ForAll( x =>
            {
                for(var i = 0; i < Transactions; i ++)
                {
                    bufferArray.Write( x.CorrelationId, x );
                }
            } );

            Thread.Sleep( 100 );

            bufferArray.Stop();
            watch.Stop();

            var agent = agency.GetAgentFor<Account>();
            accounts = Enumerable
                .Range( 0, TotalAccounts )
                .Select( x => x.ToString() )
                .Select( agent.GetActor )
                .Where( x => x.Balance == Transactions)
                .ToList();
        };
        
        private It should_create_all_accounts_with_proper_balance = () => 
            accounts.Count.ShouldEqual( TotalAccounts );

        private It should_take_less_than_1_second = () => 
            watch.ElapsedMilliseconds.ShouldBeLessThan( 601 );
    }

    public class when_playing_bank_teller_with_local_channel
        : with_buffer_array_setup
    {
        protected static List<Account> accounts { get; set; }
        protected static Stopwatch watch;
        protected static int TotalAccounts = 60;
        protected static int Transactions = 10000;
        protected static IBus bus;

        Because of = () =>
        {
            accounts = new List<Account>();
            bus = Assimilate.GetInstanceOf<IBus>();
            bus.AddLocalChannel( x => x.Named( "transaction" ).CorrelateBy<Transaction>( m => m.AccountId ) );

            watch = Stopwatch.StartNew();
            //create 10,000 1.00 transactions for each 1000 accounts
            var txList = Enumerable.Range(0, TotalAccounts)
                .Select(x =>
                {
                    var correlationId = x.ToString();
                    return new Transaction(correlationId, 1.00m);
                });

            txList.AsParallel().ForAll(x =>
            {
                for (var i = 0; i < Transactions; i++)
                {
                    bus.Publish( x );
                }
            });

            Thread.Sleep(150);

            watch.Stop();

            var agent = agency.GetAgentFor<Account>();
            accounts = Enumerable
                .Range(0, TotalAccounts)
                .Select(x => x.ToString())
                .Select(agent.GetActor)
                .ToList();
        };

        private It should_create_all_accounts_with_proper_balance = () =>
            accounts.All(x => x.Balance == (Transactions)).ShouldBeTrue();

        private It should_take_less_than_1_second = () =>
            watch.ElapsedMilliseconds.ShouldBeLessThan(1001);
    }
}
