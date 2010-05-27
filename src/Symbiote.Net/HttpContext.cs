namespace Symbiote.Net
{
    public class HttpContext
    {
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        protected IHttpClient Client { get; set; }

        public HttpContext(IHttpClient client, HttpRequest request)
        {
            Client = client;
            Request = request;
            Response = new HttpResponse();
        }
    }
}