using System;
using System.Collections.Generic;

namespace Symbiote.Http.Owin
{
    public interface IRegisterApplication
    {
        IRegisterApplication DefineApplication<TApplication>(Predicate<IRequest> route)
            where TApplication : IApplication;

        IRegisterApplication DefineApplication(Predicate<IRequest> route, Type applicationType);

        IRegisterApplication DefineApplication(Predicate<IRequest> route,
                                               Action<IDictionary<string, object>, Action<string, IDictionary<string, IList<string>>, IEnumerable<object>>, Action<Exception>> application);
    }
}