using Moonglade.Auditing;
using Moonglade.Data.Entities;
using Moonglade.Data.Infrastructure;
using Moonglade.Pages;
using Moq;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Moonglade.Tests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class PageServiceTests
    {
        private MockRepository _mockRepository;

        private Mock<IRepository<PageEntity>> _mockPageRepository;
        private Mock<IBlogAudit> _mockBlogAudit;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new(MockBehavior.Strict);

            _mockPageRepository = _mockRepository.Create<IRepository<PageEntity>>();
            _mockBlogAudit = _mockRepository.Create<IBlogAudit>();
        }

        private readonly PageEntity _fakePageEntity = new()
        {
            CreateTimeUtc = new(996, 9, 6),
            CssContent = ".pdd .work { border: 996px solid #007; }",
            HideSidebar = true,
            HtmlContent = "<p>PDD is evil</p>",
            Id = Guid.Empty,
            IsPublished = true,
            MetaDescription = "PDD is evil",
            Slug = "pdd-IS-evil",
            Title = "PDD is Evil "
        };

        private PageService CreatePageService()
        {
            return new(
                _mockPageRepository.Object,
                _mockBlogAudit.Object);
        }

        [Test]
        public async Task GetAsync_PageId()
        {
            _mockPageRepository.Setup(p => p.GetAsync(It.IsAny<Guid>()))
                .Returns(ValueTask.FromResult(_fakePageEntity));

            var svc = CreatePageService();
            var page = await svc.GetAsync(Guid.Empty);

            Assert.IsNotNull(page);
            Assert.AreEqual("PDD is Evil", page.Title);
            Assert.AreEqual("pdd-is-evil", page.Slug);
        }

        [Test]
        public async Task GetAsync_PageSlug()
        {
            _mockPageRepository.Setup(p => p.GetAsync(It.IsAny<Expression<Func<PageEntity, bool>>>()))
                .Returns(Task.FromResult(_fakePageEntity));

            var svc = CreatePageService();
            var page = await svc.GetAsync("pdd-is-evil");

            Assert.IsNotNull(page);
            Assert.AreEqual("PDD is Evil", page.Title);
            Assert.AreEqual("pdd-is-evil", page.Slug);
        }

        [Test]
        public void RemoveScriptTagFromHtml()
        {
            var html = @"<p>Microsoft</p><p>Rocks!</p><p>Azure <br /><script>console.info('hey');</script><img src=""a.jpg"" /> The best <span>cloud</span>!</p>";
            var output = PageService.RemoveScriptTagFromHtml(html);

            Assert.IsTrue(output == @"<p>Microsoft</p><p>Rocks!</p><p>Azure <br /><img src=""a.jpg"" /> The best <span>cloud</span>!</p>");
        }
    }
}
