using Xunit;
using FluentAssert;

namespace ProphetsWay.Utilities.Tests
{
    public class StreamerTests
    {
        [Fact]
        public void TestStreamifyAndReadToEnd()
        {
            const int size = 1024 * 1024 * 3;
            var arr = new byte[size];

            for (var i = 0; i < size; i++)
                arr[i] = (byte)(i % 256);


            var stream = arr.Streamify();
            var newArr = stream.ReadToEnd();

            //new array shouldn't be the same as the original
            newArr.ShouldNotBeEqualTo(arr);

            //lengths should match
            newArr.Length.ShouldBeEqualTo(arr.Length);

            //as should two random bytes
            newArr[123].ShouldBeEqualTo(arr[123]);
            newArr[size - 123].ShouldBeEqualTo(arr[size - 123]);
        }
    }
}
