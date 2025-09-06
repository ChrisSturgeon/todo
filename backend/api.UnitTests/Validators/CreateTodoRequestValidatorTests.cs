using api.DTOs;
using api.Validators;

namespace api.UnitTests.Validators;

public class CreateTodoRequestValidatorTests
{
    [Fact]
    public async Task CreateTodoRequestValidator_Passes_WhenNameIsProvided()
    {
        var validator = new CreateTodoRequestValidator();
        var dto = new CreateTodoRequest
        {
            Name = "Test name"
        };
        var result = await validator.ValidateAsync(dto);
        
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public async Task CreateTodoRequestValidator_WhenNameIsEmpty()
    {
        var validator = new CreateTodoRequestValidator();
        var dto = new CreateTodoRequest
        {
            Description = "Test description",
        };
        var result = await validator.ValidateAsync(dto);
        
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains("Todo name is required", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task CreateTodoRequestValidator_Passes_WhenProvidedDescriptionWithinLength()
    {
        var validator = new CreateTodoRequestValidator();
        var dto = new CreateTodoRequest()
        {
            Name = "Test name",
            Description = new string('a', 100),
        };
        var result = await validator.ValidateAsync(dto);
        
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreateTodoRequestValidator_Fails_WhenProvidedShortDescription()
    {
        var validator = new CreateTodoRequestValidator();
        var dto = new CreateTodoRequest()
        {
            Name = "Test Name",
            Description = "ab"
        };
        var result = await validator.ValidateAsync(dto);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Todo description must be between 3 and 100 characters", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task CreateTodoRequestValidator_Fails_WhenProvidedLongDescription()
    {
        var validator = new CreateTodoRequestValidator();
        var dto = new CreateTodoRequest()
        {
            Name = "Test name",
            Description = new string('a', 101),
        };
        var result = await validator.ValidateAsync(dto);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Todo description must be between 3 and 100 characters", result.Errors[0].ErrorMessage);
    }
}