using System;
using System.Linq;
using System.Linq.Expressions;
using Machine.Specifications;
using Newtonsoft.Json.Linq;
using Symbiote.Core.Extensions;
using Symbiote.Restfully;

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
                                     remoteAction = new RemoteAction<ITestService>(x => x.OneArgCall(testArg));
                                     json = remoteAction.ToJson();
                                     deserializedAction = json.FromJson();

                                     JObject.Parse(json)["ExpressionTree"];

                                     procedure = deserializedAction as IRemoteProcedure;
                                 };

        protected void CreateExpressionFromJObject()
        {
            
        }

        private It should_be_of_correct_type = () => 
                                               deserializedAction.GetType().ShouldEqual(typeof (RemoteAction<ITestService>));

        private It should_have_method_name = () => 
                                             procedure.Method.ShouldEqual("OneArgCall");

        private It should_have_argument_list = () => 
                                               procedure.Args.Values.Count.ShouldEqual(1);

        private It should_have_correct_argument_values = () => 
                                                         ShouldExtensionMethods.ShouldBeTrue(procedure.Args.Values.SequenceEqual(new [] {"hi"}));



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