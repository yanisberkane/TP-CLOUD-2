﻿using Microsoft.AspNetCore.Http.HttpResults;
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
    }
}
