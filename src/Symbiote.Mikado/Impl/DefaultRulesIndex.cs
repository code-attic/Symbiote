/* 
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core;
using Symbiote.Core.Impl.Reflection;

namespace Symbiote.Mikado.Impl
{
    /// <summary>
    /// Out-of-the-box class that provides simple scan-and-load capability for loading
    /// rules and target types into the Rules collection
    /// </summary>
    public class DefaultRulesIndex : IRulesIndex
    {
        public DefaultRulesIndex()
        {
            Rules = new ConcurrentDictionary<Type, List<IRule>>();
            ScanAndLoadRules();
        }

        /// <summary>
        /// Gets all instances of IRule types from the IoC container and builds
        /// up a list of the target types (the "T" in IRule&lt;T&gt;) and loads them
        /// in the Rules dictionary with rules assigned to that type (as well as assigning
        /// those rules to sub-types). 
        /// </summary>
        private void ScanAndLoadRules()
        {
            Rules.Clear();
            var rules = Assimilate.GetAllInstancesOf<IRule>().Distinct().ToList();

            rules.ForEach(rule =>
            {
                var ruleTargetType = FindRuleTargetType(rule.GetType());
                if (ruleTargetType == null) return;
                var ruleTargetChildren = Reflector.GetSubTypes(ruleTargetType).ToList();

                ruleTargetChildren
                    .ForEach(domainType =>
                    {
                        List<IRule> ruleList = null;
                        if (Rules.TryGetValue(domainType, out ruleList))
                        {
                            if (!ruleList.Contains(rule))
                                ruleList.Add(rule);
                        }
                        else
                        {
                            ruleList = new List<IRule>() { rule };
                            Rules.TryAdd(domainType, ruleList);
                        }
                    });
            });
        }

        /// <summary>
        /// Finds the type in the inheritance tree that closes the generic on the given IRule
        /// and returns the generic parameter type that the given IRule type targets
        /// </summary>
        /// <param name="ruleType">IRule type</param>
        /// <returns>Type object representing the Type which the IRule targets</returns>
        private static Type FindRuleTargetType(Type ruleType)
        {
            var ruleParents = Reflector.GetInheritanceChainFor(ruleType);
            var firstRuleWithGeneric = ruleParents.Where(x => x.IsGenericType).First();
            return firstRuleWithGeneric.GetGenericArguments().First();
        }

        /// <summary>
        /// Dictionary associating IRules with Types
        /// </summary>
        public ConcurrentDictionary<Type, List<IRule>> Rules { get; set; }
    }
}
