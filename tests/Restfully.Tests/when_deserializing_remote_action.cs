using System;
using System.Linq;
using System.Linq.Expressions;
using Machine.Specifications;
using Newtonsoft.Json.Linq;
using StructureMap;
using Symbiote.Core.Extensions;
using Symbiote.Restfully;
using Symbiote.Restfully.Impl;

namespace Restfully.Tests
{
    [Subject("RemoteAction")]
    public class when_deserializing_remote_action
    {
        protected static RemoteAction<ITestService> remoteAction;
        protected static string json;
        protected static object deserializedAction;
        protected static IRemoteProcedure procedure;
        protected static string testArg = "hi";

        private Because of = () =>
                                 {
                                     ObjectFactory.Configure(x => x.For<ITestService>().Use<TestService>());
                                     remoteAction = new RemoteAction<ITestService>(x => x.OneArgCall(testArg));
                                     json = remoteAction.ToJson();
                                     deserializedAction = json.FromJson();

                                     procedure = deserializedAction as IRemoteProcedure;
                                     procedure.JsonExpressionTree = json;
                                     procedure.Invoke();
                                 };

        private It should_be_of_correct_type = () => 
                                               deserializedAction.GetType().ShouldEqual(typeof (RemoteAction<ITestService>));

        private It should_have_method_name = () => 
                                             procedure.Method.ShouldEqual("OneArgCall");



    }

    public abstract class with_expression
    {
        protected static Expression<Action<ITestService>> expression;
        protected static DateTime arg1 = DateTime.Now;

        private Establish context = () =>
                                        {
                                            var id = Guid.NewGuid();
                                            expression = x => x.TwoArgCall(arg1, id);
                                        };
    }

    [Subject("Tranforming Expression")]
    public class when_translating_expression_args_to_constants : with_expression
    {
        protected static Expression<Action<ITestService>> transformedExpression;
        protected static string json;

        private Because of = () =>
                                 {
                                     transformedExpression = expression.ChangeArgsToConstants();
                                     json = transformedExpression.ToJson();
                                     var test = "";
                                 };

        private It should_produce_json = () => json.ShouldNotBeNull();
    }
}