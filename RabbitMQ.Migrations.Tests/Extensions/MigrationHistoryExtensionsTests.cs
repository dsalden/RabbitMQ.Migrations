using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMQ.Migrations.Extensions;
using RabbitMQ.Migrations.Objects.v2;
using RabbitMQ.Migrations.Tests.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMQ.Migrations.Tests.Extensions
{
    [TestClass]
    public class MigrationHistoryExtensionsTests
    {
        #region GetMigration

        [TestMethod]
        public void GetMigrationTest()
        {
            var historyRow = new MigrationHistoryRow
            {
                Prefix = "prefix",
                AppliedMigrations = new List<MigrationHistoryRowDetails>
                {
                    new MigrationHistoryRowDetails
                    {
                        Name = "Detail1",
                        Hash = "Hash1"
                    },
                    new MigrationHistoryRowDetails
                    {
                        Name = "Detail2",
                        Hash = "Hash2"
                    }
                }
            };

            var detail = historyRow.GetMigration("Detail1");

            Assert.IsNotNull(detail);
            Assert.AreEqual("Detail1", detail.Name);
        }

        [TestMethod]
        public void GetMigrationNoResultTest()
        {
            var historyRow = new MigrationHistoryRow
            {
                Prefix = "prefix",
                AppliedMigrations = new List<MigrationHistoryRowDetails>
                {
                    new MigrationHistoryRowDetails
                    {
                        Name = "Detail1",
                        Hash = "Hash1"
                    },
                    new MigrationHistoryRowDetails
                    {
                        Name = "Detail2",
                        Hash = "Hash2"
                    }
                }
            };

            var detail = historyRow.GetMigration("Detail3");

            Assert.IsNull(detail);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetMigrationNullTest()
        {
            MigrationHistoryExtensions.GetMigration(null, "prefix");
        }

        #endregion GetMigration

        #region AddMigration

        [TestMethod]
        public void AddMigrationTest()
        {
            var historyRow = new MigrationHistoryRow
            {
                Prefix = "prefix",
                AppliedMigrations = new List<MigrationHistoryRowDetails>()
            };

            historyRow.AddMigration(new KeyValuePair<string, RabbitMqMigration>("foo", new TestMigration()));

            Assert.AreEqual(1, historyRow.AppliedMigrations.Count);
            var applied = historyRow.AppliedMigrations.First();
            Assert.AreEqual("foo", applied.Name);
            Assert.IsNotNull(applied.Hash);
            Assert.IsNotNull(applied.DownOperations);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddMigrationNullTest()
        {
            MigrationHistoryExtensions.AddMigration(null, new KeyValuePair<string, RabbitMqMigration>("foo", null));
        }

        #endregion AddMigration

        #region RemoveMigration

        [TestMethod]
        public void RemoveMigrationTest()
        {
            var historyRow = new MigrationHistoryRow
            {
                Prefix = "prefix",
                AppliedMigrations = new List<MigrationHistoryRowDetails>
                {
                    new MigrationHistoryRowDetails
                    {
                        Name = "foo",
                        Hash = "Hash1"
                    },
                    new MigrationHistoryRowDetails
                    {
                        Name = "bar",
                        Hash = "Hash2"
                    }
                }
            };

            historyRow.RemoveMigration("foo");

            Assert.AreEqual(1, historyRow.AppliedMigrations.Count);
            var applied = historyRow.AppliedMigrations.First();
            Assert.AreEqual("bar", applied.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveMigrationNullTest()
        {
            MigrationHistoryExtensions.RemoveMigration(null, "foo");
        }

        #endregion RemoveMigration

        #region UpdateMigration

        [TestMethod]
        public void UpdateMigrationTest()
        {
            var historyRowDetail = new MigrationHistoryRowDetails {Name = "001_TestMigration" };

            historyRowDetail.UpdateMigration(new TestMigration());

            Assert.IsNotNull(historyRowDetail.Hash);
            Assert.IsNotNull(historyRowDetail.DownOperations);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateMigrationNullTest1()
        {
            MigrationHistoryExtensions.UpdateMigration(null, new TestMigration());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateMigrationNullTest2()
        {
            new MigrationHistoryRowDetails().UpdateMigration(null);
        }

        #endregion UpdateMigration
    }
}
