using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moonfire.Core.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonfire.Core.Collections.Tests
{
    [TestClass()]
    public class ObjectPoolTests
    {
        private ObjectPool<String> sut;

        [TestInitialize]
        public void Setup()
        {
            sut = new ObjectPool<String>(() => "");
        }

        [TestMethod]
        public void ShouldReturnEmptyStringIfNothingWasIn()
        {
            String result = sut.GetObject();

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void ShouldReturnEmptyStringWhenAskedTwice()
        {
            String result = sut.GetObject();

            Assert.AreEqual("", result);
            Assert.AreEqual("", result);
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