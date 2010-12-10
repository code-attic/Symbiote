using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;
using Symbiote.Core;
using Symbiote.StructureMap;
using Symbiote.Redis;
using Symbiote.Daemon;
using Symbiote.Log4Net;
using Symbiote.Core.Extensions;
using NetGraph;
using System.Runtime.Serialization;

namespace NetGraphDemo
{
    public class NetGraphMgr
    {
        protected INetGraphClient Client { get; set; }
        private Dictionary<DateTime, DateNode> _EffectiveEntryDateNode;
        private DateTime _NullProcessingDateTime;
        private DateTime _NullSettlementDateTime;
        private Dictionary<DateTime, DateNode> _ProcessingDateNode;
        private Dictionary<DateTime, DateNode> _SettlementDateNode;
        private Dictionary<int, TransactionNode> _transactionDict;
        private const string STR_Ctransactionsbin = @"C:\transactions.bin";
        private const string CREDIT_KEY = "CREDITS";
        private const string DEBIT_KEY = "DEBITS";
        private const string OFFSET_KEY = "OFFSETS";
        private const string ORIGINATION_KEY = "ORIGINATIONS";


        public NetGraphMgr(INetGraphClient netGraphCli)
        {
            Client = netGraphCli;

            _transactionDict = new Dictionary<int, TransactionNode>();
            _EffectiveEntryDateNode = new Dictionary<DateTime, DateNode>();
            _SettlementDateNode = new Dictionary<DateTime, DateNode>();
            _NullSettlementDateTime = DateTime.MinValue;
            AddDateNode(_NullSettlementDateTime, _SettlementDateNode);
            _ProcessingDateNode = new Dictionary<DateTime, DateNode>();
            _NullProcessingDateTime = DateTime.MinValue;
            AddDateNode(_NullProcessingDateTime, _ProcessingDateNode);
        }

        public void LoadRedisFromFile()
        {
            var watch = Stopwatch.StartNew();

            int counter = 0;
            TransactionProto proto = null;
            using (var file = File.OpenRead(STR_Ctransactionsbin))
            {
                bool successful = true;
                do
                {
                    try
                    {
                        proto = Serializer.DeserializeWithLengthPrefix<TransactionProto>(file, PrefixStyle.Base128);

                    }
                    catch (EndOfStreamException ex)
                    {
                        successful = false;
                    }

                    int id = proto.TransactionID;
                    var node = new TransactionNode();
                    node.InitKey();
                    node.CloneTransactionProto(proto);
                    AddTransactionNode(id, node);
                    counter++;

                    LinkTransactionToDateNode(node, proto.EffectiveEntryDate, _EffectiveEntryDateNode, "EED");
                    LinkTransactionToDateNode(node, proto.ProcessingDate, _ProcessingDateNode, "PD");
                    LinkTransactionToDateNode(node, proto.SettlementDate, _SettlementDateNode, "SD");
                } while (successful && counter <= 10000);
                "Loaded {0} records in {1} miliseconds. \r\n\t{2} calls ops per second.\r\n\t{3} miliseconds per operation. \r\n\t{4} writes total. \r\n\t{5} writes per second\r\n\t{6} miliseconds per write."
                    .ToDebug<NetGraphDemo>(
                        counter,
                        watch.ElapsedMilliseconds,
                        (counter * 9) / watch.Elapsed.TotalSeconds,
                        watch.Elapsed.TotalMilliseconds / (counter * 9),
                        (counter * 9),
                        (counter * 9) / watch.Elapsed.TotalSeconds,
                        watch.Elapsed.TotalMilliseconds / (counter * 9)
                        );
            }

        }

        public void LookupByEed()
        {
            LookupRelationships("EED", _EffectiveEntryDateNode);
        }

        public void LookupByPd()
        {
            LookupRelationships("PD", _ProcessingDateNode);
        }

        public void LookupBySd()
        {
            LookupRelationships("SD", _SettlementDateNode);
        }

        private void LookupRelationships(string lookupType, Dictionary<DateTime, DateNode> dictToUse)
        {
            var nodeCounter = 0;
            var eedCounter = 0;
            Stopwatch watch = Stopwatch.StartNew();
            foreach (DateNode Eed in dictToUse.Values)
            {
                eedCounter++;
                var nodes = Client.GetRelatedNodes(Eed.Key, EdgeDirection.Forward);
                nodes.ForEach(x => nodeCounter++);
            }
            watch.Stop();
            "Retrieved {7} relationships for {0} edges in {1} miliseconds. \r\n\t{2} calls per second.\r\n\t{3} miliseconds per operation. \r\n\t{4} nodes read total. \r\n\t{5} nodes per second\r\n\t{6} miliseconds per node."
                .ToDebug<NetGraphDemo>(
                    eedCounter,
                    watch.ElapsedMilliseconds,
                    (eedCounter) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (eedCounter),
                    nodeCounter,
                    (nodeCounter) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (nodeCounter),
                    lookupType
                    );
        }

