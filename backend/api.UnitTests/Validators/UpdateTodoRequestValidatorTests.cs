using api.DTOs;
using api.Validators;

namespace api.UnitTests.Validators;

public class UpdateTodoRequestValidatorTests
{
    [Fact]
    public async Task UpdateTodoRequestValidator_Passes_WhenNameIsProvidedWithinLengthRange()
    {
        var validator = new UpdateTodoRequestValidator();
        var request = new UpdateTodoRequest()
        {
            Name = "Test name"
        };
        var result = await validator.ValidateAsync(request);
        
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public async Task UpdateTodoRequestValidator_Fails_WhenNameIsNotProvided()
    {
        var validator = new UpdateTodoRequestValidator();
        var request = new UpdateTodoRequest()
        {
        };
        var result = await validator.ValidateAsync(request);
        
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains("Todo name is required", result.Errors[0].ErrorMessage);
    }
    
    [Fact]
    public async Task UpdateTodoRequestValidator_Fails_WhenNameIsTooShort()
    {
        var validator = new UpdateTodoRequestValidator();
        var request = new UpdateTodoRequest()
        {
            Name = "ab",
        };
        var result = await validator.ValidateAsync(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Todo name must be between 3 and 50 characters", result.Errors[0].ErrorMessage);
    }
    
    [Fact]
    public async Task UpdateTodoRequestValidator_Fails_WhenNameIsTooLong()
    {
        var validator = new UpdateTodoRequestValidator();
        var request = new UpdateTodoRequest()
        {
            Name = new string('a', 51),
        };
        var result = await validator.ValidateAsync(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Todo name must be between 3 and 50 characters", result.Errors[0].ErrorMessage);
    }
    
    [Fact]
    public async Task UpdateTodoRequestValidator_Fails_WhenDescriptionIsTooShort()
    {
        var validator = new UpdateTodoRequestValidator();
        var request = new UpdateTodoRequest()
        {
            Name = "Test name",
            Description = "ab",
        };
        var result = await validator.ValidateAsync(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Todo description must be between 3 and 100 characters", result.Errors[0].ErrorMessage);
    }
    
    [Fact]
    public async Task UpdateTodoRequestValidator_Fails_WhenDescriptionIsTooLong()
    {
        var validator = new UpdateTodoRequestValidator();
        var request = new UpdateTodoRequest()
        {
            Name = "Test name",
            Description = new string('a', 101),
        };
        var result = await validator.ValidateAsync(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Todo description must be between 3 and 100 characters", result.Errors[0].ErrorMessage);
    }
}