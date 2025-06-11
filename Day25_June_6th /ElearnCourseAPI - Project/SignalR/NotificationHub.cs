
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ElearnAPI.SignalR
{
     [Authorize] 
    public class NotificationHub : Hub
    {
        public async Task JoinCourseGroup(Guid courseId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"course-{courseId}");
        }

        public async Task LeaveCourseGroup(Guid courseId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"course-{courseId}");
        }
    }
}
