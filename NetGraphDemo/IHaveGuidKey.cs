using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NetGraphDemo
{
    interface IHaveGuidKey
    {
        string Key { get; set; }
    }

    [DataContract]
    public abstract class HaveGuidKeyBase : IHaveGuidKey, IComparable
    {
        protected string _key = null;

        [DataMember(Name = "Key", Order = 1, IsRequired = false)]
        public string Key
        {
            get { return GetKey(); }
            set { SetKey(value); }
        }

        private void SetKey(string value)
        {
            _key = value;
        }

        private string GetKey()
        {
            InitKey();
            return _key;
        }

        public int CompareTo(object obj)
        {
            if (obj is IHaveGuidKey)
            {
                return _key.CompareTo((obj as IHaveGuidKey).Key);
            }
            else
                return -1;
        }

        public void InitKey()
        {
            if (_key == null)
                _key = Guid.NewGuid().ToString("N");
        }

    }

    [DataContract]
    public class TransactionNode : HaveGuidKeyBase, IHaveGuidKey
    {

        [DataMember(Name = "TransactionID", Order = 10, IsRequired = false)]
        public int TransactionID;
        [DataMember(Name = "CustomerID", Order = 2, IsRequired = false)]
        public int CustomerID;
        [DataMember(Name = "OutgoingBatchID", Order = 3, IsRequired = false)]
        public int OutgoingBatchID;
        [DataMember(Name = "UploadedCustomerFileID", Order = 4, IsRequired = false)]
        public int UploadedCustomerFileID;
        [DataMember(Name = "EffectiveEntryDate", Order = 5, IsRequired = false)]
        public DateTime EffectiveEntryDate;
        [DataMember(Name = "ProcessingDate", Order = 6, IsRequired = false)]
        public DateTime ProcessingDate;
        [DataMember(Name = "SettlementDate", Order = 7, IsRequired = false)]
        public DateTime SettlementDate;
        [DataMember(Name = "IsOffset", Order = 8, IsRequired = false)]
        public bool IsOffset;
        [DataMember(Name = "Amount", Order = 9, IsRequired = false)]
        public decimal Amount;


        public void CloneTransactionProto(TransactionProto proto)
        {
            this.TransactionID = proto.TransactionID;
            this.CustomerID = proto.CustomerID;
            this.OutgoingBatchID = proto.OutgoingBatchID;
            this.UploadedCustomerFileID = proto.UploadedCustomerFileID;
            this.EffectiveEntryDate = proto.EffectiveEntryDate;
            this.ProcessingDate = proto.ProcessingDate;
            this.SettlementDate = proto.SettlementDate;
            this.IsOffset = proto.IsOffset;
            this.Amount = proto.Amount;

        }
    }

    [DataContract]
    public class DateNode : HaveGuidKeyBase, IHaveGuidKey
    {

        [DataMember(Name = "Value", Order = 2, IsRequired = false)]
        public DateTime Value;

    }
}
