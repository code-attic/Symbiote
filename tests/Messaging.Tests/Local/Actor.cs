using System;
using System.Collections.Generic;
using Symbiote.Core.Extensions;
using Symbiote.Messaging.Impl.Actors;

namespace Messaging.Tests.Local
{
    public class Actor        
    {
        public string Id { get; set; }
        public List<string> FacesIveDrivenOver { get; set; }
        public static int Created { get; set; }
        public static List<Actor> ArmyOfMehself = new List<Actor>();

        public void KickTheCrapOutOf(string target)
        {
            "I'm going to drive a truck over your face {0}!"
                .ToInfo<Actor>(target);
            FacesIveDrivenOver.Add(target);
        }

        public Actor()
        {
            FacesIveDrivenOver = new List<string>();
            Created ++;
            ArmyOfMehself.Add(this);
        }
    }

    public class ActorFactory
        : IActorFactory<Actor>
    {
        public Actor CreateInstance<TKey>(TKey id)
        {
            return new Actor() {Id = id.ToString()};
        }
    }
}