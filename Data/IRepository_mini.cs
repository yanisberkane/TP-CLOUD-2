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
        Task<List<PostReadDTO>> GetPostsIndex(int pageNumber, int pageSize);
        Task<int> GetPostsCount();
        Task Add(Post post);
        Task IncrementPostLike(Guid id);
        Task IncrementPostDislike(Guid id);

        // Comments
        Task<List<CommentReadDTO>> GetCommentsIndex(Guid id);
        Task AddComments(Comment comment);
        Task IncrementCommentLike(Guid id);
        Task IncrementCommentDislike(Guid id);
    }
}
