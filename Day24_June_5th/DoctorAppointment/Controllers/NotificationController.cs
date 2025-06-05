using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using DoctorAppointment.Models;
using DoctorAppointment.Hubs;

namespace DoctorAppointment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] MessageDto message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.User, message.Content);
            return Ok(new { status = "Message sent" });
        }
    }
}
