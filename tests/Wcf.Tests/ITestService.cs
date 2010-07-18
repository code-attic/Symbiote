using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Wcf.Tests
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        Return TwoArgCall(DateTime date, Guid id);
    }

    [DataContract]
    public class Return
    {
        [DataMember]
        public DateTime datetime { get; set; }
    }
}