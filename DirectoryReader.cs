using System;
using System.IO;
using System.Collections.Generic;

namespace DirectoryReader
{
    /*
     * Steps:
     *   - Retrieve all folders within a given root
     *   - Create Dictionary like object for each folder which contains sub folders and files as well as size
     *   - Output to a Dictionary List based on size (sub folders will need to do this too)
     *   - Write a into a formatted .txt file based on the ordered dict list
     *   - .txt file should look roughly like 
     *   
     *   FolderA    size
     *     SubFolderB    size
     *       file.txt    size
     *     aFileInA.txt    size
     *   FolderB    size
    */
    class MainProgram
    {
        static void Main(string[] args)
        {
            /* Accessing C: comes with an unauthorised exception, except this */
            string fileSystem = "C://Users//J-Cha//Documents//DnD//Guides";

            DirectorySearcher searcher = new DirectorySearcher();

            Console.WriteLine("Reading Directories...");
            DirectoryStats directories = searcher.GetFullDirectory(directoryPath: fileSystem);

            Console.WriteLine("Sorting");
            directories.SortSubdirectories(directories);

            foreach (DirectoryStats sub in directories.subdirectories)
            {
                Console.WriteLine(sub.name);
                Console.WriteLine(sub.size);
            }

            Console.WriteLine("Writing to {0}");

            Console.ReadLine();
        }
    }

    //Holds the stats for a directory. Subdirectories are held in a List of DirectoryStats objects.
    //Can sort based on directory size.
    public class DirectoryStats
    {
        public string name;
        public List<FileStats> files = new List<FileStats>() { };
        public List<DirectoryStats> subdirectories = new List<DirectoryStats>() { };
        public long size = 0;
        public bool access = true;

        public DirectoryStats(string directoryName)
        {
            name = directoryName;
        }
        
        //Sorts by size in Descending order
        public void SortSubdirectories(DirectoryStats dir)
        {
            //Recursively sets subdirectories to sort themselves
            foreach (DirectoryStats subdirectory in dir.subdirectories)
            {
                if (subdirectory.subdirectories.Count != 0)
                {
                    SortSubdirectories(subdirectory);
                }

                //Once all subdirectories have been sorted the directory sorts itself
                dir.subdirectories.Sort((x, y) => y.size.CompareTo(x.size));
            }

            //Sorts the directories files (if there are any)
            if (dir.files.Count != 0)
            {
                dir.files.Sort((x, y) => y.size.CompareTo(x.size));
            }
        }
    }

    public struct FileStats {
        public string name;
        public long size;

        public FileStats(string fileName, long fileSize)
        {
            name = fileName;
            size = fileSize;
        }
    }

    //Searches the given directory and formats the results to a nested DirectoryStats object.
    //Subdirectories are also DirectoryStats objects listed within the subdirectories List.
    class DirectorySearcher
    {
        public DirectoryStats GetFullDirectory(string directoryPath = "C:/")
        {
            return GetDirectories(path: directoryPath);
        }

        private DirectoryStats GetDirectories(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            string[] files, subdirectories = new string[] { };
            DirectoryStats directory = new DirectoryStats(directoryInfo.Name);

            try
            {
                files = Directory.GetFiles(path);
                subdirectories = Directory.GetDirectories(path);
            }

            catch (System.UnauthorizedAccessException)
            {
                directory.access = false;
                return directory;
            }

            //Adds all the directory's files into a dictionary and includes the size of the total files
            if (files.Length > 0)
            {
                List<object> directoryFiles = getDirectoryFiles(files);
                directory.files = (List<FileStats>)directoryFiles[1];
                directory.size += (long)directoryFiles[0];
            }
            
            //Recursively checks all subdirectories for the same criteria. Adding them to subdictionaries
            //Sums up the total directory size as it goes.
            if (subdirectories.Length > 0)
            {
                List<DirectoryStats> subdirectoryList = new List<DirectoryStats>();
                
                foreach(string subPath in subdirectories)
                {
                    DirectoryStats subdirectory = GetDirectories(subPath);
                    directory.size += subdirectory.size;
                    subdirectoryList.Add(subdirectory);
                }

                directory.subdirectories = subdirectoryList;
            }

            return directory;
        }

        //Adds all the files into a dictionary of Name and Size.
        //Also returns the size of all those files as a long
        private List<object> getDirectoryFiles(string[] files)
        {
            List<FileStats> fileList = new List<FileStats>();
            long directorySize = 0;

            foreach (string filePath in files)
            {
                FileInfo file;
                try
                {
                    file = new FileInfo(filePath);
                }
                catch (System.IO.PathTooLongException)
                {
                    continue;
                }

                FileStats fileStats = new FileStats(file.Name, file.Length);

                directorySize += file.Length;
                fileList.Add(fileStats);
            }

            List<object> outputList = new List<object>();
            outputList.Add(directorySize);
            outputList.Add(fileList);

            return outputList;
        }
    }

    //Outputs a dictionary list to a custom formatted .txt file
    class TextWriter
    {

    }
}
