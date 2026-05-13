using BookHaven.DataAccess.Data;
using BookHaven.DataAccess.Repository;
using BookHaven.DataAccess.Repository.IRepository;
using BookHaven.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookHaven.Tests;

public class UnitOfWorkTests
{
    private async Task<ApplicationDbContext> GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var dbContext = new ApplicationDbContext(options);
        dbContext.Database.EnsureCreated();
        return dbContext;
    }

    [Fact]
    public async Task UnitOfWork_Save_ShouldPersistMultipleChangesAcrossDifferentRepositories()
    {
        //ARRANGE
        var db = await GetDbContext();
        IUnitOfWork unitOfWork = new UnitOfWork(db);

        var newCategory = new Category()
        {
            Name = "Science Fiction",
            DisplayOrder = 75
        };
        unitOfWork.Category.Add(newCategory);

        var newProduct = new Product()
        {
            Title = "Dune",
            ISBN = "DUNE123",
            Description = "",
            Author = "Frank Herbert",
            ListPrice = 30,
            Price = 25,
            Price50 = 22,
            Price100 = 20,
            ImageUrl =  "",
            Category = newCategory
        };
        unitOfWork.Product.Add(newProduct);
        
        //ACT
        unitOfWork.Save();
        
        //ASSERT
        var productInDb = await db.Products.Include(u => u.Category)
            .FirstOrDefaultAsync(u => u.ISBN ==  "DUNE123");

        productInDb.Should().NotBeNull();
        productInDb.Category.Name.Should().Be("Science Fiction");
    }

    [Fact]
    public async Task UnitOfWork_DeleteProduct_ShouldNotDeleteCategory()
    {
        // ARRANGE
        var db = await GetDbContext();
        IUnitOfWork unitOfWork = new UnitOfWork(db);

        var product = unitOfWork.Product.Get(u => u.Id == 1);
        var categoryId = product.CategoryId;
        
        //ACT
        unitOfWork.Product.Remove(product);
        unitOfWork.Save();
        
        //ASSERT
        var deletedProduct = unitOfWork.Product.Get(u => u.Id == 1);
        var categoryStillExists = unitOfWork.Category.Get(u => u.Id == categoryId);

        deletedProduct.Should().BeNull(); 
        categoryStillExists.Should().NotBeNull();
    }
}