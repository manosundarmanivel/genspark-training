using BankingApp.DTOs;
using BankingApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateAccount(Guid userId,CreateAccountDto dto)
        {
            var result = await _accountService.CreateAccountAsync(userId, dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(Guid id)
        {
            var result = await _accountService.GetAccountByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

       
    }
}
