﻿using System.Collections.Generic;
using System.Diagnostics;
using Machine.Specifications;
using Symbiote.Core.Hashing;

namespace Core.Tests.Distributor.Basic
{
    public abstract class with_simple_distributor_of_lists
    {
        protected static int listCount { get; set; }
        protected static List<List<long>> lists { get; set; }
        protected static LoadBalancer<List<long>> LoadBalancer { get; set; }
        protected static Stopwatch treeWatch { get; set; }
        protected static Stopwatch fetchTimer { get; set; }
        protected static double totalFetched { get; set; }
        protected static double averageFetchTime { get; set; }
        protected static double totalFetchTime { get; set; }

        private Establish context = () =>
                                        {
                                            listCount = 10;
                                            lists = new List<List<long>>(listCount);
                                            LoadBalancer = new LoadBalancer<List<long>>(1000);

                                            treeWatch = Stopwatch.StartNew();
                                            for (int i = 0; i < listCount; i++)
                                            {
                                                var list = new List<long>(1000);
                                                lists.Add(list);
                                                LoadBalancer.AddNode(i.ToString(), list);
                                            }
                                            treeWatch.Stop();
                                        };

        protected static void Write(long value)
        {
            fetchTimer = Stopwatch.StartNew();
            var list = LoadBalancer.GetNode(value);
            fetchTimer.Stop();
            totalFetched++;
            totalFetchTime += fetchTimer.ElapsedMilliseconds;
            list.Add(value);
        }
    }
}