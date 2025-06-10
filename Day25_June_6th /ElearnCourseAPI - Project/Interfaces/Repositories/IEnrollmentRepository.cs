using ElearnAPI.Models;
using System.Threading.Tasks;

namespace ElearnAPI.Interfaces.Repositories
{
    public interface IEnrollmentRepository
    {
       Task<bool> IsEnrolledAsync(Guid userId, Guid courseId);
        Task AddEnrollmentAsync(Enrollment enrollment);
        Task<Enrollment?> GetEnrollmentAsync(Guid userId, Guid courseId);
        Task RemoveEnrollmentAsync(Enrollment enrollment);
        Task<IEnumerable<Course>> GetEnrolledCoursesAsync(Guid userId);
    }
}
