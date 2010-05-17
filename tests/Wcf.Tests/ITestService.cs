using System;
using System.ServiceModel;

namespace Wcf.Tests
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        bool TwoArgCall(DateTime date, Guid id);
    }
}