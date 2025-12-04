using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Library.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;



namespace Library.Tests
{
    public class AuthorRepositoryTestes
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


        [Fact]
        public async Task GetAuthorBYIdShouldReturnAuthorIfExists()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var libContext = new LibraryContext(options);

            var authorToAdd = new Author
            {
                FirstName = "Test",
                LastName = "Testlast",
                Site = "TestSite",
            };

            await libContext.Authors.AddAsync(authorToAdd);
            await libContext.SaveChangesAsync();

            var repo = new AuthorRepository(libContext);

            var result = await repo.GetAuthorById(authorToAdd.Id);

            Assert.NotNull(result);
            Assert.Equal(authorToAdd.FirstName, result.FirstName);
            Assert.Equal(authorToAdd.LastName,result.LastName);
        }


        [Fact]
        public async Task GetAuthorByIdShouldReturnNullIfNotExists()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var libContext = new LibraryContext(options);
            var repo = new AuthorRepository(libContext);

            var result = await repo.GetAuthorById(999);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAuthorsShouldReturnPaginatedList()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;

            using var libContext = new LibraryContext(options);

            for(int i = 1; i<= 15; i++)
            {
                await libContext.Authors.AddAsync(new Author
                {
                    FirstName = $"FirstName{i}",
                    LastName = $"LastName{i}",
                    Site = $"Site{i}"
                });
            }

            await libContext.SaveChangesAsync();

            var repo = new AuthorRepository(libContext);

            var page1 = await repo.GetAuthors(1, 5);
            var page2 = await repo.GetAuthors(2, 5);
            var page3 = await repo.GetAuthors(3, 5);

            Assert.Equal(5, page1.Items.Count);
            Assert.Equal(5, page2.Items.Count);
            Assert.Equal(5, page3.Items.Count);
            Assert.Equal(1, page1.Items.Count);
            Assert.Equal(2, page2.Items.Count);
            Assert.Equal(3, page3.Items.Count);
            Assert.Equal(3, page1.Items.Count);
            Assert.False(page1.HasPrevious);
            Assert.True(page1.HasNext);
            Assert.True(page2.HasPrevious);
            Assert.True(page2.HasNext);
            Assert.True(page3.HasPrevious);
            Assert.False(page3.HasNext);

        }
    }
}