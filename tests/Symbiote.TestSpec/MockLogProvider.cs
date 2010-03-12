using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using StructureMap;
using Symbiote.Core.Log;
using Symbiote.Core.Log.Impl;

namespace Symbiote.TestSpec
{
    public class MockLogger
    {
        private static Mock<ILogProvider> _provider;
        public static Dictionary<Type, Mock<ILogger>> LoggerMocks = new Dictionary<Type, Mock<ILogger>>();
        public static Mock<ILogProvider> Provider
        {
            get
            {
                _provider = _provider ?? CreateProvider();
                return _provider;
            }
        }

        public static void ConfigureForLogOf<T>()
        {
            var loggerMock = new Mock<ILogger>();
            LoggerMocks[typeof(T)] = loggerMock;
            Provider
                .Setup(x => x.GetLoggerForType<T>())
                .Returns(loggerMock.Object);
        }

        private static Mock<ILogProvider> CreateProvider()
        {
            var providerMock = new Mock<ILogProvider>();
            ObjectFactory.Configure(c => c.For<ILogProvider>().Use(providerMock.Object));
            return providerMock;
        }
    }
}
