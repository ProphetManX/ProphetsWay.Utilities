using System;
using Xunit;

namespace ProphetsWay.Utilities.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var b = new blarg();

            b.SerializeAsByteArrToFile("test.txt");
        }


        public class blarg
        {
            public string name { get; }
            public int intValue { get; set; }
            public DateTime timestamp { get; set; }
        }
    }
}
