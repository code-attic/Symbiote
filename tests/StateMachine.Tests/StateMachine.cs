using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Microsoft.Practices.ServiceLocation;
using StateMachine.Tests;
using Symbiote.Core;
using Symbiote.StructureMap;

namespace StateMachine.Tests
{
    public interface IHaveState
    {

    }

    public interface ICorrelate
    {
        string CorrelationKey { get; set; }
    }

    public interface IState<TEvent>
        where TEvent : struct
    {
        bool Started { get; set; }
        bool InProcess { get; set; }
        bool Completed { get; set; }
    }

    public interface IDirect
    {
        IObservable<object> EventStream { get; set; }
        ISagaRegistry Registry { get; set; }

        IDirect For<TSaga, TMessage>(Action<TSaga, TMessage> workflow)
            where TSaga : class, IHaveState, ICorrelate
            where TMessage : ICorrelate;
    }

    public interface ISagaRegistry
    {
        TSaga GetSagaOf<TSaga>(string correlationId)
            where TSaga : class, IHaveState, ICorrelate;
    }

    public class InMemorySagaRegistry
        : ISagaRegistry
    {
        protected ConcurrentDictionary<string, IHaveState> sagas { get; set; }

        public TSaga GetSagaOf<TSaga>(string correlationId)
            where TSaga : class, IHaveState, ICorrelate
        {
            return sagas.GetOrAdd(correlationId, CreateSagaOf<TSaga>(correlationId)) as TSaga;
        }

        protected TSaga CreateSagaOf<TSaga>(string correlationId)
            where TSaga : class, IHaveState, ICorrelate
        {
            var saga = ServiceLocator.Current.GetInstance<TSaga>();
            saga.CorrelationKey = correlationId;
            return saga;
        }

        public InMemorySagaRegistry()
        {
            sagas = new ConcurrentDictionary<string, IHaveState>();
        }
    }

    public class Director
        : IDirect
    {
        public IObservable<object> EventStream { get; set; }
        public ISagaRegistry Registry { get; set; }

        public IDirect For<TSaga, TMessage>(Action<TSaga, TMessage> workflow)
            where TSaga : class, IHaveState, ICorrelate
            where TMessage : ICorrelate
        {
            EventStream
                .OfType<TMessage>()
                .Subscribe(m =>
                               {
                                   var saga = Registry.GetSagaOf<TSaga>(m.CorrelationKey);
                                   workflow(saga, m);
                               });
            return this;
        }

        public Director(IObservable<object> observable)
        {
            EventStream = observable;
            Registry = ServiceLocator.Current.GetInstance<ISagaRegistry>();
        }
    }

    public class StateMachine
        : IHaveState, ICorrelate
    {
        public string CorrelationKey { get; set; }

        public List<string> Events { get; set; }

        public void IShouldStartYawlsy(Start startmessage)
        {
            Events.Add(startmessage.CorrelationKey == CorrelationKey ? "Received correlated start message" : "Received incorrect start message :(");
        }

        public void Step1(Step1 step1Message)
        {
            Events.Add(step1Message.CorrelationKey == CorrelationKey ? "Received correlated step 1 message" : "Received incorrect step 1 message :(");
        }

        public void Step2(Step2 step2Message)
        {
            Events.Add(step2Message.CorrelationKey == CorrelationKey ? "Received correlated step 2 message" : "Received incorrect step 2 message :(");
        }

        public void ShutItDown(Stop stopMessage)
        {
            Events.Add(stopMessage.CorrelationKey == CorrelationKey ? "Received correlated stop message" : "Received incorrect stop message :(");
        }

        public StateMachine()
        {
            Events = new List<string>();
        }
    }

    public abstract class CorrelatedBy<T>
        : ICorrelate
    {
        protected T correlateBy { get; set; }

        public string CorrelationKey
        {
            get { return correlateBy.ToString(); }
            set { /* do nothing */ }
        }

        protected CorrelatedBy(T correlateBy)
        {
            this.correlateBy = correlateBy;
        }
    }

