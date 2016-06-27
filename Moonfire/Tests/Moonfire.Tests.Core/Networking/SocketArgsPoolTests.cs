namespace Moonfire.Core.Networking.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net.Sockets;

    [TestClass()]
    public class SocketArgsPoolTests
    {
        [TestMethod()]
        public void ShouldReturnNonNullSocketArgs()
        {
            SocketAsyncEventArgs socketArgs = SocketArgsPool.GetSocketArgs();

            Assert.IsNotNull(socketArgs);
        }

        [TestMethod()]
        public void ShouldReleaseSocketArgsAndReturnItWhenGettingMoreSocketArgs()
        {
            SocketAsyncEventArgs socketArgs = SocketArgsPool.GetSocketArgs();
            SocketArgsPool.ReleaseSocketArgs(socketArgs);

            SocketAsyncEventArgs result = SocketArgsPool.GetSocketArgs();
            Assert.AreSame(socketArgs, result);
            Assert.AreEqual(null, result.AcceptSocket);
            Assert.AreEqual(null, result.BufferList);
            Assert.AreEqual(false, result.DisconnectReuseSocket);
            Assert.AreEqual(null, result.RemoteEndPoint);
            Assert.AreEqual(null, result.SendPacketsElements); ;
            Assert.AreEqual(TransmitFileOptions.UseDefaultWorkerThread, result.SendPacketsFlags);
            Assert.AreEqual(0, result.SendPacketsSendSize);
            Assert.AreEqual(SocketError.Success, result.SocketError);
            Assert.AreEqual(SocketFlags.None, result.SocketFlags);
            Assert.AreEqual(null, result.UserToken);
        }

        [TestMethod()]
        public void ShouldNotFailWhenTryingToReleaseNull()
        {
            SocketArgsPool.ReleaseSocketArgs(null);
        }
    }
}