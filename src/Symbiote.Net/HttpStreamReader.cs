﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Linq;
using Symbiote.Core.Extensions;

namespace Symbiote.Net
{
    public class HttpStreamReader : IObservable<string>, IDisposable
    {
        protected Stream HttpStream { get; set; }
        protected ConcurrentBag<IObserver<string>> Observers { get; set; }
        protected bool Running { get; set; }
        protected byte[] ReadBuffer { get; set; }
        protected long BufferSize { get; set; }
        protected int BytesRead { get; set; }

        public void Start()
        {
            Running = true;
            BeginRead();
        }

        private void BeginRead()
        {
            if (HttpStream.CanRead && Running)
            {
                HttpStream.BeginRead(ReadBuffer, BytesRead, 2048, CompleteRead, null);
            }
        }

        private void CompleteRead(IAsyncResult ar)
        {
            BytesRead += HttpStream.EndRead(ar);
            NotifyObservers();
            InitializeBuffer();
            BeginRead();
        }

        private void NotifyObservers()
        {
            Observers.ForEach(x => x.OnNext(Encoding.UTF8.GetString(ReadBuffer.Take(BytesRead).ToArray())));
        }

        private bool IsTransmissionComplete()
        {
            return ReadBuffer[BytesRead - 2] == 13 && ReadBuffer[BytesRead - 1] == 10;
        }

        private void InitializeBuffer()
        {
            ReadBuffer = new byte[BufferSize];
            BytesRead = 0;
        }

        public void Stop()
        {
            Running = false;
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            Observers.Add(observer);
            return observer as IDisposable;
        }

        public HttpStreamReader(Stream stream)
        {
            HttpStream = stream;
            BufferSize = 1024*1024;
            InitializeBuffer();
            Observers = new ConcurrentBag<IObserver<string>>();
        }

        public static HttpStreamReader StartNew(Stream stream, IObserver<string> observer)
        {
            var reader = new HttpStreamReader(stream);
            reader.Subscribe(observer);
            reader.Start();
            return reader;
        }

        public void Dispose()
        {
            Stop();
            Observers.ForEach(x => x.OnCompleted());
            HttpStream = null;
        }
    }
}