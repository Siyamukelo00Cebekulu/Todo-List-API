using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace TodoListApi.Tests;

public class IntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public IntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterLoginRefreshAndManageTodoList_Succeeds()
    {
        var client = _factory.CreateClient();

        var registerResponse = await client.PostAsJsonAsync("api/auth/register", new
        {
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "Password123!"
        });

        registerResponse.EnsureSuccessStatusCode();
        var authData = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authData);
        Assert.False(string.IsNullOrWhiteSpace(authData!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(authData.RefreshToken));

        var refreshResponse = await client.PostAsJsonAsync("api/auth/refresh", new
        {
            RefreshToken = authData.RefreshToken
        });

        refreshResponse.EnsureSuccessStatusCode();
        var refreshData = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(refreshData);
        Assert.False(string.IsNullOrWhiteSpace(refreshData!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(refreshData.RefreshToken));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", refreshData.AccessToken);

        var createTodoResponse = await client.PostAsJsonAsync("api/todo", new
        {
            Title = "First task",
            Description = "Write integration tests",
            IsComplete = false,
            DueDate = DateTime.UtcNow.AddDays(3),
            Priority = 1
        });

        createTodoResponse.EnsureSuccessStatusCode();
        var todoItem = await createTodoResponse.Content.ReadFromJsonAsync<TodoItemResponse>();
        Assert.NotNull(todoItem);
        Assert.Equal("First task", todoItem!.Title);

        var listResponse = await client.GetAsync("api/todo?page=1&pageSize=10&sortBy=duedate&sortOrder=asc");
        listResponse.EnsureSuccessStatusCode();
        var list = await listResponse.Content.ReadFromJsonAsync<List<TodoItemResponse>>();
        Assert.NotNull(list);
        Assert.Single(list);
        Assert.Equal("First task", list![0].Title);
    }

    [Fact]
    public async Task UnauthorizedTodoAccess_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("api/todo");
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private sealed record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);
    private sealed record TodoItemResponse(int Id, string Title, string? Description, bool IsComplete, DateTime CreatedAt, DateTime? DueDate, int Priority);
}
