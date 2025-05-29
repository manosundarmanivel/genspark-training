using SecondWebApi.Contexts;
using SecondWebApi.Interfaces;
using SecondWebApi.Interfaces.Mappers;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;
using SecondWebApi.Services.Mappers;

public class DoctorService : IDoctorService
{
    private readonly IRepository<int, Doctor> _doctorRepository;
    private readonly IRepository<int, Speciality> _specialityRepository;
    private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;

    private readonly IDoctorMapper _doctorMapper;
    private readonly IDoctorSpecialityMapper _doctorSpecialityMapper;

    private readonly ClinicContext _context;



    public DoctorService(
        IRepository<int, Doctor> doctorRepository,
        IRepository<int, Speciality> specialityRepository,
        IRepository<int, DoctorSpeciality> doctorSpecialityRepository,
        IDoctorMapper doctorMapper,
        IDoctorSpecialityMapper doctorSpecialityMapper,
        ClinicContext context
        )
    {
        _doctorRepository = doctorRepository;
        _specialityRepository = specialityRepository;
        _doctorSpecialityRepository = doctorSpecialityRepository;
        _doctorMapper = doctorMapper;
        _doctorSpecialityMapper = doctorSpecialityMapper;
        _context = context;


    }

    public async Task<Doctor?> AddDoctor(DoctorAddDto doctorAddDto)
    {
        try
        {
            if (doctorAddDto == null || string.IsNullOrWhiteSpace(doctorAddDto.Name))
                throw new ArgumentException("Invalid doctor details.");

            var allSpecialities = await _specialityRepository.GetAll();
            var specialityLookup = allSpecialities.ToDictionary(s => s.Name.ToLower(), s => s);

            var matchedSpecialities = new List<Speciality>();

            if (doctorAddDto.specialities != null && doctorAddDto.specialities.Any())
            {
                foreach (var specialityDto in doctorAddDto.specialities)
                {
                    if (specialityDto?.Name == null) continue;

                    var specialityNameLower = specialityDto.Name.ToLower();
                    if (specialityLookup.ContainsKey(specialityNameLower))
                    {
                        matchedSpecialities.Add(specialityLookup[specialityNameLower]);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Speciality '{specialityDto.Name}' does not exist in the system.");
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("At least one speciality must be provided for the doctor.");
            }

            var doctor = _doctorMapper.MapFromAddDto(doctorAddDto);

            var addedDoctor = await _doctorRepository.Add(doctor);
            if (addedDoctor == null)
                throw new InvalidOperationException("Failed to add doctor.");

            var doctorSpecialities = _doctorSpecialityMapper.MapFromDoctorAndSpecialities(addedDoctor.Id, matchedSpecialities);

            foreach (var ds in doctorSpecialities)
            {

                await _doctorSpecialityRepository.Add(ds);
            }

            return addedDoctor;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Adding Doctor: {ex.Message}");
            return null;
        }
    }

    public async Task<Doctor?> GetDoctorByName(string name)
    {
        try
        {
            var allDoctors = await _doctorRepository.GetAll();
            var doctor = allDoctors.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (doctor == null)
                throw new InvalidOperationException($"Doctor with name '{name}' not found.");



            return doctor;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Fetching Doctor By Name: {ex.Message}");
            return null;
        }
    }


public async Task<ICollection<DoctorSpecialityResponseDto>> GetDoctorsBySpeciality(string specialityName)
{
    try
    {
        var doctors = await _context.GetDoctorsBySpecialityFromSP(specialityName);

        if (!doctors.Any())
            throw new InvalidOperationException($"No doctors found for speciality '{specialityName}'.");

        var responseDtos = new List<DoctorSpecialityResponseDto>();
        foreach (var doctor in doctors)
        {
            responseDtos.Add(_doctorMapper.MapToSpecialityResponse(doctor));
        }

        return responseDtos;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error Fetching Doctors By Speciality: {ex.Message}");
        return new List<DoctorSpecialityResponseDto>();
    }
}


    // public async Task<ICollection<Doctor>> GetDoctorsBySpeciality(string specialityName)
    // {
    //     try
    //     {
    //         var allDoctors = await _doctorRepository.GetAll();

    //         var doctors = allDoctors
    //             .Where(d => d.DoctorSpecialities != null &&
    //                         d.DoctorSpecialities.Any(ds => ds.Speciality != null &&
    //                                                        ds.Speciality.Name.Equals(specialityName, StringComparison.OrdinalIgnoreCase)))
    //             .ToList();

    //         if (!doctors.Any())
    //             throw new InvalidOperationException($"No doctors found for speciality '{specialityName}'.");

    //         return doctors;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error Fetching Doctors By Speciality: {ex.Message}");
    //         return new List<Doctor>();
    //     }
    // }

}
