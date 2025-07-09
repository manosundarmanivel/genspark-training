using ElearnAPI.Models;
using System.Threading.Tasks;
using ElearnAPI.DTOs;


namespace ElearnAPI.Interfaces.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<bool> IsEnrolledAsync(Guid userId, Guid courseId);
        Task AddEnrollmentAsync(Enrollment enrollment);
        Task<Enrollment?> GetEnrollmentAsync(Guid userId, Guid courseId);
        Task RemoveEnrollmentAsync(Enrollment enrollment);
        Task<IEnumerable<Course>> GetEnrolledCoursesAsync(Guid userId);

        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(Guid courseId);

        Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(Guid studentId);

        Task<Enrollment?> GetEnrollmentWithDetailsAsync(Guid userId, Guid courseId);

        Task<List<EnrollmentStatsDto>> GetDailyEnrollmentStatsAsync(int pastDays);
        



       
       

    }
}
