using api.DTOs;
using api.Validators;

namespace api.UnitTests.Validators;

public class ReorderTodoDtoValidatorTests
{
    [Fact]
    public async Task ReorderTodoDtoValidator_Passes_ForAValidDto()
    {
        var validator = new ReorderTodoDtoValidator();
        var dto = new TodoPosition()
        {
            Id = Guid.NewGuid(),
            Position = 0,
        };

        var result = await validator.ValidateAsync(dto);
        
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public async Task ReorderTodoDtoValidator_Fails_ForMissingId()
    {
        var validator = new ReorderTodoDtoValidator();
        var dto = new TodoPosition()
        {
            Position = 0,
        };

        var result = await validator.ValidateAsync(dto);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Todo Id is required", result.Errors[0].ErrorMessage);
    }
    
    [Fact]
    public async Task ReorderTodoDtoValidator_Fails_ForNegativePosition()
    {
        var validator = new ReorderTodoDtoValidator();
        var dto = new TodoPosition()
        {
            Id = Guid.NewGuid(),
            Position = -1,
        };

        var result = await validator.ValidateAsync(dto);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Position must be greater than or equal to 0", result.Errors[0].ErrorMessage);
    }
}