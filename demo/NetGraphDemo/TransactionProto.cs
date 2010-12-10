using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NetGraphDemo
{
    [DataContract]
    public class TransactionProto
    {
        private const string STR_JSON_FMT = "{{\r\n  \"CustomerID\": {0},\r\n  \"OutgoingBatchId\": {1},\r\n  \"UploadedCustomerFileID\": {2},\r\n  \"EffectiveEntryDate\": \"{3}\",\r\n  \"ProcessingDate\": \"{4}\",\r\n  \"SettlementDate\": \"{5}\",\r\n  \"IsOffset\": {6},\r\n  \"Amount\": {7}\r\n}}";

        [DataMember(Name = "TransactionID", Order = 1, IsRequired = false)]
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

        public string ToJsonString()
        {
            return string.Format(STR_JSON_FMT, CustomerID, OutgoingBatchID, UploadedCustomerFileID, EffectiveEntryDate, ProcessingDate, SettlementDate, IsOffset ? "true" : "false", Amount);
        }
    }
    [DataContract]
    public class OriginationOffsetProto
    {
        public static string[] FieldNames = new string[3] { "OriginationTransactionID", "OriginationOffsetTransctionID", "IsSettlementOffset" };
        [DataMember(Name = "OriginationTransactionID", Order = 1, IsRequired = false)]
        public int OriginationTransactionID;
        [DataMember(Name = "OriginationOffsetTransctionID", Order = 2, IsRequired = false)]
        public int OriginationOffsetTransctionID;
        [DataMember(Name = "IsSettlementOffset", Order = 3, IsRequired = false)]
        public bool IsSettlementOffset;
    }
    [DataContract]
    public class ReturnOffsetProto
    {
        public static string[] FieldNames = new string[2] { "ReturnID", "OffsetTransactionID" };
        [DataMember(Name = "ReturnID", Order = 1, IsRequired = false)]
        public int ReturnID;
        [DataMember(Name = "OffsetTransactionID", Order = 2, IsRequired = false)]
        public int OffsetTransactionID;
    }

    [DataContract]
    public class ReturnProto
    {
        public static string[] FieldNames = new string[6] { "ReturnID", "OriginalTraceNumber", "Amount", "ReturnOffsetID", "TransactionID", "EffectiveDate" };
        [DataMember(Name = "ReturnID", Order = 1, IsRequired = false)]
        public int ReturnID;
        [DataMember(Name = "OriginalTraceNumber", Order = 2, IsRequired = false)]
        public string OriginalTraceNumber;
        [DataMember(Name = "Amount", Order = 3, IsRequired = false)]
        public decimal Amount;
        [DataMember(Name = "ReturnOffsetID", Order = 4, IsRequired = false)]
        public int ReturnOffsetID;
        [DataMember(Name = "TransactionID", Order = 5, IsRequired = false)]
        public int TransactionID;
        [DataMember(Name = "EffectiveDate", Order = 6, IsRequired = false)]
        public DateTime EffectiveDate;
    }


}
