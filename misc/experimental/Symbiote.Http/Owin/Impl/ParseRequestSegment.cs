namespace Symbiote.Http.Owin.Impl
{
    public delegate void ParseRequestSegment( Request request, ConsumableSegment<byte> segment);
}