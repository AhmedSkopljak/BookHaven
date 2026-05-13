using BookHaven.DataAccess.Repository.IRepository;
using BookHaven.Models;
using BookHavenWeb.Areas.Admin.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookHaven.Tests.ControllerTests;

public class CategoryControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _controller = new CategoryController(_mockUnitOfWork.Object);
    }

    [Fact]
    public void Index_ReturnsAViewResult_WithAListOfCategories()
    {
        // ARRANGE
        var categories = new List<Category> 
        { 
            new Category { Id = 1, Name = "Sci-Fi" },
            new Category { Id = 2, Name = "History" }
        };

        _mockUnitOfWork.Setup(u => u.Category.GetAll(null)).Returns(categories);
        
        //ACT
        var result = _controller.Index();
        
        // ASSERT
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.ViewData.Model.Should().BeAssignableTo<List<Category>>().Subject;
        
        model.Count.Should().Be(2);
        model.First().Name.Should().Be("Sci-Fi");
    }

    [Fact]
    public void Create_Post_DisplayOrderAndNameAreSame_ReturnsError()
    {
        // ARRANGE
        var category = new Category { Name = "10", DisplayOrder = 10 };
        
        //ACT
        var result = _controller.Create(category);
        
        //ASSERT
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;

        _controller.ModelState.IsValid.Should().BeFalse();
        _controller.ModelState["name"].Errors[0].ErrorMessage.Should()
            .Be("The DisplayOrder Cannot Exactly Match The Name.");
        
        _mockUnitOfWork.Verify(u => u.Category.Add(It.IsAny<Category>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.Save(), Times.Never);
    }
}