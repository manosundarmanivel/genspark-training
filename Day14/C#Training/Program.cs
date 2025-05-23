

namespace WholeApplication
{
    internal class Program
    {
        public delegate void MyDelegate(int num1, int num2);

        public void Add(int n1, int n2)
        {
            int sum = n1 + n2;
            Console.WriteLine($"The sum of {n1} and {n2} is {sum}");
        }
        public void Product(int n1, int n2)
        {
            int prod = n1 * n2;
            Console.WriteLine($"The product of {n1} and {n2} is {prod}");
        }
        Program()
        {
            // MyDelegate del = new MyDelegate(Add);
            Action<int, int> del = Add;
            del += Product;
            del += (int n1, int n2) =>
            {
                int div = n1 / n2;
                System.Console.WriteLine($"The division of {n1} and {n2} is {div}");
            };

            del(10, 2);

            foreach (MyDelegate d in del.GetInvocationList())
            {
                try
                {
                    d.DynamicInvoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in method {d.Method.Name}: {ex.Message}");
                }

            }

          

                void FindEmployee()
                {
                    int empId = 102;
                    Predicate<Employee> predicate = e => e.Id == empId;
                    Employee? emp = employees.Find(predicate);
                    Console.WriteLine(emp.ToString()??"No such employee");
                }
                void SortEmployee()
                {
                    var sortedEmployees = employees.OrderBy(e => e.Name);
                    foreach (var emp in sortedEmployees)
                    {
                        Console.WriteLine(emp.ToString());
                    }
                }
        }
        static void Main(string[] args)
        {

            Program program = new();
            
        }
    }
}
