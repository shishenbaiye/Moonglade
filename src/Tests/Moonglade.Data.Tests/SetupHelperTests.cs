﻿using Dapper;
using Moonglade.Data.Setup;
using Moq;
using Moq.Dapper;
using NUnit.Framework;
using System.Data;

namespace Moonglade.Data.Tests
{
    [TestFixture]
    public class SetupHelperTests
    {
        private MockRepository _mockRepository;

        private Mock<IDbConnection> _mockDbConnection;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new(MockBehavior.Default);
            _mockDbConnection = _mockRepository.Create<IDbConnection>();
        }

        [Test]
        public void IsFirstRun_Yes()
        {
            _mockDbConnection.SetupDapper(c => c.ExecuteScalar<int>(It.IsAny<string>(), null, null, null, null)).Returns(0);
            var setupHelper = new SetupRunner(_mockDbConnection.Object);

            var result = setupHelper.IsFirstRun();
            Assert.IsTrue(result);
        }

        [Test]
        public void IsFirstRun_No()
        {
            _mockDbConnection.SetupDapper(c => c.ExecuteScalar<int>(It.IsAny<string>(), null, null, null, null)).Returns(1);
            var setupHelper = new SetupRunner(_mockDbConnection.Object);

            var result = setupHelper.IsFirstRun();
            Assert.IsFalse(result);
        }

        [Test]
        public void SetupDatabase_OK()
        {
            _mockDbConnection.SetupDapper(c => c.Execute(It.IsAny<string>(), null, null, null, null)).Returns(996);
            var setupHelper = new SetupRunner(_mockDbConnection.Object);

            Assert.DoesNotThrow(() =>
            {
                setupHelper.SetupDatabase();
            });
        }

        [Test]
        public void ClearData_OK()
        {
            _mockDbConnection.SetupDapper(c => c.Execute(It.IsAny<string>(), null, null, null, null)).Returns(251);
            var setupHelper = new SetupRunner(_mockDbConnection.Object);

            Assert.DoesNotThrow(() =>
            {
                setupHelper.ClearData();
            });
        }

        [Test]
        public void ResetDefaultConfiguration_OK()
        {
            _mockDbConnection.SetupDapper(c => c.Execute(It.IsAny<string>(), null, null, null, null)).Returns(251);
            var setupHelper = new SetupRunner(_mockDbConnection.Object);

            Assert.DoesNotThrow(() =>
            {
                setupHelper.ResetDefaultConfiguration();
            });
        }

        [Test]
        public void InitSampleData_OK()
        {
            _mockDbConnection.SetupDapper(c => c.Execute(It.IsAny<string>(), null, null, null, null)).Returns(251);
            var setupHelper = new SetupRunner(_mockDbConnection.Object);

            Assert.DoesNotThrow(() =>
            {
                setupHelper.InitSampleData();
            });
        }

        [Test]
        public void InitFirstRun_OK()
        {
            _mockDbConnection.SetupDapper(c => c.Execute(It.IsAny<string>(), null, null, null, null)).Returns(251);
            var setupHelper = new SetupRunner(_mockDbConnection.Object);

            Assert.DoesNotThrow(() =>
            {
                setupHelper.InitFirstRun();
            });
        }

        [Test]
        public void TestDatabaseConnection_OK()
        {
            _mockDbConnection.SetupDapper(c => c.ExecuteScalar<int>(It.IsAny<string>(), null, null, null, null)).Returns(1);
            var setupHelper = new SetupRunner(_mockDbConnection.Object);

            var result = setupHelper.TestDatabaseConnection();
            Assert.AreEqual(true, result);
        }

        [Test]
        public void TestDatabaseConnection_Exception_NoLogger()
        {
            _mockDbConnection.SetupDapper(c => c.ExecuteScalar<int>(It.IsAny<string>(), null, null, null, null))
                .Throws(new DataException("996"));

            var setupHelper = new SetupRunner(_mockDbConnection.Object);
            var result = setupHelper.TestDatabaseConnection();
            Assert.AreEqual(false, result);
        }
    }
}
