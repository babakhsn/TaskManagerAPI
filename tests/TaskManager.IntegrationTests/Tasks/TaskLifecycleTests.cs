using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManager.Application.DTOs;
using TaskManager.Application.Projects.DTOs;
using TaskManager.Application.Tasks.DTOs;
using TaskManager.Domain.Enums;
using TaskManager.IntegrationTests.Utils;

namespace TaskManager.IntegrationTests.Tasks;

public class TaskLifecycleTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    public TaskLifecycleTests(TestWebApplicationFactory f) { _client = f.CreateClient(); }

    [Fact]
    public async Task Owner_Can_Create_Get_Update_Delete_Task()
    {
        // Create project as owner
        var pResp = await _client.PostAsJsonAsync("/api/projects", new CreateProjectRequest("P1"));
        pResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var project = await pResp.Content.ReadFromJsonAsync<ProjectDto>();

        // Create task
        var tResp = await _client.PostAsJsonAsync($"/api/projects/{project!.Id}/tasks",
            new CreateTaskRequest("T1", "desc", DateTime.UtcNow.Date.AddDays(1), Priority.High));
        tResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var task = await tResp.Content.ReadFromJsonAsync<TaskDto>();

        // Get
        var gResp = await _client.GetAsync($"/api/projects/{project.Id}/tasks/{task!.Id}");
        gResp.StatusCode.Should().Be(HttpStatusCode.OK);

        // Update (status, assignee = owner)
        var uResp = await _client.PutAsJsonAsync(
            $"/api/projects/{project.Id}/tasks/{task.Id}",
            new UpdateTaskRequest(null, "updated", DateTime.UtcNow.Date.AddDays(2), Priority.Medium, Domain.Enums.TaskStatus.InProgress,
                                  TaskManager.IntegrationTests.Utils.TestAuthHandler.DefaultUserId));
        uResp.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await uResp.Content.ReadFromJsonAsync<TaskDto>();
        updated!.Status.Should().Be(Domain.Enums.TaskStatus.InProgress);
        updated.Priority.Should().Be(Priority.Medium);

        // Delete
        var dResp = await _client.DeleteAsync($"/api/projects/{project.Id}/tasks/{task.Id}");
        dResp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task NonMember_Cannot_Update_Or_Delete()
    {
        // Create project & task as owner (user A)
        var pResp = await _client.PostAsJsonAsync("/api/projects", new CreateProjectRequest("P2"));
        var project = await pResp.Content.ReadFromJsonAsync<ProjectDto>();
        var tResp = await _client.PostAsJsonAsync($"/api/projects/{project!.Id}/tasks",
            new CreateTaskRequest("T2", null, null, Priority.Medium));
        var task = await tResp.Content.ReadFromJsonAsync<TaskDto>();

        // Act as different user (user B)
        using var otherFactory = new TestWebApplicationFactoryDifferentUser();
        var otherClient = otherFactory.CreateClient();

        var updB = await otherClient.PutAsJsonAsync(
            $"/api/projects/{project.Id}/tasks/{task!.Id}",
            new UpdateTaskRequest("Hacker", null, null, null, null, null));
        updB.StatusCode.Should().Be(HttpStatusCode.NotFound); // not member → 404

        var delB = await otherClient.DeleteAsync($"/api/projects/{project.Id}/tasks/{task.Id}");
        delB.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Member_Can_Update()
    {
        // Owner creates project
        var pResp = await _client.PostAsJsonAsync("/api/projects", new CreateProjectRequest("P3"));
        var project = await pResp.Content.ReadFromJsonAsync<ProjectDto>();
        var tResp = await _client.PostAsJsonAsync($"/api/projects/{project!.Id}/tasks",
            new CreateTaskRequest("T3", null, null, Priority.Low));
        var task = await tResp.Content.ReadFromJsonAsync<TaskDto>();

        // Owner adds B as member (direct DB insert via test endpoint would be ideal; here we call a helper)
        await TestDataHelpers.AddMemberAsync(project.Id,
            TaskManager.IntegrationTests.Utils.TestAuthHandler2.DefaultUserId);

        // B updates
        using var bFactory = new TestWebApplicationFactoryDifferentUser();
        var bClient = bFactory.CreateClient();

        var upd = await bClient.PutAsJsonAsync(
            $"/api/projects/{project.Id}/tasks/{task!.Id}",
            new UpdateTaskRequest(null, null, null, null, Domain.Enums.TaskStatus.Done, null));
        upd.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
