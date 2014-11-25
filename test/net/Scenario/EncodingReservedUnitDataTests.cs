﻿using System;
using System.Data.Services.Client;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.MediaServices.Client.Tests.Common;
using System.Linq;

namespace Microsoft.WindowsAzure.MediaServices.Client.Tests
{
    [TestClass]
    public class EncodingReservedUnitDataTests
    {

        private CloudMediaContext _dataContext;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void SetupTest()
        {
            _dataContext = WindowsAzureMediaServicesTestConfiguration.CreateCloudMediaContext();
        }

        [TestMethod]
        [Priority(1)]
        public void ReservedUnitCollectionShouldNotBeNullOrEmpty()
        {
            var encodingReservedUnits = _dataContext.EncodingReservedUnits;
            Assert.IsNotNull(encodingReservedUnits);
            Assert.IsTrue(encodingReservedUnits.Count() == 1);
            Assert.IsTrue(encodingReservedUnits.FirstOrDefault().CurrentReservedUnits <= encodingReservedUnits.FirstOrDefault().MaxReservableUnits);
        }

        [TestMethod]
        [Priority(1)]
        public void UpdateBasicReservedEncodingUnitToOneRU()
        {
            var encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.FirstOrDefault();
            var initialReserveUnitType = encodingBasicReservedUnit.ReservedUnitType;
            var initialEncodingRUcount = encodingBasicReservedUnit.CurrentReservedUnits;
            encodingBasicReservedUnit.ReservedUnitType = (int)ReservedUnitType.Basic;
            encodingBasicReservedUnit.Update();
            Assert.AreEqual(encodingBasicReservedUnit.ReservedUnitType, (int)ReservedUnitType.Basic,
            "Expecting Encoding ReservedUnit to be of Basic Type");
            encodingBasicReservedUnit.CurrentReservedUnits = 1;
            encodingBasicReservedUnit.Update();
            encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.FirstOrDefault();
            Assert.AreEqual(encodingBasicReservedUnit.CurrentReservedUnits, 1,
                "Expecting Encoding ReservedUnit to be 1");
            encodingBasicReservedUnit.CurrentReservedUnits = initialEncodingRUcount;
            encodingBasicReservedUnit.ReservedUnitType = initialReserveUnitType;
            encodingBasicReservedUnit.Update();
            encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.FirstOrDefault();
            Assert.AreEqual(encodingBasicReservedUnit.CurrentReservedUnits, initialEncodingRUcount,
                "Expecting Encoding ReservedUnit to have decreased again");
        }

        [TestMethod]
        [Priority(1)]
        public void UpdateBasicReservedEncodingUnitToMaxRU()
        {
            var encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.FirstOrDefault();
            var initialReservedUnitCount = encodingBasicReservedUnit.CurrentReservedUnits;
            encodingBasicReservedUnit.CurrentReservedUnits = encodingBasicReservedUnit.MaxReservableUnits;
            encodingBasicReservedUnit.Update();
            encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.FirstOrDefault();
            Assert.AreEqual(encodingBasicReservedUnit.CurrentReservedUnits, encodingBasicReservedUnit.MaxReservableUnits,
                "Expecting Encoding ReservedUnit to have increased to Max");
            encodingBasicReservedUnit.CurrentReservedUnits = initialReservedUnitCount;
            encodingBasicReservedUnit.Update();
            encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.FirstOrDefault();
            Assert.AreEqual(encodingBasicReservedUnit.CurrentReservedUnits, initialReservedUnitCount,
                "Expecting Encoding ReservedUnit to have decreased again to initialCount from Max");
        }

        [TestMethod]
        [Priority(1)]
        [ExpectedException(typeof(DataServiceRequestException))]
        public void UpdateBasicReservedEncodingUnitToMoreThanMaxRU()
        {
            try
            {
                var encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.FirstOrDefault();
                var initialReservedUnitCount = encodingBasicReservedUnit.CurrentReservedUnits;
                encodingBasicReservedUnit.CurrentReservedUnits = encodingBasicReservedUnit.MaxReservableUnits + 1;
                encodingBasicReservedUnit.Update();
            }
            catch (DataServiceRequestException ex)
            {
                Assert.IsTrue(ex.ToString().Contains("can only reserve up to"));
                throw;
            }
        }

        [TestMethod]
        [Priority(1)]
        public void UpdateBasicReservedEncodingUnitToSameNumberOfRU()
        {
            var encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.FirstOrDefault();
            var initialReservedUnitsCount = encodingBasicReservedUnit.CurrentReservedUnits;
            encodingBasicReservedUnit.CurrentReservedUnits = initialReservedUnitsCount;
            encodingBasicReservedUnit.Update();
            encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.FirstOrDefault();
            Assert.AreEqual(encodingBasicReservedUnit.CurrentReservedUnits, initialReservedUnitsCount,
                "Expecting Encoding ReservedUnit to be same number");
        }

