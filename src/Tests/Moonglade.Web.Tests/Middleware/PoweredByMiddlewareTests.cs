﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moonglade.Web.Middleware;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moonglade.Web.Tests.Middleware
{
    [TestFixture]
    public class PoweredByMiddlewareTests
    {
        [Test]
        public async Task Invoke_HasHeader()
        {
            const string key = "X-Powered-By";

            var headersArray = new Dictionary<string, StringValues> { { key, string.Empty } };
            var headersDic = new HeaderDictionary(headersArray);
            var httpResponseMock = new Mock<HttpResponse>();
            httpResponseMock.SetupGet(r => r.Headers).Returns(headersDic);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Response).Returns(httpResponseMock.Object);

            static Task RequestDelegate(HttpContext context) => Task.CompletedTask;
            var middleware = new PoweredByMiddleware(RequestDelegate);

            await middleware.Invoke(httpContextMock.Object);

            Assert.IsNotNull(headersArray[key]);
            httpResponseMock.Verify(r => r.Headers, Times.Exactly(1));
        }
    }
}
