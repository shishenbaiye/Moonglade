using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moonglade.Core.CategoryFeature;
using Moonglade.Core.PostFeature;
using Moonglade.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Moonglade.Web.Pages.Admin
{
    public class EditPostModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly ITimeZoneResolver _timeZoneResolver;

        public PostEditModel ViewModel { get; set; }

        public EditPostModel(IMediator mediator, ITimeZoneResolver timeZoneResolver)
        {
            _mediator = mediator;
            _timeZoneResolver = timeZoneResolver;
            ViewModel = new()
            {
                IsPublished = false,
                Featured = false,
                EnableComment = true,
                ExposedToSiteMap = true,
                FeedIncluded = true
            };
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id is null)
            {
                var cats1 = await _mediator.Send(new GetCategoriesQuery());
                if (cats1.Count > 0)
                {
                    var cbCatList = cats1.Select(p =>
                        new CategoryCheckBox
                        {
                            Id = p.Id,
                            DisplayText = p.DisplayName,
                            IsChecked = false
                        });

                    ViewModel.CategoryList = cbCatList.ToList();
                }

                return Page();
            }

            var post = await _mediator.Send(new GetPostByIdQuery(id.Value));
            if (null == post) return NotFound();

            ViewModel = new()
            {
                PostId = post.Id,
                IsPublished = post.IsPublished,
                EditorContent = post.RawPostContent,
                Author = post.Author,
                Slug = post.Slug,
                Title = post.Title,
                EnableComment = post.CommentEnabled,
                ExposedToSiteMap = post.ExposedToSiteMap,
                FeedIncluded = post.IsFeedIncluded,
                LanguageCode = post.ContentLanguageCode,
                Abstract = post.ContentAbstract.Replace("\u00A0\u2026", string.Empty),
                Featured = post.Featured,
                IsOriginal = post.IsOriginal,
                OriginLink = post.OriginLink,
                HeroImageUrl = post.HeroImageUrl
            };

            if (post.PubDateUtc is not null)
            {
                ViewModel.PublishDate = _timeZoneResolver.ToTimeZone(post.PubDateUtc.GetValueOrDefault());
            }

            var tagStr = post.Tags
                .Select(p => p.DisplayName)
                .Aggregate(string.Empty, (current, item) => current + item + ",");

            tagStr = tagStr.TrimEnd(',');
            ViewModel.Tags = tagStr;

            var cats2 = await _mediator.Send(new GetCategoriesQuery());
            if (cats2.Count > 0)
            {
                var cbCatList = cats2.Select(p =>
                    new CategoryCheckBox
                    {
                        Id = p.Id,
                        DisplayText = p.DisplayName,
                        IsChecked = post.Categories.Any(q => q.Id == p.Id)
                    });
                ViewModel.CategoryList = cbCatList.ToList();
            }

            return Page();
        }
    }
}
