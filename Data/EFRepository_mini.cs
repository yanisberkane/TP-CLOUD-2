using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVC.Data
{
    public class EFRepository_mini<TContext> : IRepository_mini where TContext : DbContext
    {
        protected readonly TContext _context;

        protected EFRepository_mini(TContext context)
        {
            this._context = context;
        }

        public virtual async Task<Results<Created<PostReadDTO>, BadRequest, InternalServerError>> CreateAPIPost(Post post)
        {
            try
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return TypedResults.Created($"/Posts/{post.Id}", new PostReadDTO(post));
            }
            catch (Exception ex) when (ex is DbUpdateException)
            {
                return TypedResults.BadRequest();
            }
            catch (Exception)
            {
                return TypedResults.InternalServerError();
            }
        }

        // Post methods
        public virtual async Task<List<PostReadDTO>> GetPostsIndex(int pageNumber, int pageSize)
        {
            return await _context.Set<Post>()
                .OrderBy(p => p.Created)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostReadDTO(p))
                .ToListAsync();
        }

        public virtual async Task<int> GetPostsCount()
        {
            return await _context.Set<Post>().CountAsync();
        }

        public virtual async Task Add(Post post)
        {
            _context.Add(post);
            await _context.SaveChangesAsync();
        }

        public virtual async Task IncrementPostLike(Guid id)
        {
            var post = await _context.Set<Post>().FindAsync(id);
            post!.IncrementLike();
            await _context.SaveChangesAsync();
        }

        public virtual async Task IncrementPostDislike(Guid id)
        {
            var post = await _context.Set<Post>().FindAsync(id);
            post!.IncrementDislike();
            await _context.SaveChangesAsync();
        }

        // Comment methods
        public virtual async Task<List<CommentReadDTO>> GetCommentsIndex(Guid id)
        {
            return await _context.Set<Comment>()
                .Where(c => c.PostId == id)
                .OrderBy(c => c.Created)
                .Select(c => new CommentReadDTO(c))
                .ToListAsync();
        }

        public virtual async Task AddComments(Comment comment)
        {
            var post = await _context.Set<Post>().FindAsync(comment.PostId);
            post!.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public virtual async Task IncrementCommentLike(Guid id)
        {
            var comment = await _context.Set<Comment>().FindAsync(id);
            comment!.IncrementLike();
            await _context.SaveChangesAsync();
        }

        public virtual async Task IncrementCommentDislike(Guid id)
        {
            var comment = await _context.Set<Comment>().FindAsync(id);
            comment!.IncrementDislike();
            await _context.SaveChangesAsync();
        }
    }
}
