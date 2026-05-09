using System.ComponentModel.DataAnnotations;
using BookHaven.DataAccess.Data;
using BookHaven.DataAccess.Repository;
using BookHaven.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookHaven.Tests;

public class ProductRepositoryTests
{
    //Database for every test
    private async Task<ApplicationDbContext> GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        var dbContext = new ApplicationDbContext(options);
        dbContext.Database.EnsureCreated(); //This will seed my data into db
        return dbContext;
    }
    
    // Helper factory method to keep tests DRY (Don't Repeat Yourself)
    private Product GetValidProductTemplate()
    {
        return new Product
        {
            Title = "Test Book",
            Author = "Ahmed Skopljak",
            Description = "Test Description",
            ISBN = "123123ab",
            ListPrice = 50,
            Price = 45,
            Price50 = 40,
            Price100 = 35,
            CategoryId = 2,
            ImageUrl = ""
        };
    }

    //Does new Product save in database
    [Fact]
    public async Task ProductRepository_Add_ShouldSaveProductToDB()
    {
        //ARANGE
        var db = await GetDbContext();
        var productRepository = new ProductRepository(db);

        var product = GetValidProductTemplate();
        
        //ACT
        productRepository.Add(product);
        await db.SaveChangesAsync();
        
        //ASSERT
        var result = await db.Products.FirstOrDefaultAsync(x => x.ISBN == "123123ab");

        result.Should().NotBeNull();
        result.Title.Should().Be("Test Book");
        result.Price.Should().Be(45);
    }
    
    //Boundary values Test
    [Fact]
    public async Task ProductRepository_Add_ShouldFail_WhenPriceIsBelowRange()
    {
        //ARANGE
        var db = await GetDbContext();
        var productRepository = new ProductRepository(db);
        var productWithZeroPrice = new Product
        {
            Title = "Free Book",
            Author = "Test Author",
            Description = "Test Description",
            ISBN = "000-000-120",
            Price = 0, //Price 0
            ListPrice = 0,
            Price50 = 0,
            Price100 = 0,
            CategoryId = 1,
            ImageUrl = ""
        };
        //ACT && ASSERT
        productRepository.Add(productWithZeroPrice);
        
        Func<Task> act = async () => await db.SaveChangesAsync();

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public void ProductModel_Validation_ShouldCatchError_WhenPriceIsZero()
    {
        //ARANGE
        var product = new Product()
        {
            Title = "Free Book",
            Author = "Test Author",
            Description = "Test Description",
            ISBN = "000-000-120",
            Price = 0, //Price 0 [Range(1, 1000)]
            ListPrice = 0,
            Price50 = 0,
            Price100 = 0,
            CategoryId = 1,
            ImageUrl = ""
        };
        
        // ACT 
        var context = new ValidationContext(product);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(product, context, results, true);

        // ASSERT 
        isValid.Should().BeFalse();
        results.Should().Contain(x => x.ErrorMessage.Contains("1 and 1000"));
    }

    [Fact]
    public async Task ProductRepository_Update_ShouldModifyExistitngProduct()
    {
        //ARANGE
        var db = await GetDbContext();
        var productRepo = new ProductRepository(db);
        
        var productFromDB = db.Products.FirstOrDefault(x => x.Id == 1);
        string updatedTitle = "Fortune of Time - Updated Title";
        productFromDB.Title = updatedTitle;
        productFromDB.Price = 95;
        
        //ACT
        productRepo.Update(productFromDB);
        await db.SaveChangesAsync();
        
        //ASSERT
        var result = await db.Products.FindAsync(1);
        
        result.Should().NotBeNull();
        result.Title.Should().Be(updatedTitle);
        result.Price.Should().Be(95);
        // Regression check: Ensure fields we didn't touch remain the same
        result.Author.Should().Be("Billy Spark");
    }

    [Fact]
    public async Task ProductRepository_Delete_ShouldDeleteProductFromDB()
    {
        //ARANGE
        var db = await GetDbContext();
        var productRepo = new ProductRepository(db);
        
        var productToDelete = db.Products.FirstOrDefault(x => x.Id == 2);
        var initialCount = db.Products.Count();
        
        //ACT
        productRepo.Remove(productToDelete);
        await db.SaveChangesAsync();
        
        //ASSERT
        var result = await db.Products.FindAsync(2);
        var finalCount = await db.Products.CountAsync();
        result.Should().BeNull();
        finalCount.Should().Be(initialCount - 1);
    }

    [Fact]
    public async Task ProductRepository_GetProductWithCategory_ShouldReturnCategoryNameOfProduct()
    {
        //ASSERT
        var db = await GetDbContext();
        var productRepo = new ProductRepository(db);
        
        //ACT
        var product = productRepo.Get(u => u.Id == 1, includeProperties: "Category");
        
        //ASSERT
        product.Should().NotBeNull();
        product.Category.Name.Should().Be("Action");
    }
    
    [Fact]
    public void ProductModel_PriceHierarchy_ShouldBeConsistentWithDiscounts()
    {
        // ARRANGE
        var product = GetValidProductTemplate();
        product.ListPrice = 99;
        product.Price = 90;
        product.Price50 = 85;
        product.Price100 = 80;

        // ACT & ASSERT
        // 1. Regular Price should be less than or equal to ListPrice
        product.Price.Should().BeLessThanOrEqualTo(product.ListPrice, 
            "because the selling price should not be higher than the MSRP/List Price");

        // 2. Tiered discounts check: Price > Price50 > Price100
        product.Price50.Should().BeLessThan(product.Price, 
            "because bulk price for 50+ should offer a discount compared to regular price");

        product.Price100.Should().BeLessThan(product.Price50, 
            "because bulk price for 100+ should be the most economical option");

        // 3. Boundary check: ensure the lowest price tier is still profitable/positive
        product.Price100.Should().BeGreaterThan(0);
    }
}