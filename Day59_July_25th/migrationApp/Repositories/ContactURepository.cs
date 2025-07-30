using ChienVHShopOnline.Models;
using ChienVHShopOnline.Data;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class ContactURepository : IContactURepository
    {
        private readonly AppDbContext _context;

        public ContactURepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContactU>> GetAllAsync()
        {
            return await _context.ContactUs.ToListAsync();
        }

        public async Task<ContactU?> GetByIdAsync(int id)
        {
            return await _context.ContactUs.FindAsync(id);
        }

        public async Task<ContactU> CreateAsync(ContactU contact)
        {
            _context.ContactUs.Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.ContactUs.FindAsync(id);
            if (existing == null) return false;

            _context.ContactUs.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
