namespace ElearnAPI.DTOs
{
    public class EnrollDto
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
    }
     public class EnrollmentStatsDto
    {
        public DateTime Date { get; set; }
        public int EnrollmentCount { get; set; }
    }
}
