namespace Symbiote.Http.Owin.Impl
{
    public class HttpConstants
    {
        protected const byte CR = 0x0d;
        protected const byte LF = 0x0a;
        protected const byte DOT = 0x2e;
        protected const byte SPACE = 0x20;
        protected const byte SEMI = 0x3b;
        protected const byte COLON = 0x3a;
        protected const byte HASH = 0x23;
        protected const byte QMARK = 0x3f;
        protected const byte SLASH = 0x2f;
        protected const byte DASH = 0x2d;
        protected const byte NULL = 0x00;
        protected static readonly byte[] LINE_TERMINATOR = new [] { CR, LF };
        protected static readonly byte[] BODY_PREFIX = new [] { CR, LF, CR, LF };
    }
}