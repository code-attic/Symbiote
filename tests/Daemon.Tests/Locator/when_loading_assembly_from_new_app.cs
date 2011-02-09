using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core.Futures;
using Symbiote.Daemon.BootStrap;
using Symbiote.Core.Extensions;

namespace Daemon.Tests.Locator
{
    public class when_loading_assembly_from_new_app
    {
        public static string sourceDir = @"c:\git\Symbiote\demo\Minion\Minions\Hosted";
        public static Minion minion { get; set; }
        public static string testDir = @"c:\git\Symbiote\demo\Minion\Minions\Test";
        public static bool deleted;
        public static bool running;

        private Because of = () =>
                                 {
                                     CopySource();
                                     minion = new Minion( testDir );
                                     Thread.Sleep( 5000 );
                                     Future.WithoutResult( () => minion.StartUp() ).Start();
                                     Thread.Sleep( 10000 );
                                     
                                     running = minion.Running;
                                     minion.ShutItDown();
                                     deleted = Delete();
                                 };
        
        private It should_start_minion = () => running.ShouldBeTrue();
        private It should_have_deleted_directory = () => deleted.ShouldBeTrue();

        public static void CopySource()
        {
            var files = Directory.GetFiles( sourceDir );
            Delete();
            Directory.CreateDirectory( testDir );
            files.ForEach( x => File.Copy( x, Path.Combine( testDir, Path.GetFileName( x ) ) ) );
        }

        public static bool Delete()
        {
            try
            {
                Directory.Delete(testDir, true);
                return true;
            }
            catch ( Exception e )
            {
                return false;
            }
        }
    }
}
