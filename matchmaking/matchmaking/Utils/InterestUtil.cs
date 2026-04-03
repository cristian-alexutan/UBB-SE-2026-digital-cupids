using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


[assembly: InternalsVisibleTo("matchmaking.Tests")]
namespace matchmaking.Utils
{
    internal class InterestUtil
    {
        private string interestsFile;
        private List<String> interests;
       
        public InterestUtil()
        {
            interests = new List<String>();
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            interestsFile = Path.Combine(baseDirectory, "interests.csv");

            if (File.Exists(interestsFile))
            {
                String allLines = File.ReadAllText(interestsFile);
                interests = File.ReadAllLines(interestsFile)
                                    .Select(i => i.Trim().ToLower())
                                    .Where(i => !string.IsNullOrEmpty(i))
                                    .ToList();
            }
            else
            {
                throw new FileNotFoundException("The file doesn't exist");
            }
        }

        public List<String> GetAll()
        {
            return interests;
        }

    }
}
