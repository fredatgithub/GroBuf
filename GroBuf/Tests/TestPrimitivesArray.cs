﻿using NUnit.Framework;

namespace GroBuf.Tests
{
    [TestFixture]
    public class TestPrimitivesArray
    {
        private Serializer serializer;

        [SetUp]
        public void SetUp()
        {
            serializer = new Serializer();
        }

        [Test]
        public void TestArrayNull()
        {
            var data = serializer.Serialize<int[]>(null);
            var array = serializer.Deserialize<int[]>(data);
            Assert.NotNull(array);
            Assert.AreEqual(0, array.Length);
        }

        [Test]
        public void TestArrayLength0()
        {
            var data = serializer.Serialize(new int[0]);
            var array = serializer.Deserialize<int[]>(data);
            Assert.NotNull(array);
            Assert.AreEqual(0, array.Length);
        }

        [Test]
        public void TestArrayNullInProp()
        {
            var data = serializer.Serialize(new Z());
            var obj = serializer.Deserialize<Z>(data);
            Assert.IsNotNull(obj);
            Assert.IsNull(obj.Array);
        }

        [Test]
        public void TestArrayLength0InProp()
        {
            var data = serializer.Serialize(new Z{Array = new int[0]});
            var obj = serializer.Deserialize<Z>(data);
            Assert.IsNotNull(obj);
            Assert.IsNotNull(obj.Array);
            Assert.AreEqual(0, obj.Array.Length);
        }

        public class Z
        {
            public int[] Array { get; set; }
        }
    }
}