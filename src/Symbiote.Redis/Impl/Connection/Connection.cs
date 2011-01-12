/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Config;

namespace Symbiote.Redis.Impl.Connection
{
    public class Connection
        : IConnection
    {
        protected Socket Socket { get; set; }
        protected BufferedStream BufferedStream { get; set; }
        protected RedisConfiguration Configuration { get; set; }
        protected const byte RETURN = (byte) '\r';
        protected const byte NEWLINE = (byte) '\n';
        private const string ERROR_HEADER = "-ERR";
        private const string AUTHORIZE = "AUTH {0}\r\n";
        private const int BUFFER_LIMIT = 16*1024;
        private const string KEY_EXISTS = "EXISTS {0}\r\n";
        protected readonly byte[] end_data = new [] { RETURN, NEWLINE };
        private string UNABLE_TO_CONNECT = "Unable to connect";

        public bool InUse { get; set; }

        public bool ContainsKey (string key)
        {
            if (key == null)
                throw new ArgumentNullException ("key");
            return SendDataExpectInt(null, KEY_EXISTS.AsFormat(key)) == 1;
        }

        public bool ExpectSuccess()
        {
            var sentinel = GetSentinelCharacter();
            var line = ReadLine ();
            if (sentinel == '-')
                throw new ResponseException (line.StartsWith ("ERR") ? line.Substring (4) : line);
            return true;
        }

        protected int GetSentinelCharacter()
        {
            var sentinel = BufferedStream.ReadByte ();
            if (sentinel == -1)
                throw new ResponseException ("No more data");
            return sentinel;
        }
        
        protected string GetNextLine(ref int? sentinel)
        {
            var line = ReadLine();
            if (line.Length == 0)
                throw new ResponseException("Zero length respose");
            sentinel = sentinel ?? line[0];
            if (sentinel == '-')
                throw new ResponseException(line.StartsWith("ERR") ? line.Substring(4) : line);
            return line;
        }

        public bool SendExpectSuccess(byte[] data, string command)
        {
           SendCommand (data, command);
           return ExpectSuccess();
        }

        public IEnumerable<bool> SendExpectSuccess(IEnumerable<Tuple<byte[], string>> pairs)
        {
            pairs.ForEach(p => SendCommand(p.Item1, p.Item2));
            var rslt = new List<bool>();
            for (int i = 0; i < pairs.Count(); i++)
            {
            	rslt.Add(ExpectSuccess());
            }
            return rslt;
        }

        public int SendDataExpectInt(byte[] data, string command)
        {
            SendCommand(data, command);
            int? sentinel = GetSentinelCharacter();
            var line = GetNextLine(ref sentinel);
            
            if (sentinel == ':'){
                int i;
                if (int.TryParse (line, out i))
                    return i;
            }
            throw new ResponseException ("Unknown reply on integer request: " + sentinel + line);
        }

        public string SendExpectString(string command)
        {
            SendCommand(null, command);
            int? sentinel = GetSentinelCharacter();
            var line = GetNextLine(ref sentinel);

            if (sentinel == '+')
                return line;
            throw new ResponseException ("Unknown reply on integer request: " + sentinel + line);
        }

        public byte[] SendExpectData(byte[] data, string command)
        {
            SendCommand(data, command);
            return ReadData ();
        }

        public List<byte[]> SendExpectDataList(byte[] data, string command)
        {
            SendCommand(data, command);
            return ReadDataList();
        }

        public IDictionary<string, byte[]> SendExpectDataDictionary(byte[] data, string command)
        {
            SendCommand(data, command);
            return ReadDataDict();
        }

        protected byte[] ReadData()
        {
            int? sentinel = null;
            var line = GetNextLine(ref sentinel);

            if (sentinel == '$'){
                if (line == "$-1")
                    return null;
                var bufferLength = 0;

                if (Int32.TryParse (line.Substring (1), out bufferLength)){
                    var returnBuffer = new byte [bufferLength];
                    var bytesRead = 0;

                    do 
                    {
                        var read = BufferedStream.Read (returnBuffer, bytesRead, bufferLength - bytesRead);
                        if (read < 1)
                            throw new ResponseException("Invalid termination mid stream");
                        bytesRead += read; 
                    } while (bytesRead < bufferLength);
                    
                    if (BufferedStream.ReadByte () != RETURN || BufferedStream.ReadByte () != NEWLINE)
                        throw new ResponseException ("Invalid termination");
                    return returnBuffer;
                }
                throw new ResponseException ("Invalid length");
            }

            //returns the number of matches
            if (sentinel == '*') {
                var bufferLength = 0;
                if (Int32.TryParse(line.Substring(1), out bufferLength)) 
                    return bufferLength <= 0 ? new byte [0] : ReadData();

                throw new ResponseException ("Unexpected length parameter" + line);
            }

            throw new ResponseException ("Unexpected reply: " + line);
        }

        protected List<byte[]> ReadDataList()
        {
            int? sentinel = null;
            var line = GetNextLine(ref sentinel);
            List<byte[]> rsltList = new List<byte[]>();

            if (sentinel == '$')
            {
                if (line == "$-1")
                    return null;
                var bufferLength = 0;

                if (Int32.TryParse(line.Substring(1), out bufferLength))
                {
                    var returnBuffer = new byte[bufferLength];
                    var bytesRead = 0;

                    do
                    {
                        var read = BufferedStream.Read(returnBuffer, bytesRead, bufferLength - bytesRead);
                        if (read < 1)
                            throw new ResponseException("Invalid termination mid stream");
                        bytesRead += read;
                    } while (bytesRead < bufferLength);

                    if (BufferedStream.ReadByte() != RETURN || BufferedStream.ReadByte() != NEWLINE)
                        throw new ResponseException("Invalid termination");
                    rsltList.Add(returnBuffer);
                    return rsltList;
                }
                throw new ResponseException("Invalid length");
            }

            //returns the number of matches
            if (sentinel == '*')
            {
                var replyCount = 0;
                if (Int32.TryParse(line.Substring(1), out replyCount))
                {
                    if (replyCount <= 0)
                    {
                        rsltList.Add(new byte[0]);
                        return rsltList;
                    }
                    else
                    {
                    	for (int i = 0; i < replyCount; i++)
                        {
                            rsltList.Add(ReadData());
                        }
                        return rsltList;
                    }
                }
                throw new ResponseException("Unexpected length parameter" + line);
            }

            throw new ResponseException("Unexpected reply: " + line);
        }

        protected IDictionary<string, byte[]> ReadDataDict()
        {
            int? sentinel = null;
            var line = GetNextLine(ref sentinel);

            if (sentinel == '$')
            {
                throw new ResponseException("Invalid return data; unable to convert single value to IDictionary<string, T>");
            }

            //returns the number of matches
            if (sentinel == '*')
            {
                var rsltDict = new Dictionary<string, byte[]>();
                var replyCount = 0;
                if (Int32.TryParse(line.Substring(1), out replyCount))
                {
                    if (replyCount <= 0)
                    {
                        return rsltDict;
                    }
                    else
                    {
                        int remainder;
                        Math.DivRem(replyCount, 2, out remainder);
                        if (remainder != 0)
                            throw new ResponseException("Invalid return data; unable to convert an odd number of returns to IDictionary<string, T>");
                        
                        for (int i = 0; i < replyCount/2; i++)
                        {
                            var key = Encoding.UTF8.GetString(ReadData());
                            rsltDict.Add(key, ReadData());
                        }
                        return rsltDict;
                    }
                }
                throw new ResponseException("Unexpected length parameter" + line);
            }

            throw new ResponseException("Unexpected reply: " + line);
        }
        protected string ReadLine()
        {
            var builder = new StringBuilder ();
            var characterCode = 0;

            while ((characterCode = BufferedStream.ReadByte ()) != -1){
                if (characterCode == RETURN)
                    continue;
                if (characterCode == NEWLINE)
                    break;
                builder.Append ((char) characterCode);
            }
            return builder.ToString ();
        }

        public void Connect()
        {
            var server = Configuration.Hosts.First().Value;
            Socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.NoDelay = true;
            Socket.SendTimeout = Configuration.SendTimeout;
            Socket.Connect(server.Host, server.Port);
            if (!Socket.Connected)
            {
                Socket.Close ();
                Socket = null;
                return;
            }
            BufferedStream = new BufferedStream (new NetworkStream (Socket), BUFFER_LIMIT);

            if (Configuration.Password != null)
                SendExpectSuccess(null, AUTHORIZE.AsFormat(Configuration.Password));
        }

        protected void SendCommand(byte[] data, string command)
        {
            if (Socket == null)
                Connect ();
            if (Socket == null)
                throw new Exception(UNABLE_TO_CONNECT);

            var commandBytes = Encoding.UTF8.GetBytes (command);
            try 
            {
                Socket.Send (commandBytes);
                if(data != null && data.Length > 0)
                {
                    var sent = 0;
                    var buffer = (1024 ^ 2) * 7;
                    while(sent < data.Length)
                    {
                        var left = data.Length - sent;
                        var send = left > buffer ? buffer : left;
                        sent += Socket.Send(data, sent, send, SocketFlags.None);
                    }
                    //Socket.Send (data);
                    Socket.Send (end_data);
                }
            } 
            catch (SocketException socketException)
            {
                // timeout;
                Socket.Close ();
                Socket = null;
                throw new Exception(UNABLE_TO_CONNECT);
            }
        }

        public void Dispose()
        {
            if(Socket != null)
            {
                Socket.Close();
                Socket = null;
            }
        }

        public Connection(RedisConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}