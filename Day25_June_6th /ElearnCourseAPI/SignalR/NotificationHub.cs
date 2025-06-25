using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ElearnAPI.Services;
using ElearnAPI.Interfaces.Services;

namespace ElearnAPI.SignalR
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly ICourseService _courseService; 

        public NotificationHub(ICourseService courseService)
        {
            _courseService = courseService;
        }

public override async Task OnConnectedAsync()
{
    var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

    Console.WriteLine($"[SignalR] Connection Established - UserId: {userId}, Role: {role}");

    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
    {
        Console.WriteLine("[SignalR] Missing UserId or Role - Skipping Group Assignment");
        await base.OnConnectedAsync();
        return;
    }

    if (role == "Student")
    {
        var enrolledCourses = await _courseService.GetCourseIdsByStudentAsync(Guid.Parse(userId));
        foreach (var courseId in enrolledCourses)
        {
            Console.WriteLine($"[SignalR] Adding Student {userId} to group: course-{courseId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"course-{courseId}");
        }
    }
    else if (role == "Instructor")
    {
        Console.WriteLine($"[SignalR] Adding Instructor {userId} to group: instructor-{userId}");
        await Groups.AddToGroupAsync(Context.ConnectionId, $"instructor-{userId}");
    }

    await base.OnConnectedAsync();
}



      public async Task JoinCourseGroup(Guid courseId)
{
    Console.WriteLine($"[SignalR] Manually Joining group: course-{courseId}");
    await Groups.AddToGroupAsync(Context.ConnectionId, $"course-{courseId}");
}

public async Task LeaveCourseGroup(Guid courseId)
{
    Console.WriteLine($"[SignalR] Leaving group: course-{courseId}");
    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"course-{courseId}");
}

    }
}
