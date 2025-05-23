using WholeApplication.Models;
using WholeApplication.Interfaces;
using WholeApplication.Repository;
using WholeApplication.Services;

namespace WholeApplication
{
    class Program
    {
        static void Main()
        {
            // IAppointmentRepository repo = new SqlAppointmentRepository("conncection_string");
            IAppointmentRepository repo = new InMemoryAppointmentRepository();
            INotificationService notifier = new ConsoleNotificationService();
            var service = new AppointmentService(repo, notifier);

            while (true)
            {
                Console.WriteLine("\nOptions: 1.Book, 2.View, 3.Reschedule, 4.Cancel, 5.Exit");
                Console.Write("Choose: ");
                var option = Console.ReadLine()?.Trim().ToLower();

                if (option == "5") break;

                switch (option)
                {
                    case "1":
                        Console.Write("Enter patient name: ");
                        string name = Console.ReadLine();

                        Console.Write("Enter appointment date and time (yyyy-MM-dd HH:mm): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime bookDate))
                        {
                            service.BookAppointment(new Appointment { PatientName = name, Date = bookDate });
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format.");
                        }
                        break;

                    case "2":
                        Console.Write("Enter date to view appointments (yyyy-MM-dd): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime viewDate))
                        {
                            service.ViewAppointments(viewDate);
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format.");
                        }
                        break;

                    case "3":
                        Console.Write("Enter patient name: ");
                        string resName = Console.ReadLine();

                        Console.Write("Enter old date and time (yyyy-MM-dd HH:mm): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out DateTime oldDate))
                        {
                            Console.WriteLine("Invalid old date.");
                            break;
                        }

                        Console.Write("Enter new date and time (yyyy-MM-dd HH:mm): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out DateTime newDate))
                        {
                            Console.WriteLine("Invalid new date.");
                            break;
                        }

                        service.RescheduleAppointment(resName, oldDate, newDate);
                        break;

                    case "4":
                        Console.Write("Enter patient name: ");
                        string cancelName = Console.ReadLine();

                        Console.Write("Enter appointment date and time (yyyy-MM-dd HH:mm): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime cancelDate))
                        {
                            service.CancelAppointment(cancelName, cancelDate);
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format.");
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
    }
}