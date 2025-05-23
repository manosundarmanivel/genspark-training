namespace ProxyDemo
{
    public class File : IFile
    {
        public void Read()
        {
            Console.WriteLine("[Access Granted] Reading sensitive content...");
        }
    }
}