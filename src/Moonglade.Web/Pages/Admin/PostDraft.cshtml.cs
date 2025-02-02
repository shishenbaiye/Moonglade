using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moonglade.Core.PostFeature;
using Moonglade.Data.Spec;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moonglade.Web.Pages.Admin
{
    public class PostDraftModel : PageModel
    {
        private readonly IMediator _mediator;
        public IReadOnlyList<PostSegment> PostSegments { get; set; }

        public PostDraftModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task OnGet()
        {
            PostSegments = await _mediator.Send(new ListPostSegmentByStatusQuery(PostStatus.Draft));
        }
    }
}
