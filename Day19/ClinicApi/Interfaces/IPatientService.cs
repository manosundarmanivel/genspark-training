
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Interfaces;
public interface IPatientService
{
    Task<Patient?> AddPatientAsync(PatientAddDto patientAddDto);
    Task DeletePatient(int id);
    Task<Patient?> UpdatePatient(int id, Patient patient);
    Task<IEnumerable<Patient>?> GetPatientByName(string name);
}