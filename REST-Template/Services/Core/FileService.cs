using System.IO;
using System.Text;

namespace Backend.Services.Core
{
    public class FileService
    {
        public string ReadFile(string path)
        {
            string readContents;
            string actualPath = Environment.CurrentDirectory + "/Assets/" + path;
            using (StreamReader streamReader = new StreamReader(actualPath, Encoding.UTF8))
            {
                readContents = streamReader.ReadToEnd();
            }

            return readContents;
        }
    }
}
