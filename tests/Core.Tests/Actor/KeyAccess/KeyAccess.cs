using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;

namespace Core.Tests.Actor.KeyAccess
{
    public class ConcreteType
    {
        public string Id { get; set; }
    }

    public interface IHaveId
    {
        string Id { get; }
        void SetId( string id );
    }

    public class Class1 : IHaveId
    {
        public string Id { get; protected set; }
        public void SetId( string id )
        {
            Id = id;
        }
    }

    public class Class2 : IHaveId
    {
        public string Id { get; protected set; }
        public void SetId(string id)
        {
            Id = id;
        }
    }

    public class ConcreteKeyAccessor : IKeyAccessor<ConcreteType>
    {
        public string GetId( ConcreteType actor )
        {
            return actor.Id;
        }

        public void SetId<TKey>( ConcreteType actor, TKey key )
        {
            actor.Id = key.ToString();
        }
    }

    public class InterfaceKeyAccessor : IKeyAccessor<IHaveId>
    {
        public string GetId( IHaveId actor )
        {
            return actor.Id;
        }

        public void SetId<TKey>( IHaveId actor, TKey key )
        {
            actor.SetId( key.ToString() );
        }
    }

    public class with_key_accessor 
        : with_assimilation
    {
        public static IKeyAccessor KeyAccessor { get; set; }
        private Establish context = () =>
                                        {
                                            KeyAccessor = Assimilate.GetInstanceOf<IKeyAccessor>();
                                        };
    }

    public class when_accessing_key_by_concrete_accessor
        : with_key_accessor
    {
        public static ConcreteType instance { get; set; }
        public static string expected = "test";
        public static string Id { get; set; }
        private Because of = () =>
                                 {
                                     instance = new ConcreteType();
                                     KeyAccessor.SetId( instance, expected );
                                     Id = KeyAccessor.GetId( instance );
                                 };

        private It should_have_correct_id = () => Id.ShouldEqual( expected );
    }

    public class when_accessing_key_by_inherited_accessor
        : with_key_accessor
    {
        public static Class1 instance { get; set; }
        public static string expected = "test";
        public static string Id { get; set; }
        private Because of = () =>
        {
            instance = new Class1();
            KeyAccessor.SetId(instance, expected);
            Id = KeyAccessor.GetId(instance);
        };

        private It should_have_correct_id = () => Id.ShouldEqual(expected);
    }
}
