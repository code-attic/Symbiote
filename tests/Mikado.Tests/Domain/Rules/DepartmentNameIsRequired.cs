using System;
using Mikado.Tests.Domain.Model;
using Symbiote.Mikado.Impl;

namespace Mikado.Tests.Domain.Rules
{
    public class DepartmentNameIsRequired : Rule<Manager>
    {
        public DepartmentNameIsRequired()
            : base(x => !String.IsNullOrEmpty(x.Department), "LastName cannot exceed 25 characters.")
        {

        }
    }
}