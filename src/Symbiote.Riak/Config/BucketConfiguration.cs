namespace Symbiote.Riak.Config
{
    public class BucketConfiguration
    {
        public uint QuorumReadNodes { get; set; }
        public uint QuorumWriteNodes { get; set; }
        public string BucketName { get; set; }
        public bool WaitOnWrites { get; set; }

        public BucketConfiguration()
        {
            QuorumReadNodes = 1;
            QuorumWriteNodes = 1;
            BucketName = ( System.Reflection.Assembly.GetEntryAssembly() ??
                           System.Reflection.Assembly.GetExecutingAssembly() ).GetName().Name.Replace( ".", "" );
        }

        public BucketConfiguration(string name)
        {
            BucketName = name;
        }
    }
}