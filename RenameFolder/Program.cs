#region

using Newtonsoft.Json;
using System.Text.RegularExpressions;

#endregion

namespace RenameFolder
{
    internal class Program
    {
        private static string? sDir = @"E:\SteamLibrary\steamapps\workshop\content\431960";
        private static int errorCount;
        private static string name;

        private static void Main(string[] args)
        {
            Console.WriteLine(@"Exemple: E:\SteamLibrary\steamapps\workshop\content\431960 ");
            Console.Write("Enter Path of workshot content: ");
            sDir = Console.ReadLine() == "" ? sDir : Console.ReadLine();

            string[] files = Directory.GetFiles(sDir, "project.json", SearchOption.AllDirectories);

            foreach (string data in files)
            {
                if (data.Contains("project.json"))
                {
                    try
                    {
                        string readText = File.ReadAllText(data);

                        dynamic? results = JsonConvert.DeserializeObject<dynamic>(readText);

                        name = results.title;

                        if (!Directory.Exists(sDir + @"\" + RemoveInvalidChars(name)))
                            Directory.Move(Path.GetDirectoryName(data), sDir + @"\" + RemoveInvalidChars(name));
                        else
                        {
                            if (sDir + @"\" + RemoveInvalidChars(name) != Path.GetDirectoryName(data))
                            {
                                string dirName = RemoveInvalidChars(name);
                                string[] file = Directory.GetDirectories(sDir);
                                int count = file.Count(file => { return file.Contains(dirName); });

                                string newFileName = count == 0 ? RemoveInvalidChars(name) : string.Format("{0} ({1})", dirName, count + 1);
                                Directory.Move(Path.GetDirectoryName(data), sDir + @"\" + newFileName);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine("Error: " + sDir + @"\" + RemoveInvalidChars(name));
                        errorCount++;
                    }
                }
                else
                {
                    Console.WriteLine("No project.json found");
                }
            }
            Console.WriteLine("\nError Count: " + errorCount);
        }

        public static string RemoveInvalidChars(string filename)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            filename = r.Replace(filename, "").Replace("?", "").Replace("/", "");
            return filename;
        }
    }
}
