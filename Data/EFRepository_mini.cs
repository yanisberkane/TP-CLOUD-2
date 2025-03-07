using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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


        public virtual async Task<Results<Ok<PostReadDTO>, NotFound>> GetAPIPost(Guid id)
        {
            var post = await _context.Set<Post>().FindAsync(id);
            if (post == null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(new PostReadDTO(post));
        }

        // Post methods
        public virtual async Task<Results<Ok<List<PostReadDTO>>, NotFound>> GetPostsIndex(int pageNumber, int pageSize)
        {
            var posts = await _context.Set<Post>()
                .OrderBy(p => p.Created)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostReadDTO(p))
                .ToListAsync();

            if (posts.Count == 0)
            {
                return TypedResults.NotFound();
            }
            
            return TypedResults.Ok(posts);
        }

        public virtual async Task<Results<Ok, NotFound>> IncrementPostLike(Guid id)
        {
            var post = await _context.Set<Post>().FindAsync(id);
            if (post == null)
            {
                return TypedResults.NotFound();
            }
            post!.IncrementLike();
            await _context.SaveChangesAsync();
            return TypedResults.Ok();
        }

        public virtual async Task<Results<Ok, NotFound>> IncrementPostDislike(Guid id)
        {
            var post = await _context.Set<Post>().FindAsync(id);
            if (post == null)
            {
                return TypedResults.NotFound();
            }
            post!.IncrementDislike();
            await _context.SaveChangesAsync();
            return TypedResults.Ok();
        }

        // Comment methods
        public virtual async Task<Results<Ok<List<CommentReadDTO>>, NotFound>> GetCommentsIndex(Guid id)
        {
            var comments = await _context.Set<Comment>()
                .Where(c => c.PostId == id)
                .OrderBy(c => c.Created)
                .Select(c => new CommentReadDTO(c))
                .ToListAsync();
            if (comments.Count == 0)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(comments);
        }

        public virtual async Task<Results<Created<CommentReadDTO>, BadRequest, InternalServerError, NotFound>> AddComments(Comment comment)
        {
            var post = await _context.Set<Post>().FindAsync(comment.PostId);
            if (post == null)
            {
                return TypedResults.NotFound();
            }
            try 
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return TypedResults.Created($"/Comments/{comment.Id}", new CommentReadDTO(comment));
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

        public virtual async Task<Results<Ok, NotFound>> IncrementCommentLike(Guid id)
        {
            var comment = await _context.Set<Comment>().FindAsync(id);
            if (comment == null)
            {
                return TypedResults.NotFound();
            }
            comment!.IncrementLike();
            await _context.SaveChangesAsync();
            return TypedResults.Ok();
        }

        public virtual async Task<Results<Ok, NotFound>> IncrementCommentDislike(Guid id)
        {
            var comment = await _context.Set<Comment>().FindAsync(id);
            if (comment == null)
            {
                return TypedResults.NotFound();
            }
            comment!.IncrementDislike();
            await _context.SaveChangesAsync();
            return TypedResults.Ok();
        }
    }
}
