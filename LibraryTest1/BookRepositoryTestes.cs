using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Library.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Tests
{
    public class BookRepositoryTestes
    {
        [Fact]
        public async Task CreateBookShouldSaveAnewBookInDb()
        {
            var oprtions = new DbContextOptionsBuilder<LibraryContext>()
                          .UseInMemoryDatabase(Guid.NewGuid().ToString().ToString())
                          .Options;

            using var libContext = new LibraryContext(oprtions);
            IBookRepository repo = new BookRepository(libContext);
            var bookToAdd = new Book()
            {
                Title = "TestBook",
                ISBN = "9781234567897",
                Stock = 3,
            };
            Assert.True(bookToAdd.Id == 0);

            await repo.CreateBook(bookToAdd);

            Assert.True(bookToAdd.Id > 0);
            var bookCreated = await libContext.Books.FirstOrDefaultAsync(b => b.Id == bookToAdd.Id);
            Assert.True(bookCreated != null);
        }

        [Fact]
        public async Task UpdateBookShouldSaveUpdateIfExists()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                  .UseInMemoryDatabase(Guid.NewGuid().ToString())
                  .Options;

            using var libContext = new LibraryContext(options);
            IBookRepository repo = new BookRepository(libContext);
            var updatedBook = new Book
            {
                Title = "UpdatedTitle",
                ISBN = "012Updated",
                Stock = 3
            };

            var bookToAdd = new Book
            {
                Title = "BookTest",
                ISBN = "012Test",
                Stock = 2
            };

            await libContext.AddAsync(bookToAdd);
            await libContext.SaveChangesAsync();

            Assert.True(bookToAdd.Id != 0);
            bookToAdd.Title = updatedBook.Title;
            bookToAdd.ISBN = updatedBook.ISBN;
            bookToAdd.Stock = updatedBook.Stock;
            await repo.UpdateBook(bookToAdd);

            var bookFromDb = await libContext.Books.FirstOrDefaultAsync(b => b.Id == bookToAdd.Id);
            Assert.True(bookFromDb != null);
            Assert.Equal(updatedBook.Title, bookFromDb.Title);
            Assert.Equal(updatedBook.ISBN, bookFromDb.ISBN);
            Assert.Equal(updatedBook.Stock, bookFromDb.Stock);
        
        }


        [Fact]
        public async Task UpdateBookShouldThrowIfBookDoesNotExists()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            using var libContext = new LibraryContext(options);
            IBookRepository repo = new BookRepository(libContext);
            var bookToAdd = new Book()
            {
                Id = 1,
                Title = "TestBook",
                ISBN = "012Test",
                Stock = 3,
            };

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => repo.UpdateBook(bookToAdd));
        }

        [Fact]
        public async Task DeleteBookShouldThrowIfBookDoesNotexists()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var libContext = new LibraryContext(options);
            IBookRepository repo = new BookRepository(libContext);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.DeleteBook(1));
        }


        [Fact]
        public async Task DeleteBookShouldDeleteIfAuthorExists()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            using var libContext = new LibraryContext(options);
            IAuthorRepository repo = new AuthorRepository(libContext);
            var updatedBook = new Book
            {
                Title = "UpdatedValues",
                ISBN = "UpdateValues",
                Stock = 3
            };

            var dummyBook = new Book
            {
                Title = "Test",
                ISBN = "TestLast",
                Stock = 4,

            };
            await libContext.AddAsync(dummyBook);
            await libContext.SaveChangesAsync();

            Assert.True(dummyBook.Id > 0);
            Assert.True(await libContext.Books.FirstOrDefaultAsync(a => a.Id == dummyBook.Id) != null);

            await repo.DeleteAuthor(dummyBook.Id);
            Assert.True(await libContext.Books.FirstOrDefaultAsync(a => a.Id == dummyBook.Id) == null);

        }
    }
}
