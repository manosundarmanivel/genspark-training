namespace FirstApi.Models.DTOs
{

    public class DoctorSpecialityDTO
    {
        public int DoctorId { get; set; }
        public int SpecialityId { get; set; }

        public string? DoctorName { get; set; }
        public string? SpecialityName { get; set; }
    }
}
