// /* 
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
using System.Security.Principal;

namespace Symbiote.Daemon.Installation
{
    public class CredentialCheck
        : ICheckPermission
    {
        #region ICheckPermission Members

        public bool HasPermission()
        {
            var identity = WindowsIdentity.GetCurrent();
            var permission = false;
            if ( identity != null )
            {
                permission = new WindowsPrincipal( identity )
                    .IsInRole( WindowsBuiltInRole.Administrator );
            }
            return permission;
        }

        #endregion
    }
}