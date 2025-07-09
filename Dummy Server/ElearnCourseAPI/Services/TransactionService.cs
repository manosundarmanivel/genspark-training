


using ElearnAPI.Interfaces.Repositories;
using ElearnAPI.Interfaces.Services;
using ElearnAPI.Models;
using System.Threading.Tasks;
using ElearnAPI.DTOs;
using AutoMapper;

namespace ElearnAPI.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto)
        {
            var transaction = _mapper.Map<Transaction>(dto);
            await _repository.AddAsync(transaction);
            return _mapper.Map<TransactionDto>(transaction);
        }

        public async Task<IEnumerable<TransactionDto>> GetAllAsync()
        {
            var transactions = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        public async Task<TransactionDto?> GetByIdAsync(Guid id)
        {
            var transaction = await _repository.GetByIdAsync(id);
            return transaction != null ? _mapper.Map<TransactionDto>(transaction) : null;
        }
    }
}