using Library.Application.Interfaces;
using Library.Domain.Common;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Persistance
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibraryContext _libraryContext;
        private static readonly Func<LibraryContext, int, Task<Author?>> GetAuthorByIdCompiled = EF.CompileAsyncQuery((LibraryContext context, int id) => context.Authors.FirstOrDefault(a => a.Id == id));
        public AuthorRepository(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }
        public async Task CreateAuthor(Author author, CancellationToken ct = default)
        {
            await _libraryContext.Authors.AddAsync(author, ct);
            await _libraryContext.SaveChangesAsync(ct);
        }

        public async Task DeleteAuthor(int id, CancellationToken ct = default)
        {
            var authorToRemove = await _libraryContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (authorToRemove == null)
            {
                throw new KeyNotFoundException();
            }
            _libraryContext.Authors.Remove(authorToRemove);
            await _libraryContext.SaveChangesAsync();
        }

        public async Task<Author?> GetAuthorById(int id, CancellationToken ct = default) =>
           await GetAuthorByIdCompiled(_libraryContext, id);


        public async Task<PaginatedList<Author>> GetAuthors(int page, int pageSize, CancellationToken ct = default)
        {
            var total = await _libraryContext.Authors.CountAsync(ct);
            var authors = await _libraryContext.Authors
                .AsNoTracking()
                .OrderBy(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PaginatedList<Author>(authors, page, (int)Math.Ceiling((double)total / pageSize));
        }

        public async Task UpdateAuthor(Author author, CancellationToken ct = default)
        {
            if (author == null) return;

           
            var dbAuthor = await _libraryContext.Authors
                .Include(a => a.AuthorGenres)
                .FirstOrDefaultAsync(a => a.Id == author.Id, ct);

            if (dbAuthor == null) return;

            
            _libraryContext.Entry(dbAuthor).CurrentValues.SetValues(author);

            
            if (author.AuthorGenres != null)
            {
               
                var genresToRemove = dbAuthor.AuthorGenres
                    .Where(dbG => !author.AuthorGenres.Any(aG => aG.GenreId == dbG.GenreId))
                    .ToList();

            
                var genresToAdd = author.AuthorGenres
                    .Where(aG => !dbAuthor.AuthorGenres.Any(dbG => dbG.GenreId == aG.GenreId))
                    .Select(aG => new AuthorGenres 
                    {
                        AuthorId = author.Id,
                        GenreId = aG.GenreId
                    })
                    .ToList();

                if (genresToRemove.Any())
                {
                    _libraryContext.AuthorGenres.RemoveRange(genresToRemove);
                }

                if (genresToAdd.Any())
                {
                    await _libraryContext.AuthorGenres.AddRangeAsync(genresToAdd, ct);
                }
            }

            await _libraryContext.SaveChangesAsync(ct);
        }
    }
}