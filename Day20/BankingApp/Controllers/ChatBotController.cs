using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChatBotController : ControllerBase
{
    private readonly IUserProgressService _progressService;
    private readonly IAIService _aiService;

    public ChatBotController(IUserProgressService progressService, IAIService aiService)
    {
        _progressService = progressService;
        _aiService = aiService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> AskBot([FromBody] ChatRequestDto request)
    {
        var progress = await _progressService.GetUserProgressAsync(request.UserId);
        var reply = await _aiService.GetResponseAsync(request.Message, progress);
        return Ok(new { reply });
    }
}