        [TestMethod]
        [Priority(1)]
        public void UpdateBasicToStandardReservedUnitType()
        {
            try
            {
                var encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.FirstOrDefault();
                var initialReserveUnitType = encodingBasicReservedUnit.ReservedUnitType;
                var initialRUcount = encodingBasicReservedUnit.CurrentReservedUnits;
                encodingBasicReservedUnit.ReservedUnitType = (int)ReservedUnitType.Basic;
                encodingBasicReservedUnit.Update();
                Assert.AreEqual(encodingBasicReservedUnit.ReservedUnitType, (int)ReservedUnitType.Basic,
                "Expecting Encoding ReservedUnit to be of Basic Type");
                encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.Where(c => c.ReservedUnitType == (int)ReservedUnitType.Basic).FirstOrDefault();
                encodingBasicReservedUnit.ReservedUnitType = (int)ReservedUnitType.Standard;
                encodingBasicReservedUnit.CurrentReservedUnits = 1;
                encodingBasicReservedUnit.Update();
                encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.Where(c => c.ReservedUnitType == (int)ReservedUnitType.Standard).FirstOrDefault();
                Assert.AreEqual(encodingBasicReservedUnit.ReservedUnitType, (int)ReservedUnitType.Standard,
                    "Expecting Encoding ReservedUnit to be of Standard Type");
                encodingBasicReservedUnit.ReservedUnitType = initialReserveUnitType;
                encodingBasicReservedUnit.CurrentReservedUnits = initialRUcount;
                encodingBasicReservedUnit.Update();
                encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.Where(c => c.ReservedUnitType == initialReserveUnitType).FirstOrDefault();
                Assert.AreEqual(encodingBasicReservedUnit.ReservedUnitType, initialReserveUnitType,
                    "Expecting Encoding ReservedUnit to be reset to initial type");
            }
            catch (DataServiceRequestException ex)
            {
                Assert.IsTrue(ex.ToString().Contains("can only reserve up to"));
            }
        }

        [TestMethod]
        [Priority(1)]
        public void UpdateBasicToPremiumReservedUnitType()
        {
            try
            {
                var encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.FirstOrDefault();
                var initialReserveUnitType = encodingBasicReservedUnit.ReservedUnitType;
                var initialRUcount = encodingBasicReservedUnit.CurrentReservedUnits;
                encodingBasicReservedUnit.ReservedUnitType = (int)ReservedUnitType.Basic;
                encodingBasicReservedUnit.Update();
                Assert.AreEqual(encodingBasicReservedUnit.ReservedUnitType, (int)ReservedUnitType.Basic,
                    "Expecting Encoding ReservedUnit to be of Basic Type");
                encodingBasicReservedUnit =
                    _dataContext.EncodingReservedUnits.Where(c => c.ReservedUnitType == (int)ReservedUnitType.Basic)
                        .FirstOrDefault();
                encodingBasicReservedUnit.ReservedUnitType = (int)ReservedUnitType.Premium;
                encodingBasicReservedUnit.CurrentReservedUnits = 1;
                encodingBasicReservedUnit.Update();
                encodingBasicReservedUnit =
                    _dataContext.EncodingReservedUnits.Where(c => c.ReservedUnitType == (int)ReservedUnitType.Premium)
                        .FirstOrDefault();
                Assert.AreEqual(encodingBasicReservedUnit.ReservedUnitType, (int)ReservedUnitType.Premium,
                    "Expecting Encoding ReservedUnit to be of premium type");
                encodingBasicReservedUnit.ReservedUnitType = initialReserveUnitType;
                encodingBasicReservedUnit.CurrentReservedUnits = initialRUcount;
                encodingBasicReservedUnit.Update();
                encodingBasicReservedUnit =
                    _dataContext.EncodingReservedUnits.Where(c => c.ReservedUnitType == initialReserveUnitType)
                        .FirstOrDefault();
                Assert.AreEqual(encodingBasicReservedUnit.ReservedUnitType, initialReserveUnitType,
                    "Expecting Encoding ReservedUnit to be reset to initial type");
            }
            catch (DataServiceRequestException ex)
            {
                Assert.IsTrue(ex.ToString().Contains("can only reserve up to"));
            }
        }


        [TestMethod]
        [Priority(1)]
        public void UpdateAsyncBasicReservedEncodingUnitToOneRU()
        {
            var encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.FirstOrDefault();
            var initialEncodingReservedUnitType = encodingBasicReservedUnit.ReservedUnitType;
            var initialReservedUnitCount = encodingBasicReservedUnit.CurrentReservedUnits;
            encodingBasicReservedUnit.ReservedUnitType = (int)ReservedUnitType.Basic;
            encodingBasicReservedUnit.Update();
            Assert.AreEqual(encodingBasicReservedUnit.ReservedUnitType, (int)ReservedUnitType.Basic,
            "Expecting Encoding ReservedUnit to be of Basic Type");
            encodingBasicReservedUnit = _dataContext.EncodingReservedUnits.Where(c => c.ReservedUnitType == (int)ReservedUnitType.Basic).FirstOrDefault();
            encodingBasicReservedUnit.CurrentReservedUnits = 1;
            encodingBasicReservedUnit = encodingBasicReservedUnit.UpdateAsync().Result;
            Assert.AreEqual(encodingBasicReservedUnit.CurrentReservedUnits, 1,
                "Expecting Encoding ReservedUnit to have increased");
            encodingBasicReservedUnit.CurrentReservedUnits = initialReservedUnitCount;
            encodingBasicReservedUnit.ReservedUnitType = initialEncodingReservedUnitType;
            encodingBasicReservedUnit = encodingBasicReservedUnit.UpdateAsync().Result;
            Assert.AreEqual(encodingBasicReservedUnit.CurrentReservedUnits, initialReservedUnitCount,
                "Expecting Encoding ReservedUnit to have decreased again");
        }
    }
}
