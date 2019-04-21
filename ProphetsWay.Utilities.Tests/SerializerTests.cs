using System;
using System.Collections.Generic;
using System.Xml;
using FluentAssert;
using Xunit;

namespace ProphetsWay.Utilities.Tests
{
    public class SerializerTests
    {
        [Fact]
        public void SerializeStringAndBackAsByteArray()
        {
            const string input = "Hello World!";

            var s = input.SerializeAsByteArr();
            var output = s.DeserializeFromByteArr();

            output.ShouldBeEqualTo(input);
        }

        [Fact]
        public void SerializeObjectAndBackAsByteArray()
        {
            var to = new TestObject();

            var s = to.SerializeAsByteArr();

            var ds = s.DeserializeFromByteArr<TestObject>();

            ds.intProperty.ShouldBeEqualTo(to.intProperty);
            ds.boolProperty.ShouldBeEqualTo(to.boolProperty);
            ds.Timestamp.ShouldBeEqualTo(to.Timestamp);

            ds.dictProperty[1].ShouldBeEqualTo(to.dictProperty[1]);
            ds.dictProperty[2].ShouldBeEqualTo(to.dictProperty[2]);
            ds.dictProperty[3].ShouldBeEqualTo(to.dictProperty[3]);
        }

        [Fact]
        public void SerializeObjectAndBackAsXml()
        {
            var to = new XmlFriendlyTestObject();

            var xml = to.SerializeAsXml();
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var ds = doc.DeserializeFromXml<XmlFriendlyTestObject>();

            ds.intProperty.ShouldBeEqualTo(to.intProperty);
            ds.boolProperty.ShouldBeEqualTo(to.boolProperty);
            ds.Timestamp.ShouldBeEqualTo(to.Timestamp);
        }


        [Serializable]
        public class XmlFriendlyTestObject
        {
            public XmlFriendlyTestObject()
            {
                intProperty = 2345677;
                boolProperty = true;
                Timestamp = DateTime.Now;
            }

            public int intProperty { get; set; }

            public bool boolProperty { get; set; }

            public DateTime Timestamp { get; set; }
        }


        [Serializable]
        public class TestObject
        {
            public TestObject()
            {
                dictProperty = new Dictionary<int, string>();
                dictProperty.Add(1, "one");
                dictProperty.Add(2, "two");
                dictProperty.Add(3, "three");

                intProperty = 2345677;
                boolProperty = true;
                Timestamp = DateTime.Now;
            }

            public int intProperty { get; set; }

            public bool boolProperty { get; set; }

            public Dictionary<int, string> dictProperty { get; set; }

            public DateTime Timestamp { get; set; }

        }

    }
}
