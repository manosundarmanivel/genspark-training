using System.Data;
using System.Data.SqlClient;
using WholeApplication.Interfaces;
using WholeApplication.Models;

namespace WholeApplication.Repository
{
    public class SqlAppointmentRepository : IAppointmentRepository
    {
        private readonly string _connectionString;

        public SqlAppointmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Save(Appointment appointment)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new(
                "INSERT INTO Appointments (PatientName, Date, Description) VALUES (@PatientName, @Date, @Description)",
                connection);

            command.Parameters.AddWithValue("@PatientName", appointment.PatientName);
            command.Parameters.AddWithValue("@Date", appointment.Date);
            command.Parameters.AddWithValue("@Description", appointment.Description ?? "");

            connection.Open();
            command.ExecuteNonQuery();
        }

        public List<Appointment> GetAppointments(DateTime date)
        {
            var appointments = new List<Appointment>();

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new(
                "SELECT Id, PatientName, Date, Description FROM Appointments WHERE CAST(Date AS DATE) = @Date",
                connection);

            command.Parameters.AddWithValue("@Date", date.Date);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                appointments.Add(new Appointment
                {
                    Id = reader.GetInt32(0),
                    PatientName = reader.GetString(1),
                    Date = reader.GetDateTime(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }

            return appointments;
        }

        public void Remove(Appointment appointment)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = new(
                "DELETE FROM Appointments WHERE Id = @Id",
                connection);

            command.Parameters.AddWithValue("@Id", appointment.Id);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
