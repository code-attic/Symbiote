using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Serialization;

namespace Core.Tests.Serialization.Extensions
{
    public class when_using_serialization_extensions
    {
        It should_recognize_unmarked_readonly_properties = () => 
            typeof( UnmarkedReadonlyProperties ).ReadOnlyPropertiesMarkedWithJsonIgnore().ShouldBeFalse();

        It should_recognize_marked_readonly_properties = () => 
            typeof( MarkedReadonlyProperties ).ReadOnlyPropertiesMarkedWithJsonIgnore().ShouldBeTrue();

        It should_recognize_data_contract_marking = () => 
            typeof( FullyMarkedWithContract ).MarkedWithDataContracts().ShouldBeTrue();

        It should_recognize_data_contract_partial = () => 
            typeof( PartiallyMarkedWithContract ).MarkedWithDataContracts().ShouldBeFalse();

        It should_not_allow_partial_markings_to_use_protobuf = () => 
            typeof( PartiallyMarkedWithContract ).IsProtobufSerializable().ShouldBeFalse();

        It should_recognize_default_arg_for_inherited_constructor = () =>
            typeof( PartiallyMarkedWithContract ).HasDefaultConstructor().ShouldBeTrue();

        It should_recognize_no_default_constructor = () => 
            typeof( MarkedWithContractButNoDefaultConstructor ).HasDefaultConstructor().ShouldBeFalse();

        It should_recognize_json_serializable = () =>
            typeof( PartiallyMarkedWithContract ).IsJsonSerializable().ShouldBeTrue();
    }
}
