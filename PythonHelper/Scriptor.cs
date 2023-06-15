using IronPython.Hosting;

namespace GarageApp.PythonHelper
{
    public class Scriptor
    {
        public Scriptor() { }

        public void ExecuteScript()
        {
            Microsoft.Scripting.Hosting.ScriptEngine pythonEngine =
                IronPython.Hosting.Python.CreateEngine();

            // Print the default search paths
            Console.Out.WriteLine("Search paths:");
            ICollection<string> searchPaths = pythonEngine.GetSearchPaths();
            foreach (string path in searchPaths)
            {
                Console.Out.WriteLine(path);
            }
            Console.Out.WriteLine();
        }
    }
}