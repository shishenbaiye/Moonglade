﻿using MediatR;
using Moonglade.Data;
using Moonglade.Data.Entities;
using Moonglade.Data.Infrastructure;
using Moonglade.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Moonglade.Comments
{
    public class ReplyCommentCommand : IRequest<CommentReply>
    {
        public ReplyCommentCommand(Guid commentId, string replyContent)
        {
            CommentId = commentId;
            ReplyContent = replyContent;
        }

        public Guid CommentId { get; set; }

        public string ReplyContent { get; set; }
    }

    public class ReplyCommentCommandHandler : IRequestHandler<ReplyCommentCommand, CommentReply>
    {
        private readonly IBlogAudit _audit;
        private readonly IRepository<CommentEntity> _commentRepo;
        private readonly IRepository<CommentReplyEntity> _commentReplyRepo;

        public ReplyCommentCommandHandler(
            IBlogAudit audit, IRepository<CommentEntity> commentRepo, IRepository<CommentReplyEntity> commentReplyRepo)
        {
            _audit = audit;
            _commentRepo = commentRepo;
            _commentReplyRepo = commentReplyRepo;
        }

        public async Task<CommentReply> Handle(ReplyCommentCommand request, CancellationToken cancellationToken)
        {
            var cmt = await _commentRepo.GetAsync(request.CommentId);
            if (cmt is null) throw new InvalidOperationException($"Comment {request.CommentId} is not found.");

            var id = Guid.NewGuid();
            var model = new CommentReplyEntity
            {
                Id = id,
                ReplyContent = request.ReplyContent,
                CreateTimeUtc = DateTime.UtcNow,
                CommentId = request.CommentId
            };

            await _commentReplyRepo.AddAsync(model);

            var reply = new CommentReply
            {
                CommentContent = cmt.CommentContent,
                CommentId = request.CommentId,
                Email = cmt.Email,
                Id = model.Id,
                PostId = cmt.PostId,
                PubDateUtc = cmt.Post.PubDateUtc.GetValueOrDefault(),
                ReplyContent = model.ReplyContent,
                ReplyContentHtml = ContentProcessor.MarkdownToContent(model.ReplyContent, ContentProcessor.MarkdownConvertType.Html),
                ReplyTimeUtc = model.CreateTimeUtc,
                Slug = cmt.Post.Slug,
                Title = cmt.Post.Title
            };

            await _audit.AddEntry(BlogEventType.Content, BlogEventId.CommentReplied, $"Replied comment id '{request.CommentId}'");
            return reply;
        }
    }
}
