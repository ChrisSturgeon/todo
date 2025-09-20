using api.DTOs;
using api.Validators;

namespace api.UnitTests.Validators;

public class ReorderTodosRequestValidatorTests
{
    [Fact]
    public async Task ReorderTodosRequestValidator_Passes_ValidRequest()
    {
        var validator = new ReorderTodosRequestValidator();
        var request = new ReorderTodosRequest()
        {
            Todos = new List<TodoPosition>()
            {
                new ()
                {
                    Id = Guid.NewGuid(),
                    Position = 2,
                },
                new ()
                {
                    Id = Guid.NewGuid(),
                    Position = 1,
                },
                new ()
                {
                    Id = Guid.NewGuid(),
                    Position = 3,
                }
            }
        };

        var result = await validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ReorderTodosRequestValidator_Fails_WhenGivenEmptyTodos()
    {
        var validator = new ReorderTodosRequestValidator();
        var request = new ReorderTodosRequest()
        {
            Todos = []
        };

        var result = await validator.ValidateAsync(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Todos is empty", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task ReorderTodosRequestValidator_Fails_WhenGivenDuplicateIds()
    {
        var duplicateGuid = new Guid("fe84672e-302e-460e-b292-938ac417ccda");
        var validator = new ReorderTodosRequestValidator();
        var request = new ReorderTodosRequest()
        {
            Todos = new List<TodoPosition>()
            {
                new ()
                {
                    Id =  duplicateGuid,
                    Position = 2,
                },
                new ()
                {
                    Id = duplicateGuid,
                    Position = 1,
                },
            }
        };

        var result = await validator.ValidateAsync(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Todos must not contain duplicate ids", result.Errors[0].ErrorMessage);
    }
    
    [Fact]
    public async Task ReorderTodosRequestValidator_Fails_WhenGivenDuplicatePositions()
    {
        var validator = new ReorderTodosRequestValidator();
        var request = new ReorderTodosRequest()
        {
            Todos = new List<TodoPosition>()
            {
                new ()
                {
                    Id =  Guid.NewGuid(),
                    Position = 1,
                },
                new ()
                {
                    Id = Guid.NewGuid(),
                    Position = 1,
                },
            }
        };

        var result = await validator.ValidateAsync(request);
        
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("Todos must not contain duplicate positions", result.Errors[0].ErrorMessage);
    }
}