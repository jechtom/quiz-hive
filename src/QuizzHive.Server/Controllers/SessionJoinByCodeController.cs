using Microsoft.AspNetCore.Mvc;
using QuizzHive.Server.Models;
using QuizzHive.Server.Services;

namespace QuizzHive.Server.Controllers
{
    [ApiController]
    [Route("api/join-code")]
    public class SessionJoinByCodeController : ControllerBase
    {
        private readonly ILogger<SessionJoinByCodeController> _logger;
        private readonly SessionsManager sessionsManager;

        public SessionJoinByCodeController(ILogger<SessionJoinByCodeController> logger, SessionsManager sessionsManager)
        {
            _logger = logger;
            this.sessionsManager = sessionsManager;
        }

        [HttpPost]
        public async Task<SessionJoinResponse> PostAsync(SessionJoinByCodeRequest request)
        {
            string? sessionId = await sessionsManager.GetSessionIdByJoinCodeAsync(request.Code);

            if (string.IsNullOrEmpty(sessionId))
            {
                return new SessionJoinResponse() { Success = false, Token = "" };
            }

            return new SessionJoinResponse() { Success = true, Token = $"abc to" };
        }
    }
}
