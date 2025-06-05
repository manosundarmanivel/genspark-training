using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using DoctorAppointment.Policies;

namespace DoctorAppointment.Misc
{
    public class MinimumExperienceHandler : AuthorizationHandler<MinimumExperienceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumExperienceRequirement requirement)
        {
            var experienceClaim = context.User.FindFirst("YearsOfExperience");

            if (experienceClaim != null && int.TryParse(experienceClaim.Value, out int years))
            {
                if (years >= requirement.MinimumYears)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
