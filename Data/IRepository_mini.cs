using Microsoft.AspNetCore.Http.HttpResults;
using MVC.Models;

namespace MVC.Data
{
    public interface IRepository_mini
    {
        // API
        Task<Results<Created<PostReadDTO>, BadRequest, InternalServerError>> CreateAPIPost(Post post);
    }
}
