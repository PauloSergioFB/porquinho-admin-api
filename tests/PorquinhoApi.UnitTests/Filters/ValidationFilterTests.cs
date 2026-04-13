using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using PorquinhoApi.Filters;
using Xunit;

namespace PorquinhoApi.UnitTests.Filters;

public class ValidationFilterTests
{
    [Fact]
    public async Task InvokeAsync_WhenDtoIsValid_ShouldCallNext()
    {
        // Arrange
        var filter = new ValidationFilter<CreateUserDto>();
        var dto = new CreateUserDto
        {
            FullName = "Paulo França",
            EmailAddress = "paulo@email.com"
        };

        var httpContext = new DefaultHttpContext();
        var invocationContext = CreateContext(httpContext, dto);

        var expectedResult = Results.Ok();
        var nextCalled = false;

        EndpointFilterDelegate next = (context) =>
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>(expectedResult);
        };

        // Act
        var result = await filter.InvokeAsync(invocationContext, next);

        // Assert
        nextCalled.Should().BeTrue();
        Assert.Same(expectedResult, result);
    }

    [Fact]
    public async Task InvokeAsync_WhenDtoIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var filter = new ValidationFilter<CreateUserDto>();
        var dto = new CreateUserDto
        {
            FullName = "",
            EmailAddress = ""
        };

        var httpContext = new DefaultHttpContext();
        var invocationContext = CreateContext(httpContext, dto);

        var nextCalled = false;

        EndpointFilterDelegate next = (context) =>
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>(Results.Ok());
        };

        // Act
        var result = await filter.InvokeAsync(invocationContext, next);

        // Assert
        nextCalled.Should().BeFalse();

        var badRequest = Assert.IsType<BadRequest<Dictionary<string, string>>>(result);
        badRequest.Value.Should().NotBeNull();
        badRequest.Value.Should().ContainKey("full_name");
        badRequest.Value.Should().ContainKey("email_address");
    }

    [Fact]
    public async Task InvokeAsync_WhenDtoIsInvalid_ShouldConvertPropertyNameToSnakeCase()
    {
        // Arrange
        var filter = new ValidationFilter<CreateUserDto>();
        var dto = new CreateUserDto
        {
            FullName = "",
            EmailAddress = ""
        };

        var httpContext = new DefaultHttpContext();
        var invocationContext = CreateContext(httpContext, dto);

        EndpointFilterDelegate next = (context) =>
            ValueTask.FromResult<object?>(Results.Ok());

        // Act
        var result = await filter.InvokeAsync(invocationContext, next);

        // Assert
        var badRequest = Assert.IsType<BadRequest<Dictionary<string, string>>>(result);

        badRequest.Value!.Keys.Should().Contain("full_name");
        badRequest.Value.Keys.Should().Contain("email_address");
        badRequest.Value.Keys.Should().NotContain("FullName");
        badRequest.Value.Keys.Should().NotContain("EmailAddress");
    }

    [Fact]
    public async Task InvokeAsync_WhenTypedDtoIsNotPresent_ShouldCallNext()
    {
        // Arrange
        var filter = new ValidationFilter<CreateUserDto>();
        var httpContext = new DefaultHttpContext();

        var invocationContext = CreateContext(httpContext, "argumento-que-nao-e-o-dto");

        var expectedResult = Results.Ok();
        var nextCalled = false;

        EndpointFilterDelegate next = (context) =>
        {
            nextCalled = true;
            return ValueTask.FromResult<object?>(expectedResult);
        };

        // Act
        var result = await filter.InvokeAsync(invocationContext, next);

        // Assert
        nextCalled.Should().BeTrue();
        Assert.Same(expectedResult, result);
    }

    private static EndpointFilterInvocationContext CreateContext(
        HttpContext httpContext,
        params object?[] arguments)
    {
        var mock = new Mock<EndpointFilterInvocationContext>();

        mock.SetupGet(x => x.HttpContext).Returns(httpContext);
        mock.SetupGet(x => x.Arguments).Returns(arguments);

        return mock.Object;
    }

    private class CreateUserDto
    {
        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-mail é obrigatório.")]
        public string EmailAddress { get; set; } = string.Empty;
    }
}