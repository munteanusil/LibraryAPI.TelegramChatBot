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
    public class BookRepository : IBookRepository
    {
        private readonly LibraryContext _libraryContext;

        public BookRepository(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }
        public async Task CreateBook(Book book, CancellationToken ct = default)
        {
           _libraryContext.Books.AddAsync(book,ct);
           _libraryContext.SaveChangesAsync();
        }

        public async Task DeleteBook(int id, CancellationToken ct = default)
        {
            var bookToRemove = await _libraryContext.Books.FirstOrDefaultAsync(a => a.Id == id);
            if (bookToRemove == null)
            {
                throw new KeyNotFoundException();
            }
            _libraryContext.Books.Remove(bookToRemove);
            await _libraryContext.SaveChangesAsync();

        }

        public Task<Book> GetBookById(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PaginetedList<Book>> GetBooks(int page, int pageSize, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateBook(Book book, CancellationToken ct = default)
        {
            _libraryContext.Books.Update(book);
            await _libraryContext.SaveChangesAsync();
        }
    }
}
