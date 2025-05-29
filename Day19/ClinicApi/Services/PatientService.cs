

using System.Threading.Tasks;
using SecondWebApi.Interfaces;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

public class PatientService : IPatientService
{
    private readonly IRepository<int, Patient> _repository;

    public PatientService(IRepository<int, Patient> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Repository cannot be null");
    }

    public async Task<Patient?> AddPatientAsync(PatientAddDto patientAddDto)
    {
        try
        {

            if (patientAddDto == null)
            {
                throw new ArgumentNullException("Patient cannot be null");
            }
            var patient = new Patient
            {
                Name = patientAddDto.Name,
                Age = patientAddDto.Age,
                Email = patientAddDto.Email,
                Phone = patientAddDto.Phone
            };
            var addedPatient = await _repository.Add(patient);
            return addedPatient;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding patient: {ex.Message}");
            return null;
        }
    }
    public async Task DeletePatient(int id)
    {
        try
        {

            await _repository.Delete(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting patient with ID {id}: {ex.Message}");
        }
    }
    public async Task<Patient?> UpdatePatient(int id, Patient patient)
    {
        try
        {
            return await _repository.Update(id, patient);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating patient with ID {id}: {ex.Message}");
            return null;
        }
    }
    public async Task<IEnumerable<Patient>?> GetPatientByName(string name)
    {
        try
        {
            var patients = await _repository.GetAll();
            if (patients.Count() == 0)
            {
                throw new Exception("No Patients Found");
            }
            var FilteredPatients = patients.Where(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return FilteredPatients;
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving patient with name : {name}: {ex.Message}");
            return null;
        }
    }
}