    public class Start
        : CorrelatedBy<int>
    {
        public Start(int correlateBy)
            : base(correlateBy)
        {
        }
    }

    public class Step1
        : CorrelatedBy<int>
    {
        public Step1(int correlateBy)
            : base(correlateBy)
        {
        }
    }

    public class Step2
        : CorrelatedBy<int>
    {
        public Step2(int correlateBy)
            : base(correlateBy)
        {
        }
    }

    public class Stop
        : CorrelatedBy<int>
    {
        public Stop(int correlateBy)
            : base(correlateBy)
        {
        }
    }

    public abstract class with_observable_of_messages
    {
        protected static IObservable<Start> startMessages { get; set; }
        protected static IObservable<Step1> step1Messages { get; set; }
        protected static IObservable<Step2> step2Messages { get; set; }
        protected static IObservable<Stop> stopMessages { get; set; }

        protected static IObservable<object> aggregateObservable { get; set; }

        private Establish context = () =>
                                        {
                                            startMessages = Observable.Generate(1,
                                                                                x => x < 6,
                                                                                x => new Start(x),
                                                                                x => x = x + 1);

                                            step1Messages = Observable.Generate(1,
                                                                                x => x < 6,
                                                                                x => new Step1(x),
                                                                                x => x = x + 1);

                                            step2Messages = Observable.Generate(1,
                                                                                x => x < 6,
                                                                                x => new Step2(x),
                                                                                x => x = x + 1);

                                            stopMessages = Observable.Generate(1,
                                                                                x => x < 6,
                                                                                x => new Stop(x),
                                                                                x => x = x + 1);

                                            aggregateObservable = Observable.Merge(
                                                    startMessages.Cast<object>(),
                                                    step1Messages.Cast<object>(),
                                                    step2Messages.Cast<object>(),
                                                    stopMessages.Cast<object>()
                                                );
                                        };
    }

    public abstract class with_single_state_machine : with_observable_of_messages
    {
        protected static StateMachine stateMachine { get; set; }
        protected static IDirect director { get; set; }

        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Core<StructureMapAdapter>()
                                                .Dependencies(x =>
                                                                  {
                                                                      x.For<ISagaRegistry>().Use<InMemorySagaRegistry>();
                                                                  });

                                            director = new Director(aggregateObservable)
                                                .For<StateMachine, Start>((x, y) => x.IShouldStartYawlsy(y))
                                                .For<StateMachine, Step1>((x, y) => x.Step1(y))
                                                .For<StateMachine, Step2>((x, y) => x.Step2(y))
                                                .For<StateMachine, Stop>((x, y) => x.ShutItDown(y));
                                        };
    }

    public class when_processing_messages_for_sagas : with_single_state_machine
    {
        protected static StateMachine saga1 { get; set; }
        protected static StateMachine saga2 { get; set; }
        protected static StateMachine saga3 { get; set; }
        protected static StateMachine saga4 { get; set; }
        protected static StateMachine saga5 { get; set; }

        private It should_have_five_sagas = () =>
                                                {
                                                    saga1 = director.Registry.GetSagaOf<StateMachine>("1");
                                                    saga2 = director.Registry.GetSagaOf<StateMachine>("2");
                                                    saga3 = director.Registry.GetSagaOf<StateMachine>("3");
                                                    saga4 = director.Registry.GetSagaOf<StateMachine>("4");
                                                    saga5 = director.Registry.GetSagaOf<StateMachine>("5");
                                                };

        private It should_have_four_events_each = () =>
                                                      {
                                                          saga1.Events.Count.ShouldEqual(4);
                                                          saga2.Events.Count.ShouldEqual(4);
                                                          saga3.Events.Count.ShouldEqual(4);
                                                          saga4.Events.Count.ShouldEqual(4);
                                                          saga5.Events.Count.ShouldEqual(4);
                                                      };
    }
}
