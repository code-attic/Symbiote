using System;

namespace Core.Tests.Domain.Model
{
    public interface IHaveTestKey
    {
        Guid Id { get; set; }
    }
}