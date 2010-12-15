using System.Security.Principal;

namespace Symbiote.Daemon.Installation
{
    public class CredentialCheck
        : ICheckPermission
    {
        public bool HasPermission()
        {
            var identity = WindowsIdentity.GetCurrent();
            var permission = false;
            if(identity != null)
            {
                permission = new WindowsPrincipal( identity )
                    .IsInRole( WindowsBuiltInRole.Administrator );
            }
            return permission;
        }
    }
}