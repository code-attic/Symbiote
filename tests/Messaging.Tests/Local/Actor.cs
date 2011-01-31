using System;
using System.Collections.Generic;
using Symbiote.Core;
using Symbiote.Core.Actor;

namespace Messaging.Tests.Local
{
    public class Actor        
    {
        public string Id { get; set; }
        public List<string> FacesIveDrivenOver { get; set; }
        public static int Created { get; set; }
        public TimeSpan Lag { get; set; }

        public void KickTheCrapOutOf(string target)
        {
            FacesIveDrivenOver.Add(target);
        }

        public Actor()
        {
            FacesIveDrivenOver = new List<string>();
            Created ++;
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

    public class ActorKeyAccessor
        : IKeyAccessor<Actor>
    {
        public string GetId(Actor actor)
        {
            return actor.Id;
        }

        public void SetId<TKey>(Actor actor, TKey id)
        {
            actor.Id = id.ToString();
        }
    }
}