﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Riak.Impl
{
    public interface IRiakConnection
    {
        object Send<T>( T command );
    }
}