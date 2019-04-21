using Xunit;
using FluentAssert;
using static ProphetsWay.Utilities.Tests.SerializerTests;

namespace ProphetsWay.Utilities.Tests
{
    public class DuplicatorTests
    {
        [Fact]
        public void DuplicatorShouldReturnCopyOfObject()
        {
            var to = new TestObject();

            var dup = to.DuplicateObjectSerial();

            //shouldn't be the same instance
            dup.ShouldNotBeEqualTo(to);

            //but should retain all the same values
            dup.intProperty.ShouldBeEqualTo(to.intProperty);
            dup.boolProperty.ShouldBeEqualTo(to.boolProperty);
            dup.Timestamp.ShouldBeEqualTo(to.Timestamp);
            dup.dictProperty[1].ShouldBeEqualTo(to.dictProperty[1]);
            dup.dictProperty[2].ShouldBeEqualTo(to.dictProperty[2]);
            dup.dictProperty[3].ShouldBeEqualTo(to.dictProperty[3]);
        }

    }
}
