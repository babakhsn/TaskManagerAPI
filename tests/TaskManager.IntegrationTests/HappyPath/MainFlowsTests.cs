using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using TaskManager.Application.DTOs;
using TaskManager.Application.Projects.DTOs;
using TaskManager.Application.Tasks.DTOs;
using TaskManager.Domain.Enums;
using TaskManager.IntegrationTests.Utils;

namespace TaskManager.IntegrationTests.HappyPath;

public class MainFlowsTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    public MainFlowsTests(TestWebApplicationFactory f) => _client = f.CreateClient();

    [Fact]
    public async Task Project_Task_Comment_Flow_Succeeds()
    {
        // Create project
        var pResp = await _client.PostAsJsonAsync("/api/projects", new CreateProjectRequest("CI Project"));
        pResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var project = await pResp.Content.ReadFromJsonAsync<ProjectDto>();
        project!.Name.Should().Be("CI Project");

        // Create task
        var tResp = await _client.PostAsJsonAsync($"/api/projects/{project.Id}/tasks",
            new CreateTaskRequest("CI Task", "desc", DateTime.UtcNow.Date.AddDays(1), Priority.High));
        tResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var task = await tResp.Content.ReadFromJsonAsync<TaskDto>();

        // Update task status
        var uResp = await _client.PutAsJsonAsync($"/api/projects/{project.Id}/tasks/{task!.Id}",
            new UpdateTaskRequest(null, "updated", DateTime.UtcNow.Date.AddDays(2), Priority.Medium, Domain.Enums.TaskStatus.InProgress, null));
        uResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await uResp.Content.ReadFromJsonAsync<TaskDto>();
        updated!.Status.Should().Be(Domain.Enums.TaskStatus.InProgress);

        // Add comment
        var cResp = await _client.PostAsJsonAsync($"/api/projects/{project.Id}/tasks/{task.Id}/comments",
            new TaskManager.Application.Comments.DTOs.CreateCommentRequest("Nice work!"));
        cResp.StatusCode.Should().Be(HttpStatusCode.Created);

        // List comments
        var lResp = await _client.GetAsync($"/api/projects/{project.Id}/tasks/{task.Id}/comments");
        lResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var comments = await lResp.Content.ReadFromJsonAsync<List<CommentDto>>();
        comments!.Should().ContainSingle().Which.Body.Should().Contain("Nice");

        // Delete task
        var dResp = await _client.DeleteAsync($"/api/projects/{project.Id}/tasks/{task.Id}");
        dResp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