        public void LookupEedByAttributesDbOnly()
        {
            var lookupType = "EED";
            var nodeCounter = 0;
            var eedCounter = 0;
            Stopwatch watch = Stopwatch.StartNew();
            var eedList = Client.GetMasterAttributeKeysForType("EED");

            foreach (string Eed in eedList)
            {
                eedCounter++;
                List<string> keys = new List<string>();
                keys.Add(Eed);
                keys.Add(CREDIT_KEY);
                var nodes = Client.GetCommonNodes(keys);
                nodes.ForEach(x => nodeCounter++);

                keys.Remove(CREDIT_KEY);
                keys.Add(DEBIT_KEY);
                nodes = Client.GetCommonNodes(keys);
                nodes.ForEach(x => nodeCounter++);
            }
            watch.Stop();
            "Retrieved {7} relationships for {0} edges in {1} miliseconds. \r\n\t{2} calls per second.\r\n\t{3} miliseconds per operation. \r\n\t{4} nodes read total. \r\n\t{5} nodes per second\r\n\t{6} miliseconds per node."
                .ToDebug<NetGraphDemo>(
                    eedCounter,
                    watch.ElapsedMilliseconds,
                    (eedCounter) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (eedCounter),
                    nodeCounter,
                    (nodeCounter) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (nodeCounter),
                    lookupType
                    );

        }

        
        public void LookupEedByAttributes()
        {
            var lookupType = "EED";
            var dictToUse = _EffectiveEntryDateNode;
            var nodeCounter = 0;
            var eedCounter = 0;
            Stopwatch watch = Stopwatch.StartNew();
            foreach (DateNode Eed in dictToUse.Values)
            {
                eedCounter++;
                List<string> keys = new List<string>();
                keys.Add("{0}:ForwardLink".AsFormat(Eed.Key));
                keys.Add(CREDIT_KEY);
                var nodes = Client.GetCommonNodes(keys);
                nodes.ForEach(x => nodeCounter++);

                keys.Remove(CREDIT_KEY);
                keys.Add(DEBIT_KEY);
                nodes = Client.GetCommonNodes(keys);
                nodes.ForEach(x => nodeCounter++);
            }
            watch.Stop();
            "Retrieved {7} relationships for {0} edges in {1} miliseconds. \r\n\t{2} calls per second.\r\n\t{3} miliseconds per operation. \r\n\t{4} nodes read total. \r\n\t{5} nodes per second\r\n\t{6} miliseconds per node."
                .ToDebug<NetGraphDemo>(
                    eedCounter,
                    watch.ElapsedMilliseconds,
                    (eedCounter) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (eedCounter),
                    nodeCounter,
                    (nodeCounter) / watch.Elapsed.TotalSeconds,
                    watch.Elapsed.TotalMilliseconds / (nodeCounter),
                    lookupType
                    );
            
        }
        private void LinkTransactionToDateNode(TransactionNode transactionNode, DateTime linkDate, Dictionary<DateTime, DateNode> dict, string linkType)
        {
            if (!dict.ContainsKey(linkDate))
                AddDateNode(linkDate, dict);
            DateNode node = dict[linkDate];
            Client.AddAttribute(linkType, node.Key, transactionNode.Key);
        }

        private void AddDateNode(DateTime dtToAdd, Dictionary<DateTime, DateNode> dictionary)
        {
//            string key = GetGuidKey();
            string key = dtToAdd.ToString("ddMMyyyy");
            DateNode node = new DateNode { Key = key, Value = dtToAdd };
            node.InitKey();
            //Client.AddNode(node.Key);
            dictionary.Add(dtToAdd, node);
        }

        private void AddTransactionNode(int origId, TransactionNode node)
        {
            if (!_transactionDict.ContainsKey(origId))
            {
                string key = node.Key;
                Client.AddNode(node.Key);

                var keyToUse = node.IsOffset? OFFSET_KEY: ORIGINATION_KEY;
                Client.AddToLookupSet(keyToUse, node.Key);

                keyToUse = node.Amount >= 0? CREDIT_KEY: DEBIT_KEY;
                Client.AddToLookupSet(keyToUse, node.Key);

                _transactionDict.Add(origId, node);
            }

        }

        private string GetGuidKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
