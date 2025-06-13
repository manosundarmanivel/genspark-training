using ElearnAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElearnAPI.Interfaces.Repositories
{
    public interface ICourseRepository
    {
        Task<Course?> GetByIdAsync(Guid id);
        Task<IEnumerable<Course>> GetAllAsync(int page, int pageSize);
        Task<IEnumerable<Course>> GetByInstructorIdAsync(Guid instructorId, int page, int pageSize);
        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(Course course);
        Task<int> CountAsync();
        Task<IEnumerable<Course>> SearchByNameAsync(string query);

    }
}
