﻿using System;

namespace Symbiote.Core.DI
{
    public interface IDependencyDefinition
    {
        object ConcreteInstance { get; set; }
        Type ConcreteType { get; set; }
        Type FactoryType { get; set; }
        bool HasFactory { get; set; }
        bool IsAdd { get; set; }
        bool IsNamed { get; set; }
        bool IsSingleton { get; set; }
        string PluginName { get; set; }
        Type PluginType { get; set; }
        bool HasSingleton { get; set; }
    }
}