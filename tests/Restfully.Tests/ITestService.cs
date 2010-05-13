﻿using System;

namespace Restfully.Tests
{
    public interface  ITestService
    {
        void OneArgCall(string arg1);

        bool TwoArgCall(DateTime date, Guid id);

        ComplexReturn ComplexCall(ComplexArg arg);
    }
}