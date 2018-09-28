using System;
using System.Collections.Generic;

namespace OpenGloveApp.IO
{
    public interface ICSV
    {
        //For write a FileName in a FolderName in a default ExternalStoragePath, this method return ExternalStoragePath
        string Write(string folderName, string fileName, string columnTitle, List<long> values);
    }
}


