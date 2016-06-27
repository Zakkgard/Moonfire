namespace Moonfire.Core.Collections.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class ObjectPoolTests
    {
        private ObjectPool<string> sut;

        [TestInitialize]
        public void Setup()
        {
            sut = new ObjectPool<string>(() => "");
        }

        [TestMethod]
        public void ShouldReturnEmptyStringIfNothingWasIn()
        {
            string result = sut.GetObject();

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void ShouldReturnEmptyStringWhenAskedTwice()
        {
            Assert.AreEqual("", sut.GetObject());
            Assert.AreEqual("", sut.GetObject());
        }

        [TestMethod]
        public void ShouldReturnObjectThatWasAlreadyPutInThePull()
        {
            sut.PutObject("1");

            Assert.AreEqual("1", sut.GetObject());
        }

        [TestMethod]
        public void ShouldReturnObjectsInReveresedOrderOfTheOneTheyWarePutIn()
        {
            sut.PutObject("1");
            sut.PutObject("2");

            Assert.AreEqual("2", sut.GetObject());
            Assert.AreEqual("1", sut.GetObject());
        }

        [TestMethod]
        public void ShouldReturnEmptyStringIfAllOjbectedThatInThePullArePulledOut()
        {
            sut.PutObject("1");
            sut.PutObject("2");

            sut.GetObject();
            sut.GetObject();

            Assert.AreEqual("", sut.GetObject());
        }
    }
}