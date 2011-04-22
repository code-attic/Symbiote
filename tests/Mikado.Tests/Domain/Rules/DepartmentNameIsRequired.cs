using System;
using Mikado.Tests.Domain.Model;
using Symbiote.Mikado.Impl;

namespace Mikado.Tests.Domain.Rules
{
    public class DepartmentNameIsRequired : Rule<Manager>
    {
        public DepartmentNameIsRequired()
            : base(x => !String.IsNullOrEmpty(x.Department), "Department Name is required.")
        {

        }
    }

    public class AddressIsRequired : Rule<IHaveAddress>
    {
        public AddressIsRequired()
            : base(x => !String.IsNullOrEmpty(x.Address), "Address is required.")
        {

        }
    }

    public class CityIsRequired : Rule<IHaveCity>
    {
        public CityIsRequired()
            : base(x => !String.IsNullOrEmpty(x.City), "City is required.")
        {

        }
    }

    public class StateIsRequired : Rule<IHaveState>
    {
        public StateIsRequired()
            : base(x => !String.IsNullOrEmpty(x.State), "State is required.")
        {

        }
    }

    public class ZipCodeIsRequired : Rule<IHaveZipCode>
    {
        public ZipCodeIsRequired()
            : base(x => !String.IsNullOrEmpty(x.ZipCode), "ZipCode is required.")
        {

        }
    }
}