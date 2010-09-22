using System;

namespace Symbiote.Redis.Impl
{
    public class SortOptions {
        public string Key { get; set; }
        public bool Descending { get; set; }
        public bool Lexographically { get; set; }
        public Int32 LowerLimit { get; set; }
        public Int32 UpperLimit { get; set; }
        public string By { get; set; }
        public string StoreInKey { get; set; }
        public string Get { get; set; }

        public string ToCommand()
        {
            var command = "SORT " + this.Key;
            if (LowerLimit != 0 || UpperLimit != 0)
                command += " LIMIT " + LowerLimit + " " + UpperLimit;
            if (Lexographically)
                command += " ALPHA";
            if (!string.IsNullOrEmpty (By))
                command += " BY " + By;
            if (!string.IsNullOrEmpty (Get))
                command += " GET " + Get;
            if (!string.IsNullOrEmpty (StoreInKey))
                command += " STORE " + StoreInKey;
            return command;
        }
    }
}