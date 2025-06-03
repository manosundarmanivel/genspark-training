using Microsoft.AspNetCore.Authorization;


namespace DoctorAppointment.Policies
{
    public class MinimumExperienceRequirement : IAuthorizationRequirement
    {
        public int MinimumYears { get; }

        public MinimumExperienceRequirement(int years)
        {
            MinimumYears = years;
        }
    }

}
