﻿/* 
Copyright 2008-2010 Jim Cowart

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

namespace Symbiote.Mikado
{
    public interface IRunRules : IPublishBrokenRules
    {
        IRulesIndex RuleIndex { get; set; }
        void ApplyRules(object target);
    }

    public interface IPublishBrokenRules
    {
        IDisposable Subscribe( IListenToBrokenRules subscriber );
    }

    public interface IListenToBrokenRules
    {
        void OnBrokenRule(IBrokenRuleNotification brokenRuleNotification);
    }
}
