﻿using Moonglade.Web.Models;
using NUnit.Framework;
using System;

namespace Moonglade.Web.Tests.Models
{
    [TestFixture]
    public class NotEmptyAttributeTests
    {
        [Test]
        public void IsValid_EmptyGUID()
        {
            var notEmptyAttribute = new NotEmptyAttribute();
            var isValid = notEmptyAttribute.IsValid(Guid.Empty);
            Assert.IsFalse(isValid);
        }

        [Test]
        public void IsValid_NotEmptyGUID()
        {
            var notEmptyAttribute = new NotEmptyAttribute();
            var isValid = notEmptyAttribute.IsValid(Guid.NewGuid());
            Assert.IsTrue(isValid);
        }
    }
}
