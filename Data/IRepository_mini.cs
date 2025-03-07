using Microsoft.AspNetCore.Http.HttpResults;
using MVC.Models;

namespace MVC.Data
{
    public interface IRepository_mini
    {
        // API
        Task<Results<Created<PostReadDTO>, BadRequest, InternalServerError>> CreateAPIPost(Post post);
        Task<Results<Ok<PostReadDTO>, NotFound>> GetAPIPost(Guid id);

        // Post
        Task<Results<Ok<List<PostReadDTO>>, NotFound>> GetPostsIndex(int pageNumber, int pageSize);

        Task<Results<Ok, NotFound>> IncrementPostLike(Guid id);
        Task<Results<Ok, NotFound>> IncrementPostDislike(Guid id);

        // Comments
        Task<Results<Ok<List<CommentReadDTO>>, NotFound>> GetCommentsIndex(Guid id);
        Task<Results<Created<CommentReadDTO>, BadRequest, InternalServerError, NotFound>> AddComments(Comment comment);
        Task<Results<Ok, NotFound>> IncrementCommentLike(Guid id);
        Task<Results<Ok, NotFound>> IncrementCommentDislike(Guid id);
    }
}
