using System;
using System.Collections.Generic;

namespace Rabbit.Tests
{
    public class Actor
    {
        public string Id { get; set; }
        public static int Created { get; set; }
        public static List<Actor> ArmyOfMehself = new List<Actor>();
        protected static List<int> _messages = new List<int>();
        public static List<int> MessageIds
        {
            get { return _messages; }
        }

        public void Received(int messageid)
        {
            if(_messages.Contains( messageid ))
                throw new Exception("This is crap!");
            _messages.Add(messageid);
        }

        public Actor()
        {
            Created++;
            ArmyOfMehself.Add(this);
        }
    }
}