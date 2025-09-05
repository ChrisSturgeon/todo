using api.Models;
using api.DTOs;
using api.Repositories.Interfaces;
using api.Services;
using Moq;

namespace api.UnitTests.Services;

public class TodoServiceTests
{
    [Fact]
    public async Task GetAllTodosAsync_ShouldReturnEmptyArrayWhenNoTodosFound()
    {
        IEnumerable<Todo> emptyTodos = new List<Todo>();
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetAllTodosAsync()).ReturnsAsync(emptyTodos);

        var service = new TodoService(mockRepo.Object);
        var result = await service.GetAllTodosAsync();

        Assert.Equal(emptyTodos, result);
    }

    [Fact]
    public async Task GetAllTodosAsync_ShouldReturnListOfTodosWhenPresent()
    {
        IEnumerable<Todo> todos = new List<Todo>() { new Todo()
            {
                Name = "Walk dog",  
                Description = "Take the dog around the park",
                Completed = false,
                CreatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
                Position = 0
            } 
        };

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetAllTodosAsync()).ReturnsAsync(todos);

        var service = new TodoService(mockRepo.Object);
        var result = await service.GetAllTodosAsync();
        
        Assert.Equal(todos, result);
    }

    [Fact]
    public async Task GetAllTodosAsync_ShouldReturnTodosOrderedByPosition()
    {
        var todos = new List<Todo>
        {
            new Todo { Name = "Second", Position = 1 },
            new Todo { Name = "First", Position = 0 },
            new Todo { Name = "Third", Position = 2 },
        };

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetAllTodosAsync()).ReturnsAsync(todos);

        var service = new TodoService(mockRepo.Object);
        var result = await service.GetAllTodosAsync();

        Assert.Collection(result,
            item => Assert.Equal("First", item.Name),
            item => Assert.Equal("Second", item.Name),
            item => Assert.Equal("Third", item.Name));
    }

    [Fact]
    public async Task GetTodoByIdAsync_ReturnsNullWhenNotFound()
    {
        var testTodoId = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");
            
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(testTodoId)).ReturnsAsync((Todo?)null);

        var service = new TodoService(mockRepo.Object);
        var result = await service.GetTodoByIdAsync(testTodoId);
        
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTodoByIdAsync_ReturnsTodoWhenFound()
    {
        var testTodo = new Todo
        {
            Name = "Walk dog",
            Description = "Take the dog around the park",
            Completed = false,
            CreatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            Position = 0
        };

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(testTodo.Id)).ReturnsAsync(testTodo);

        var service = new TodoService(mockRepo.Object);
        var result = await service.GetTodoByIdAsync(testTodo.Id);
        
        Assert.Equal(result, testTodo);
    }

    [Fact]
    public async Task CreateTodoAsync_ReturnsNewlyCreatedTodo()
    {
        const string testTodoName = "Walk the dog";
        const string testTodoDescription = "Take the dog for a long walk around the park,";
        IEnumerable<Todo> existingTodos = new List<Todo>();
        var newTodo = new Todo
        {
            Id = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada"),
            Name = testTodoName,
            Description = testTodoDescription,
            Completed = false,
            Position = existingTodos.Count(),
            CreatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
        };

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetAllTodosAsync()).ReturnsAsync(existingTodos);
        mockRepo.Setup(r => r.AddTodoAsync(newTodo)).Returns(Task.CompletedTask);

        var service = new TodoService(mockRepo.Object);
        var result = await service.CreateTodoAsync(testTodoName, testTodoDescription);

        Assert.NotNull(result);
        Assert.Equal(testTodoName, result.Name);
        Assert.Equal(testTodoDescription, result.Description);
        Assert.False(result.Completed);
        Assert.Equal(0, result.Position);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(result.CreatedAt, result.UpdatedAt);
    }

    [Fact]
    public async Task CreateTodoAsync_AssignsCorrectPosition_WhenTodosExist()
    {
        var existingTodos = new List<Todo>
        {
            new Todo { Position = 0 },
            new Todo { Position = 1 }
        };

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetAllTodosAsync()).ReturnsAsync(existingTodos);

        Todo? savedTodo = null;
        mockRepo.Setup(r => r.AddTodoAsync(It.IsAny<Todo>()))
            .Callback<Todo>(t => savedTodo = t)
            .Returns(Task.CompletedTask);

        var service = new TodoService(mockRepo.Object);
        var result = await service.CreateTodoAsync("New Todo", "Something");

        Assert.Equal(2, result.Position);
        Assert.Equal(savedTodo?.Position, result.Position);
    }

    [Fact]
    public async Task UpdateTodoAsync_ReturnsFalse_WhenTodoNotFound()
    {
        var testGuid = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");
        
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(testGuid))
            .ReturnsAsync((Todo?)null);

        var service = new TodoService(mockRepo.Object);
        var result = await service.UpdateTodoAsync(testGuid, null, null, null);
        
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateTodoAsync_ReturnsTrue_WhenTodoFound()
    {
        var testGuid = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(testGuid))
            .ReturnsAsync(new Todo());

        var service = new TodoService(mockRepo.Object);
        var result = await service.UpdateTodoAsync(
            testGuid,
            null,
            null,
            true);
        
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateTodoAsync_UpdatesTheNameWhenProvided()
    {
        var existingTodoId = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");
            
        var existingTodo = new Todo
        {
            Id = existingTodoId,
            Name = "Walk dog",
            Description = "Take the dog around the park",
            Completed = false,
            CreatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            Position = 0
        };

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada")))
            .ReturnsAsync(existingTodo);
        
        Todo? updatedTodo = null;
        mockRepo.Setup(r => r.UpdateTodoAsync(It.IsAny<Todo>()))
            .Callback<Todo>(t => updatedTodo = t)
            .Returns(Task.CompletedTask);

        var service = new TodoService(mockRepo.Object);
        var result = await service.UpdateTodoAsync(
            existingTodoId,
            "Feed the dog",
            null,
            true);
        
        Assert.True(result);
        Assert.Equal("Feed the dog", updatedTodo?.Name);
    }

    [Fact]
    public async Task UpdateTodo_PersistsExistingName_WhenNotProvided()
    {
        var existingTodoId = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");
            
        var existingTodo = new Todo
        {
            Id = existingTodoId,
            Name = "Walk dog",
            Description = "Take the dog around the park",
            Completed = false,
            CreatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            Position = 0
        };

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada")))
            .ReturnsAsync(existingTodo);
        
        Todo? updatedTodo = null;
        mockRepo.Setup(r => r.UpdateTodoAsync(It.IsAny<Todo>()))
            .Callback<Todo>(t => updatedTodo = t)
            .Returns(Task.CompletedTask);

        var service = new TodoService(mockRepo.Object);
        var result = await service.UpdateTodoAsync(existingTodoId, null, null, true);
        
        Assert.True(result);
        Assert.Equal("Walk dog", updatedTodo?.Name);
    }
    
    [Fact]
    public async Task UpdateTodoAsync_UpdatesTheDescription_WhenProvided()
    {
        var existingTodoId = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");
            
        var existingTodo = new Todo
        {
            Id = existingTodoId,
            Name = "Walk dog",
            Description = "Take the dog around the park",
            Completed = false,
            CreatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            Position = 0
        };

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada")))
            .ReturnsAsync(existingTodo);
        
        Todo? updatedTodo = null;
        mockRepo.Setup(r => r.UpdateTodoAsync(It.IsAny<Todo>()))
            .Callback<Todo>(t => updatedTodo = t)
            .Returns(Task.CompletedTask);

        var service = new TodoService(mockRepo.Object);
        var result = await service.UpdateTodoAsync(
            existingTodoId,
            null,
            "Walk Sniffles for three hours",
            true);
        
        Assert.True(result);
        Assert.Equal("Walk Sniffles for three hours", updatedTodo?.Description);
    }

    [Fact]
    public async Task UpdateTodoAsync_PersistsExistingDescription_WhenNotProvided()
    {
        var existingTodoId = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");
            
        var existingTodo = new Todo
        {
            Id = existingTodoId,
            Name = "Walk dog",
            Description = "Take the dog around the park",
            Completed = false,
            CreatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            Position = 0
        };

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada")))
            .ReturnsAsync(existingTodo);
        
        Todo? updatedTodo = null;
        mockRepo.Setup(r => r.UpdateTodoAsync(It.IsAny<Todo>()))
            .Callback<Todo>(t => updatedTodo = t)
            .Returns(Task.CompletedTask);

        var service = new TodoService(mockRepo.Object);
        var result = await service.UpdateTodoAsync(existingTodoId, null, null, true);
        
        Assert.True(result);
        Assert.Equal("Take the dog around the park", updatedTodo?.Description);
    }

    [Fact]
    public async Task UpdateTodoAsync_UpdatesTheCompletedStatus_WhenProvided()
    {
        var existingTodoId = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");
            
        var existingTodo = new Todo
        {
            Id = existingTodoId,
            Name = "Walk dog",
            Description = "Take the dog around the park",
            Completed = false,
            CreatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            Position = 0
        };

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(existingTodoId))
            .ReturnsAsync(existingTodo);

        Todo? updatedTodo = null;
        mockRepo.Setup(r => r.UpdateTodoAsync(existingTodo))
            .Callback<Todo>(t => updatedTodo = t)
            .Returns(Task.CompletedTask);

        var service = new TodoService(mockRepo.Object);
        var result = await service.UpdateTodoAsync(existingTodoId, null, null, true);
        
        Assert.True(result);
        Assert.True(updatedTodo?.Completed);
    }

    [Fact]
    public async Task UpdateTodoAsync_PersistsExistingCompleted_WhenNotProvided()
    {
        var existingTodoId = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");
            
        var existingTodo = new Todo
        {
            Id = existingTodoId,
            Name = "Walk dog",
            Description = "Take the dog around the park",
            Completed = false,
            CreatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2025, 08, 31, 11, 40, 01, TimeSpan.Zero),
            Position = 0
        };

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(existingTodoId))
            .ReturnsAsync(existingTodo);

        Todo? updatedTodo = null;
        mockRepo.Setup(r => r.UpdateTodoAsync(existingTodo))
            .Callback<Todo>(t => updatedTodo = t)
            .Returns(Task.CompletedTask);

        var service = new TodoService(mockRepo.Object);
        var result = await service.UpdateTodoAsync(existingTodoId, null, null, null);
        
        Assert.True(result);
        Assert.False(updatedTodo?.Completed);
    }

    [Fact]
    public async Task UpdateTodo_UpdatesTheUpdatedAtToCurrentTime()
    {
        var existingTodoId = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");
            
        var existingTodo = new Todo
        {
            Id = existingTodoId,
            Name = "Walk dog",
            Description = "Take the dog around the park",
            Completed = false,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            Position = 0
        };

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(existingTodoId))
            .ReturnsAsync(existingTodo);

        Todo? updatedTodo = null;
        mockRepo.Setup(r => r.UpdateTodoAsync(existingTodo))
            .Callback<Todo>(t => updatedTodo = t)
            .Returns(Task.CompletedTask);

        var service = new TodoService(mockRepo.Object);

        var before = DateTimeOffset.UtcNow;
        var result = await service.UpdateTodoAsync(existingTodoId, null, null, null);
        var after = DateTimeOffset.UtcNow;
        
        Assert.True(result);
        Assert.True(
            updatedTodo?.UpdatedAt >= before,
            $"UpdatedAt was too early: {updatedTodo.UpdatedAt} < {before}");
        Assert.True(updatedTodo.UpdatedAt <= after, 
            $"UpdatedAt was too late: {updatedTodo?.UpdatedAt} > {after}");
    }

    [Fact]
    public async Task DeleteTodoAsync_ReturnsFalse_WhenTodoNotFound()
    {
        var testTodoId = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(testTodoId))
            .ReturnsAsync((Todo?)null);

        var service = new TodoService(mockRepo.Object);
        var result = await service.DeleteTodoAsync(testTodoId);
        
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteTodoAsync_ReturnTrueWhenTodoDeleted()
    {
        var testTodoId = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(testTodoId))
            .ReturnsAsync(new Todo());
        mockRepo.Setup(r => r.GetAllTodosAsync())
            .ReturnsAsync(new List<Todo>());

        var service = new TodoService(mockRepo.Object);
        var result = await service.DeleteTodoAsync(testTodoId);
        
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTodoAsync_ReordersPositionForRemainingTodos_WhenNecessary()
    {
        var testTodoId = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");
        
        var todos = new List<Todo>
        {
            new Todo { Id = testTodoId, Name = "First", Position = 0 },
            new Todo { Name = "Second", Position = 1 },
            new Todo { Name = "Third", Position = 2 },
        };

        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetTodoByIdAsync(testTodoId))
            .ReturnsAsync(todos.FirstOrDefault(t => t.Id == testTodoId));
        
        mockRepo.Setup(r => r.GetAllTodosAsync())
            .ReturnsAsync(todos);

        mockRepo.Setup(r => r.DeleteTodoAsync(It.IsAny<Todo>()))
            .Callback<Todo>(t => todos.Remove(t))
            .Returns(Task.CompletedTask);

        IEnumerable<Todo>? updatedTodos = null;
        mockRepo.Setup(r => r.UpdateTodoPositionsAsync(It.IsAny<IEnumerable<Todo>>()))
            .Callback<IEnumerable<Todo>>(l => updatedTodos = l)
            .Returns(Task.CompletedTask);

        var service = new TodoService(mockRepo.Object);
        var result = await service.DeleteTodoAsync(testTodoId);
        
        Assert.True(result);
        Assert.NotNull(updatedTodos);
        Assert.Collection(updatedTodos,
            item =>
            {
                Assert.Equal("Second", item.Name);
                Assert.Equal(0, item.Position);
            },
            item =>
            {
                Assert.Equal("Third", item.Name);
                Assert.Equal(1, item.Position);
            });
    }

    [Fact]
    public async Task ReorderTodosAsync_ReturnsFalseWhenNoTodosExist()
    {
        var todo1Guid = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");
        var todo2Guid = new Guid("7faf2283-162f-4d65-adb4-288f4cd23d41");
        var todo3Guid = new Guid("790eeae5-ce08-43ef-ba03-ebc55f00cd10");
        
        var reorderedTodos = new List<TodoReorderDto>
        {
            new TodoReorderDto { Id = todo1Guid, Position = 2 },
            new TodoReorderDto { Id = todo2Guid, Position = 0 },
            new TodoReorderDto { Id = todo3Guid, Position = 1 },
        };
        
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetAllTodosAsync())
            .ReturnsAsync(new List<Todo>());

        var service = new TodoService(mockRepo.Object);
        var result = await service.ReorderTodosAsync(reorderedTodos);
        
        Assert.False(result);
    }

    [Fact]
    public async Task ReorderTodosAsync_ReordersTheTodos()
    {
        var todo1Guid = new Guid("4d3c2af0-ca58-4358-8c98-ceafa468cada");
        var todo2Guid = new Guid("7faf2283-162f-4d65-adb4-288f4cd23d41");
        var todo3Guid = new Guid("790eeae5-ce08-43ef-ba03-ebc55f00cd10");
        
        var todos = new List<Todo>
        {
            new Todo { Id = todo1Guid, Name = "Foo", Position = 0 },
            new Todo { Id = todo2Guid, Name = "Bar", Position = 1 },
            new Todo { Id = todo3Guid, Name = "Baz", Position = 2 },
        };
        
        var updatedTodos = new List<TodoReorderDto>
        {
            new TodoReorderDto { Id = todo1Guid, Position = 2 },
            new TodoReorderDto { Id = todo2Guid, Position = 0 },
            new TodoReorderDto { Id = todo3Guid, Position = 1 },
        };


        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(r => r.GetAllTodosAsync())
            .ReturnsAsync(todos);

        IEnumerable<Todo>? reorderedTodos = null;

        mockRepo.Setup(r => r.UpdateTodoPositionsAsync(It.IsAny<IEnumerable<Todo>>()))
            .Callback<IEnumerable<Todo>>(l => reorderedTodos = l)
            .Returns(Task.CompletedTask);

        var service = new TodoService(mockRepo.Object);
        var result = await service.ReorderTodosAsync(updatedTodos);
        
        Assert.True(result);
        Assert.NotNull(reorderedTodos);
        Assert.Collection(reorderedTodos, item =>
        {
            Assert.Equal(todo2Guid, item.Id);
            Assert.Equal(0, item.Position);
        },
            item =>
        {
            Assert.Equal(todo3Guid, item.Id);
            Assert.Equal(1, item.Position);
        }, 
            item =>
        {
            Assert.Equal(todo1Guid, item.Id);
            Assert.Equal(2, item.Position);
        });
    }
}