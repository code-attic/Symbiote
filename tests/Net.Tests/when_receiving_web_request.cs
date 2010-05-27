using System;
using System.IO;
using Machine.Specifications;
using Symbiote.Core;

namespace Net.Tests
{
    public class when_receiving_web_request : with_web_request
    {
        protected static string request;

        private Because of = () =>
                                 {
                                     int consecutiveFeeds = 0;

                                     var builder = new DelimitedBuilder(Environment.NewLine);

                                     while(secureStream == null || !secureStream.CanRead)
                                     {
                                         
                                     }

                                     var reader = new StreamReader(secureStream);
                                     while(consecutiveFeeds < 2 && !reader.EndOfStream)
                                     {
                                         var nextChar = reader.Peek();
                                         if (nextChar != 13)
                                         {
                                             var line = reader.ReadLine();
                                             if (string.IsNullOrEmpty(line))
                                             {
                                                 consecutiveFeeds++;
                                             }
                                             else
                                             {
                                                 builder.Append(line);
                                                 consecutiveFeeds = 0;
                                             }
                                         }
                                         else
                                         {
                                             break;
                                         }
                                     }
                                     request = builder.ToString();

                                     using(var writer = new StreamWriter(secureStream))
                                     {
                                         writer.WriteLine("BOOGABOOGA!");
                                     }
                                 };

        private It should_have_request_body = () => request.ShouldNotBeEmpty();
    }
}