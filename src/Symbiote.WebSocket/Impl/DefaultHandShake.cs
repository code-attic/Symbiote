using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Symbiote.Core;
using Symbiote.Core.Extensions;

namespace Symbiote.WebSocket.Impl
{
    public class DefaultHandShake : IShakeHands
    {
        protected readonly string _handshake_line1 = "GET /{0} HTTP/1.1";
        protected readonly string _handshake_line2 = "Upgrade: WebSocket";
        protected readonly string _handshake_line3 = "Connection: Upgrade";
        protected readonly string _handshake_line4 = "Host: {0}";
        protected readonly string _handshake_line5 = "Origin: {0}";
        protected readonly string _handshake_line6 = "";
        protected IWebSocketServerConfiguration _configuration;

        protected string HandshakeLine1 
        {
            get
            {
                return _handshake_line1.AsFormat(_configuration.SocketResource);
            }
        }
        protected string HandshakeLine2 { get { return _handshake_line2; } }
        protected string HandshakeLine3 { get { return _handshake_line3; } }
        protected string HandshakeLine4
        {
            get
            {
                return "Host: {0}:{1}".AsFormat(_configuration.SocketServer, _configuration.Port);
            }
        }
        protected string HandshakeLine5 { get { return _handshake_line5.AsFormat(_configuration.ServerUrl); } }
        protected string HandshakeLine6 { get { return _handshake_line6; } }

        protected IEnumerable<string> ExpectedHandshakeLines
        {
            get
            {
                yield return HandshakeLine1;
                yield return HandshakeLine2;
                yield return HandshakeLine3;
                yield return HandshakeLine4;
                yield return HandshakeLine5;
                yield return HandshakeLine6;
            }
        }

        public virtual bool ValidateHandShake(Socket socket)
        {
            using (var stream = new NetworkStream(socket))
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            {
                "Attempting handshake..."
                    .ToInfo<ISocketServer>();

                var request = new DelimitedBuilder("\r\n");
                var requestLine = "";
                var expected = ExpectedHandshakeLines.ToArray();
                var handshakeIndex = 0;
                var origin = _configuration.ServerUrl;
                do
                {
                    requestLine = reader.ReadLine();
                    var match = requestLine == expected[handshakeIndex];

                    // if we're allowing just anyone to originate the request...
                    if(!_configuration.StrictOriginMatching && handshakeIndex == 4)
                    {
                        origin = Regex.Match(requestLine, @"(?<=Origin:[ ]).+").Value;
                        match = true;
                    }

                    if (!match)
                    {
                        if (!requestLine.StartsWith("Cookie"))
                        {
                            "Step {0} of handshake from client is: \r\n\t {1}. \r\n\t Expected: {2}"
                                .ToError<ISocketServer>(handshakeIndex, requestLine, expected[handshakeIndex]);
                            socket.Close();
                            return false;
                        }
                    }
                    else
                    {
                        handshakeIndex++;
                    }

                    request.Append(requestLine);
                } while (!string.IsNullOrEmpty(requestLine));

                "{0}".ToInfo<ISocketServer>(request.ToString());

                writer.WriteLine("HTTP/1.1 101 Web Socket Protocol Handshake");
                writer.WriteLine("Upgrade: WebSocket");
                writer.WriteLine("Connection: Upgrade");
                writer.WriteLine("WebSocket-Origin: " + origin);
                writer.WriteLine("WebSocket-Location: " + _configuration.SocketUrl);
                writer.WriteLine("");
            }

            "Handshake complete."
                .ToInfo<ISocketServer>();

            return true;
        }

        public DefaultHandShake(IWebSocketServerConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}