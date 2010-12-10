using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Redis;

namespace NetGraph.Impl.Command
{
    public class GetMasterAttributeNodesByTypeCommand<T> : NetGraphCommand<string[]>
    {
        protected string AttributeType { get; set; }

        public string[] GetNodes(IRedisClient redisClient)
        {
            var rslt = redisClient.GetKeys(KeyProvider.AttributeMasterKey(AttributeType, "*"));
            return rslt;
        }

        public GetMasterAttributeNodesByTypeCommand(string attributeType)
        {
            AttributeType = attributeType;
            Command = GetNodes;
        }

    }
}
