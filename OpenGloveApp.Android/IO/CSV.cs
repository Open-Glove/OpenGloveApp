using System.Collections.Generic;
using Android.OS;
using Java.IO;
using OpenGloveApp.Droid.IO;
using OpenGloveApp.IO;

[assembly: Xamarin.Forms.Dependency(typeof(CSV))]
namespace OpenGloveApp.Droid.IO
{
    public class CSV : ICSV
    {
        public string FolderName { get; set; }
        public string FileName { get; set; }
        public string ExternalStoragePath { get; set; }

        public string Write(string folderName, string fileName, string columnTitle, List<long> values)
        {
            try
            {
                FolderName = folderName;
                FileName = fileName;

                File folder = new File(Environment.ExternalStorageDirectory
                        + "/" + FolderName);

                bool success = true;
                if (!folder.Exists())
                    success = folder.Mkdirs();

                if (success)
                {
                    string pathName = folder + "/" + fileName;
                    ExternalStoragePath = pathName;

                    PrintWriter pw = new PrintWriter(new File(pathName));
                    pw.Write(columnTitle + '\n');

                    foreach (long value in values)
                    {
                        pw.Write(value.ToString() + '\n');
                    }
                    pw.Close();
                    System.Diagnostics.Debug.WriteLine("Save file is done!");
                    System.Diagnostics.Debug.WriteLine(this.GetStorageInfo());

                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Folder no created, please enable WriteExternalStorage permission");
                    System.Diagnostics.Debug.WriteLine(this.GetStorageInfo());
                }

            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }

            return (this.ExternalStoragePath);
        }

        private string GetStorageInfo()
        {
            return "FolderName: " + FolderName +
                "\nFileName: " + FileName +
                "\nExternal StoragePath:" + ExternalStoragePath;
        }
    }
}
