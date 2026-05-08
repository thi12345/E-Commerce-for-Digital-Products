using FluentAssertions;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Tests.Domain;

public sealed class CategoryTests
{
    [Fact]
    public void Create_ShouldCreateCategory_WithValidData()
    {
        var category = Category.Create("Programming", "Books about programming");

        category.Name.Should().Be("Programming");
        category.Description.Should().Be("Books about programming");
        category.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldCreateCategory_WithoutDescription()
    {
        var category = Category.Create("Design");

        category.Name.Should().Be("Design");
        category.Description.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldThrow_WhenNameIsEmpty()
    {
        var act = () => Category.Create("");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_ShouldTrimName()
    {
        var category = Category.Create("  Programming  ");

        category.Name.Should().Be("Programming");
    }

    [Fact]
    public void Update_ShouldChangeName()
    {
        var category = Category.Create("Old Name");

        category.Update("New Name", "New description");

        category.Name.Should().Be("New Name");
        category.Description.Should().Be("New description");
        category.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_ShouldThrow_WhenNameIsEmpty()
    {
        var category = Category.Create("Programming");

        var act = () => category.Update("", null);

        act.Should().Throw<DomainException>();
    }
}
