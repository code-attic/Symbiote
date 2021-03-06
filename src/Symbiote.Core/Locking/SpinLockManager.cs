﻿// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using System.Threading;

namespace Symbiote.Core.Locking
{
    public abstract class SpinLockManager
        : ILockManager
    {
        public virtual bool AcquireLock<T>( T lockId )
        {
            var newId = Guid.NewGuid();
            var acquired = false;
            while ( !acquired )
            {
                acquired = AttemptAcquisition( lockId, newId );
                if ( !acquired )
                    Thread.Sleep( 15 );
            }
            return acquired;
        }

        public virtual void ReleaseLock<T>( T lockId )
        {
            Release( lockId );
        }

        protected virtual bool AttemptAcquisition<T>( T lockId, Guid value )
        {
            var acquired = Lock( lockId, value );
            return acquired;
        }

        protected abstract bool Lock<T>( T lockId, Guid check );
        protected abstract void Release<T>( T lockId );
    }
}