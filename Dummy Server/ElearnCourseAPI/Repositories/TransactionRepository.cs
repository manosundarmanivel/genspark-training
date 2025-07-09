using ElearnAPI.Data;
using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElearnAPI.Repositories
{

    public class TransactionRepository : ITransactionRepository
{
    private readonly ElearnDbContext _context;

    public TransactionRepository(ElearnDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
    }

  public async Task<IEnumerable<Transaction>> GetAllAsync()
{
    return await _context.Transactions
        .Include(t => t.User)
        .Include(t => t.Course)
        .OrderByDescending(t => t.CreatedAt)
        .ToListAsync();
}


    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await _context.Transactions.FindAsync(id);
    }
}
}