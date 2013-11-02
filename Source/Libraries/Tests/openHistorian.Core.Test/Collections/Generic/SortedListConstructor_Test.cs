﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using openHistorian;

namespace GSF.Collections.Generic
{
    [TestFixture]
    class SortedListConstructor_Test
    {
        [Test]
        public void Test1()
        {
            DebugStopwatch sw = new DebugStopwatch();

            for (int max = 10; max < 10000; max *= 2)
            {
                Action add1 = () =>
                    {
                        SortedList<int, int> list = new SortedList<int, int>();
                        for (int x = 0; x < max; x++)
                        {
                            list.Add(x, x);
                        }
                    };

                Action add2 = () =>
                    {
                        List<int> keys = new List<int>(max);
                        List<int> values = new List<int>(max);

                        for (int x = 0; x < max; x++)
                        {
                            keys.Add(x);
                            values.Add(x);
                        }

                        var sl = SortedListConstructor.Create(keys, values);

                    };

                //var makeList = new SortedListConstructorUnsafe<int, int>();
                //Action add3 = () =>
                //{
                //    List<int> keys = new List<int>(max);
                //    List<int> values = new List<int>(max);

                //    for (int x = 0; x < max; x++)
                //    {
                //        keys.Add(x);
                //        values.Add(x);
                //    }

                //    var sl = makeList.Create(keys, values);
                //    //var sl = SortedListConstructor.CreateUnsafe(keys, values);

                //};
                Console.WriteLine("Old Method " + max + " " + sw.TimeEvent(add1) * 1000000);
                Console.WriteLine("New Method " + max + " " + sw.TimeEvent(add2) * 1000000);
                //Console.WriteLine("Unsafe Method " + max + " " + sw.TimeEvent(add3) * 1000000);
            }


        }
    }
}
