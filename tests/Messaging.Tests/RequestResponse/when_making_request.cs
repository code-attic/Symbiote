using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Messaging.Tests.Local;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Actors;

namespace Messaging.Tests.RequestResponse
{
    public class when_making_request
        : with_bus
    {
        public static string Reply {get;set;}
        public static void OnReply(Reply reply)
        {
            Reply = reply.Text;
        }
        private Because of = () =>
        {
            bus.AddLocalChannelForMessageOf<Request>();
            bus.Request<Request, Reply>( new Request(), OnReply );
            Thread.Sleep( 60 );
        };

        private It should_have_response = () => Reply.ShouldEqual( "I have an answer!" );
    }

    public class Request
    {
        public string Text { get; set; }
    }

    public class Reply
    {
        public string Text { get; set; }
    }

    public class RequestHandler : IHandle<Request>
    {
        public void Handle( IEnvelope<Request> envelope )
        {
            envelope.Reply( new Reply()
            {
                Text = "I have an answer!"
            } );
        }
    }

    [Serializable]
    [DataContract]
    public class MakeBid
    {
        [DataMember(IsRequired = false, Order = 1)] 
        public string User { get; set; }

        [DataMember(IsRequired = false, Order = 2)]
        public string Item { get; set; }

        [DataMember(IsRequired = false, Order = 3)]
        public decimal Amount { get; set; }

        [DataMember(IsRequired = false, Order = 4)]
        public DateTime MadeOn { get; set; }

        public MakeBid()
        {
            MadeOn = DateTime.Now;
        }

        public MakeBid(string user, string item, decimal amount)
            : this()
        {
            User = user;
            Item = item;
            Amount = amount;
        }
    }

    [Serializable]
    [DataContract]
    public class AuctionStatus
    {
        public string Item { get; set; }
        public string HighestBidder { get; set; }
        public bool BiddingClosed { get; set; }
        public decimal CurrentBid { get; set; }

        public AuctionStatus() {}

        public AuctionStatus( string item, string highestBidder, bool biddingClosed, decimal currentBid )
        {
            Item = item;
            HighestBidder = highestBidder;
            BiddingClosed = biddingClosed;
            CurrentBid = currentBid;
        }
    }

    public class Bid
    {
        public string User { get; set; }
        public string Item { get; set; }
        public decimal Amount { get; set; }
        public DateTime MadeOn { get; set; }

        public Bid( string user, string item, decimal amount, DateTime madeOn )
        {
            User = user;
            Item = item;
            Amount = amount;
            MadeOn = madeOn;
        }
    }

    public class BidProcessor
        : IHandle<Auction, MakeBid>
    {
        public void Handle( Auction actor, IEnvelope<MakeBid> envelope )
        {
            var bid = new Bid(
                envelope.Message.User,
                envelope.Message.Item,
                envelope.Message.Amount,
                envelope.Message.MadeOn );
            var accepted = actor.AcceptBid(bid);


        }


    }

    public class Auction
    {
        public string Item { get; set; }
        public Bid CurrentBid { get; set; }
        public List<Bid> BidHistory { get; set; }
        public DateTime Expires { get; set; }

        public bool AcceptBid(Bid bid)
        {
            var accepted = false;
            if(Expires < DateTime.Now)
            {
                BidHistory.Add( bid );
                CurrentBid = bid;
                accepted = true;
            }
            return accepted;
        }

        public Auction()
        {
            BidHistory = new List<Bid>();
        }
    }

    public class AuctionKeyAccessor
        : IKeyAccessor<Auction>
    {
        public string GetId( Auction actor )
        {
            return actor.Item;
        }

        public void SetId<TKey>( Auction actor, TKey id )
        {
            actor.Item = id.ToString();
        }
    }


    public class AuctionFactory
        : IActorFactory<Auction>
    {
        public Auction CreateInstance<TKey>( TKey id )
        {
            return new Auction()
            {
                Item = id.ToString()
            };
        }
    }
}
