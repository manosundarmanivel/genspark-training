using ElearnAPI.DTOs;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using ElearnAPI.Models;

namespace ElearnAPI.Controllers
{

[ApiController]
[Route("api/v1/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _service;

    public TransactionsController(ITransactionService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var transactions = await _service.GetAllAsync();
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var transaction = await _service.GetByIdAsync(id);
        if (transaction == null)
            return NotFound();

        return Ok(transaction);
    }
}

}