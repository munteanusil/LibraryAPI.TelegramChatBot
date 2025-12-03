using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Library.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;



namespace Library.Tests
{
    public class AuthorrepositoryTestes
    {
        [Fact]
        public async Task CreateAuthorShouldSaveAnewAuthorInDb()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                         .UseInMemoryDatabase(Guid.NewGuid().ToString())
                         .Options;

           using var libContext = new LibraryContext(options);
           IAuthorRepository repo = new AuthorRepository(libContext);
                var authorToAdd = new Author()
                {
                    FirstName = "Test",
                    LastName = "TestLN",
                    Site ="TestSite",
                };
            Assert.True(authorToAdd.Id == 0);

            await repo.CreateAuthor(authorToAdd);

            Assert.True(authorToAdd.Id > 0);

            var authorCreadted = await libContext.Authors.FirstOrDefaultAsync(a => a.Id == authorToAdd.Id);
            Assert.True(authorCreadted != null);
        }


        [Fact]
        public async Task UpdateAuthorShouldSaveUpdateIfExists()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var libContext = new LibraryContext(options);
            IAuthorRepository repo = new AuthorRepository(libContext);
            var updatedAuthor = new Author
            {
                FirstName = "UpdatedValues",
                LastName = "UpdateValues",
                Site = "UpdatedSite"
            };

            var authorToAdd = new Author
            {
                FirstName = "Test",
                LastName = "TestLast",
                Site = "TestSite",
            
            };
            await libContext.AddAsync(authorToAdd);
            await libContext.SaveChangesAsync();

            Assert.True(authorToAdd.Id != 0);
            authorToAdd.FirstName = updatedAuthor.FirstName;
            authorToAdd.LastName = updatedAuthor.LastName;
            authorToAdd.Site = updatedAuthor.Site;
            await repo.UpdateAuthor(authorToAdd);

            var authorFromDb = await libContext.Authors.FirstOrDefaultAsync(a => a.Id == authorToAdd.Id);
            Assert.True(authorFromDb != null);
            Assert.Equal(updatedAuthor.FirstName, authorFromDb.FirstName);
            Assert.Equal(updatedAuthor.LastName, authorFromDb.LastName);
            Assert.Equal(updatedAuthor.Site, authorFromDb.Site);
        }

        [Fact]
        public async Task UpdateAuthorShouldThrowIfAuthorDoesNotExists()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var libContext = new LibraryContext(options);
            IAuthorRepository repo = new AuthorRepository(libContext);
            var authorToAdd = new Author
            {
                Id = 1,
                FirstName = "Test",
                LastName = "TestLast",
                Site = "TestSite",
         
            };

            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => repo.UpdateAuthor(authorToAdd));
        }

        [Fact]
        public async Task DeleteAuthorShouldThrowIfAuthorDoesNotExists()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var libContext = new LibraryContext(options);
            IAuthorRepository repo = new AuthorRepository(libContext);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.DeleteAuthor(1));
        }

        [Fact]
        public async Task DeleteAuthorShouldDeleteIfAuthorExists()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var libContext = new LibraryContext(options);
            IAuthorRepository repo = new AuthorRepository(libContext);
            var updatedAuthor = new Author
            {
                FirstName = "UpdatedValues",
                LastName = "UpdateValues",
                Site = "UpdatedSite"
            };

            var dummyAthor = new Author
            {
                FirstName = "Test",
                LastName = "TestLast",
                Site = "TestSite",
                
            };
            await libContext.AddAsync(dummyAthor);
            await libContext.SaveChangesAsync();

            Assert.True(dummyAthor.Id > 0);
            Assert.True(await libContext.Authors.FirstOrDefaultAsync(a => a.Id == dummyAthor.Id) != null);

            await repo.DeleteAuthor(dummyAthor.Id);
            Assert.True(await libContext.Authors.FirstOrDefaultAsync(a => a.Id == dummyAthor.Id) == null);
        }
    }
}