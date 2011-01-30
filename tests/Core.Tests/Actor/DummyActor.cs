using System;
using Symbiote.Core.Actor;

namespace Actor.Tests
{
    public class DummyActor
    {
        public string Id { get; set; }

        public DummyActor()
        {
            Instantiated++;
        }

        public static int Instantiated { get; set; }
    }
}