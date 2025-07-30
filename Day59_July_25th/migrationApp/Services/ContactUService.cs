using ChienVHShopOnline.Dtos;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;

namespace ChienVHShopOnline.Services
{
    public class ContactUService : IContactUService
    {
        private readonly IContactURepository _repository;

        public ContactUService(IContactURepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ContactUDto>> GetAllAsync()
        {
            var contacts = await _repository.GetAllAsync();
            return contacts.Select(c => new ContactUDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                Content = c.Content
            });
        }

        public async Task<ContactUDto?> GetByIdAsync(int id)
        {
            var contact = await _repository.GetByIdAsync(id);
            if (contact == null) return null;

            return new ContactUDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Phone = contact.Phone,
                Content = contact.Content
            };
        }

        public async Task<ContactUDto> CreateAsync(CreateContactUDto dto)
        {
            var contact = new ContactU
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Content = dto.Content
            };

            var created = await _repository.CreateAsync(contact);

            return new ContactUDto
            {
                Id = created.Id,
                Name = created.Name,
                Email = created.Email,
                Phone = created.Phone,
                Content = created.Content
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
