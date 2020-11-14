﻿using Moonglade.Core;
using Moonglade.Model.Settings;
using NUnit.Framework;

namespace Moonglade.Tests.Core
{
    [TestFixture]
    public class TagServiceTests
    {
        [TestCase(".NET Core", ExpectedResult = "dotnet-core")]
        [TestCase("C#", ExpectedResult = "csharp")]
        [TestCase("955", ExpectedResult = "955")]
        public string TestNormalizeTagName(string str)
        {
            var dic = new TagNormalization[]
            {
                new TagNormalization { Source = " ", Target = "-" },
                new TagNormalization { Source = "#", Target = "sharp" },
                new TagNormalization { Source = ".", Target = "dot" }
            };

            return TagService.NormalizeTagName(str, dic);
        }

        [TestCase("C", ExpectedResult = true)]
        [TestCase("C++", ExpectedResult = true)]
        [TestCase("C#", ExpectedResult = true)]
        [TestCase("Java", ExpectedResult = true)]
        [TestCase("996", ExpectedResult = true)]
        [TestCase(".NET", ExpectedResult = true)]
        [TestCase("C Sharp", ExpectedResult = true)]
        [TestCase("Cup<T>", ExpectedResult = false)]
        [TestCase("(1)", ExpectedResult = false)]
        [TestCase("usr/bin", ExpectedResult = false)]
        [TestCase("", ExpectedResult = false)]
        public bool TestValidateTagName(string tagDisplayName)
        {
            return TagService.ValidateTagName(tagDisplayName);
        }
    }
}
