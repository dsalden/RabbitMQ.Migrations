using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Client.Exceptions;
using AddUp.RabbitMQ.Fakes;
using System.Linq;
using RabbitMqConstants = RabbitMQ.Client.Constants;

namespace RabbitMQ.Migrations.Tests
{
    [TestClass]
    public class RabbitMqMigratorLockTests
    {
        [TestMethod]
        public void LockSuccessTest()
        {
            var rabbitServer = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(rabbitServer);

            var rabbitMqMigratorLock = new RabbitMqMigratorLock(connectionFactory);

            Assert.IsNotNull(rabbitMqMigratorLock);
            Assert.AreEqual(1, rabbitServer.Queues.Count);
            Assert.AreEqual(Constants.LockQueue, rabbitServer.Queues.First().Key);
        }

        [TestMethod, Ignore]
        public void LockReleaseTest()
        {
            var rabbitServer = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(rabbitServer);

            var rabbitMqMigratorLock = new RabbitMqMigratorLock(connectionFactory);
            rabbitMqMigratorLock.Dispose();

            Assert.AreEqual(0, rabbitServer.Queues.Count);
        }

        [TestMethod, Ignore]
        public void DoubleLockTest()
        {
            var rabbitServer = new RabbitServer();
            var connectionFactory = new FakeConnectionFactory(rabbitServer);

            var rabbitMqMigratorLock = new RabbitMqMigratorLock(connectionFactory);
            Assert.IsNotNull(rabbitMqMigratorLock);

            try
            {
                var rabbitMqMigratorLock2 = new RabbitMqMigratorLock(connectionFactory);
                Assert.IsNotNull(rabbitMqMigratorLock2);
                Assert.Fail();
            }
            catch (OperationInterruptedException ex)
            {
                Assert.AreEqual(RabbitMqConstants.ResourceLocked, ex.ShutdownReason.ReplyCode);
                StringAssert.StartsWith(ex.ShutdownReason.ReplyText, "RESOURCE_LOCKED - cannot obtain exclusive access to locked queue 'rmp.migrations.lock'");
            }
        }
    }
}
