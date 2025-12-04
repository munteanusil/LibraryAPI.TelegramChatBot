using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Library.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Tests
{
    public class CategoryRepositoryTests
    {
        [Fact]
        public async Task CreateCategoryShouldSaveANewCategoryInDb()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                   .UseInMemoryDatabase(Guid.NewGuid().ToString())
                   .Options;

            using var libContext = new LibraryContext(options);
            ICategoryRepository repo = new CategoryRepository(libContext);

            var categoryToAdd = new Category
            {
                Name = "Test Category"
            };

            await repo.CreateCategory(categoryToAdd);

            Assert.True(categoryToAdd.Id > 0);
            var categoryInDb = await libContext.Categories.FindAsync(categoryToAdd.Id);
            Assert.NotNull(categoryInDb);
            Assert.Equal("Test Category", categoryInDb.Name);
        }


        [Fact]
        public async Task GetCategoryByIdShouldReturnCategoryWithBooks()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;

            using var libContext = new LibraryContext(options);

            var author = new Author
            {
                FirstName = "Liviu",
                LastName = "Matei",
                Site = "CartiScriseDeLiviu.com"
            };
            await libContext.Authors.AddAsync(author);

            var category = new Category { Name = "Fiction" };
            await libContext.Categories.AddAsync(category);
            await libContext.SaveChangesAsync();

            await libContext.Books.AddRangeAsync(
                new Book
                {
                    Title = "Book 1",
                    ISBN = "ISBN1",
                    Stock = 100,
                    CategoryId = category.Id,
                    AuthorId = author.Id,
                },
                new Book
                 {
                       Title = "Book 2",
                       ISBN = "ISBN2",
                       Stock = 2000,
                       CategoryId = category.Id,
                       AuthorId = author.Id
                }

            );
            await libContext.SaveChangesAsync();

            ICategoryRepository repo = new CategoryRepository(libContext);

            var result = await repo.GetCategoryById(category.Id);

            Assert.NotNull(result);
            Assert.Equal(category.Id, result.Id);
            Assert.Equal("Fiction", result.Name);
            Assert.NotNull(result.Books);
            Assert.Equal(2, result.Books.Count);

        }

        [Fact]
        public async Task GetCategorysShouldReturnPaginatedList()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var libContext = new LibraryContext(options);

            for (int i = 1; i <= 10; i++)
            {
                await libContext.Categories.AddAsync(new Category
                {
                    Name = $"Category {i}"
                });
            }
            await libContext.SaveChangesAsync();

            ICategoryRepository repo = new CategoryRepository(libContext);

            var page1 = await repo.GetCategorys(1, 3);
            var page2 = await repo.GetCategorys(2, 3);
            var page3 = await repo.GetCategorys(3, 3);
            var page4 = await repo.GetCategorys(4, 3);

            Assert.Equal(3, page1.Items.Count);
            Assert.Equal(3, page2.Items.Count);
            Assert.Equal(3, page3.Items.Count);
            Assert.Equal(1, page4.Items.Count);
            Assert.Equal(4, page1.TotalPages);
            Assert.Equal(1, page1.PageNumber);
            Assert.Equal(2, page2.PageNumber);
        }

        [Fact]
        public async Task UpdateCategoryShouldUpdateExistingCategory()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var libContext = new LibraryContext(options);

            var category = new Category { Name = "Original Name" };
            await libContext.Categories.AddAsync(category);
            await libContext.SaveChangesAsync();

            ICategoryRepository repo = new CategoryRepository(libContext);

            category.Name = "Updated Name";

            await repo.UpdateCategory(category);

            var updatedCategory = await libContext.Categories.FindAsync(category.Id);
            Assert.NotNull(updatedCategory);
            Assert.Equal("Updated Name", updatedCategory.Name);
        }

        [Fact]
        public async Task DeleteCategoryShouldRemoveCategoryFromDb()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var libContext = new LibraryContext(options);

            var category = new Category { Name = "Category to Delete" };
            await libContext.Categories.AddAsync(category);
            await libContext.SaveChangesAsync();

            ICategoryRepository repo = new CategoryRepository(libContext);

            await repo.DeleteCategory(category.Id);

            var deletedCategory = await libContext.Categories.FindAsync(category.Id);
            Assert.Null(deletedCategory);
        }

        [Fact]
        public async Task DeleteCategoryShouldThrowIfCategoryDoesNotExist()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var libContext = new LibraryContext(options);
            ICategoryRepository repo = new CategoryRepository(libContext);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => repo.DeleteCategory(999));
        }

    }
}
