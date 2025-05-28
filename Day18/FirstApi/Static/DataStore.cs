// Static/DataStore.cs
using FirstApi.Models;

namespace FirstApi.Static
{
    public static class DataStore
    {
        public static List<Doctor> Doctors = new List<Doctor>();
        public static List<Patient> Patients = new List<Patient>();
        public static List<Appointment> Appointments = new List<Appointment>();
    }
}
