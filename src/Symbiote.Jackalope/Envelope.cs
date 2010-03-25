using System;

namespace Symbiote.Jackalope
{
    public class Envelope : IDisposable
    {
        public bool Empty { get { return Response == null || Message == null; } }
        public IResponse Response { get; set; }
        public object Message { get; set; }

        public void Dispose()
        {
            (Response as IDisposable).Dispose();
        }
    }
}