using System;
using System.Net;
using FluentAssert;
using Xunit;

namespace ProphetsWay.Utilities.Tests
{
    public class ParserTests
    {
        [Fact]
        public void GetValueReturnsNullableValues()
        {
            var now = DateTime.Now;
            var str = now.ToString();

            var dt = str.GetValue<DateTime?>();

            dt?.Year.ShouldBeEqualTo(now.Year);
            dt?.Month.ShouldBeEqualTo(now.Month);
            dt?.Day.ShouldBeEqualTo(now.Day);
            dt?.Hour.ShouldBeEqualTo(now.Hour);
            dt?.Minute.ShouldBeEqualTo(now.Minute);
            dt?.Second.ShouldBeEqualTo(now.Second);

            var nIntStr = "1234";
            var ni = nIntStr.GetValue<int?>();

            ni?.ShouldBeEqualTo(1234);
        }

        [Theory]
        [InlineData("4-2-19")]
        [InlineData("04-02-2019")]
        [InlineData("2019-04-02")]
        [InlineData("April-2-2019")]
        [InlineData("april-02-2019")]
        [InlineData("apr-2-2019")]
        [InlineData("2019-4-2")]
        [InlineData("4-2-2019")]
        public void GetValueReturnsDates(string dateFormatToCheck)
        {
            var dt = DateTime.Parse(dateFormatToCheck);
            var dv = dateFormatToCheck.GetValue<DateTime>();

            dv.ShouldBeEqualTo(dt);
        }

        [Theory]
        [InlineData("1:23:45")]
        [InlineData("1:2:3")]
        [InlineData("1:23")]
        [InlineData("1:23:45 pm")]
        [InlineData("1:23:45 am")]
        public void GetValueReturnsTimes(string timeFormatToCheck)
        {
            var dt = DateTime.Parse(timeFormatToCheck);
            var dv = timeFormatToCheck.GetValue<DateTime>();

            dv.ShouldBeEqualTo(dt);
        }

        [Fact]
        public void GetValueReturnsDateAndTimes()
        {
            var now = DateTime.Now;
            var nowString = now.ToString("yyyy-MM-dd HH:mm:ss");

            var dt = nowString.GetValue<DateTime>();

            dt.Year.ShouldBeEqualTo(now.Year);
            dt.Month.ShouldBeEqualTo(now.Month);
            dt.Day.ShouldBeEqualTo(now.Day);
            dt.Hour.ShouldBeEqualTo(now.Hour);
            dt.Minute.ShouldBeEqualTo(now.Minute);
            dt.Second.ShouldBeEqualTo(now.Second);
        }

        [Fact]
        public void GetValueReturnsTimeSpans()
        {
            var ts = new TimeSpan(int.MaxValue);
            var str = ts.ToString();

            var gv = str.GetValue<TimeSpan>();

            gv.ShouldBeEqualTo(ts);
        }

        [Fact]
        public void GetValueReturnsIPv4Address()
        {
            const string ipv4Str = "192.168.0.1";
            var ip = IPAddress.Parse(ipv4Str);

            var gv = ipv4Str.GetValue<IPAddress>();
            gv.ShouldBeEqualTo(ip);
        }

        [Fact]
        public void GetValueReturnsIPv6Address()
        {
            const string ipv6Str = "2001:db8:85a3:8d3:1319:8a2e:370:7348";
            var ip = IPAddress.Parse(ipv6Str);

            var gv = ipv6Str.GetValue<IPAddress>();
            gv.ShouldBeEqualTo(ip);
        }

        [Fact]
        public void GetValueReturnsIPv6AddressVariants()
        {
            const string ipv6Str1 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";
            const string ipv6Str2 = "2001:db8:85a3:0:0:8a2e:370:7334";
            const string ipv6Str3 = "2001:db8:85a3::8a2e:370:7334";
            var ip = IPAddress.Parse(ipv6Str1);

            var gv = ipv6Str1.GetValue<IPAddress>();
            gv.ShouldBeEqualTo(ip);

            gv = ipv6Str2.GetValue<IPAddress>();
            gv.ShouldBeEqualTo(ip);

            gv = ipv6Str3.GetValue<IPAddress>();
            gv.ShouldBeEqualTo(ip);
        }

        [Theory]
        [InlineData("100", (short)100)]
        [InlineData("100", (ushort)100)]
        [InlineData("100", (int) 100)]
        [InlineData("100", (uint)100)]
        [InlineData("100", (long)100)]
        [InlineData("100", (ulong)100)]
        [InlineData("100", (double)100)]
        [InlineData("100", (float)100)]

        [InlineData("true", true)]
        [InlineData("false", false)]

        [InlineData("Test", "Test")]

        [InlineData("Alpha", TestEnums.Alpha)]
        [InlineData("Anotherone", TestEnums.AnotherOne)]
        [InlineData("One More", TestEnums.One_More)]
        [InlineData("3", TestEnums.Gamma)]
        public void GetValueReturnsExpectedValue<T>(string input, T expected)
        {
            var result = input.GetValue<T>();
            result.ShouldBeEqualTo(expected);
        }

        public enum TestEnums
        {
            Alpha = 1,
            Beta,
            Gamma,
            AnotherOne,
            One_More
        }
    }
}
