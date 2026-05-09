using BookHaven.DataAccess.Data;
using BookHaven.DataAccess.Repository;
using BookHaven.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookHaven.Tests;

public class CategoryRepositoryTests
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
    public async Task CategoryRepository_Add_ShouldSaveCategoryWithCorrectDipslayOrder()
    {
        //ARRANGE
        var db = await GetDbContext();
        var categoryRepo = new CategoryRepository(db);
        var category = new Category()
        {
            Id = 10,
            Name = "Test Category",
            DisplayOrder = 7
        };
        
        //ACT
        categoryRepo.Add(category);
        await db.SaveChangesAsync();
        
        //ASSERT
        var result = await db.Categories.FirstOrDefaultAsync(x => x.Name == "Test Category");
        result.Should().NotBeNull();
        result.DisplayOrder.Should().Be(7);
    }
    
    [Fact]
    public async Task CategoryRepository_Update_ShouldChangeNameAndDisplayOrder()
    {
        //ARANGE
        var db = await GetDbContext();
        var categoryRepo = new CategoryRepository(db);
        var category = db.Categories.FirstOrDefault(x => x.Id == 1);
        
        category.Name = "Test Category";
        category.DisplayOrder = 7;
        
        //ACT
        categoryRepo.Update(category);
        await db.SaveChangesAsync();
        
        //ASSERT
        var result = await db.Categories.FindAsync(1);
        result.Should().NotBeNull();
        result.DisplayOrder.Should().Be(7);
        result.Name.Should().Be("Test Category");
    }
}