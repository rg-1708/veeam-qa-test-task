using System.Timers;
using Timer = System.Timers.Timer;


namespace veeam_qa_test;

class Program
{
    enum OperationType
    {
        Update,
        Delete,
        Create,
        Copy,
        Default,
    }

    
    static string? _sourceFolder;
    static string? _destinationFolder;
    static string? _logFilePath;
    private static Timer _timer = null!;
        
    static void Main(string?[] args)
    {
        if (args.Length < 4)
        {
            Console.WriteLine("Usage: Program.exe <source_folder> <destination_folder> <sync_interval_ms> <log_file_path>");
            return;
        }
        
        _sourceFolder = args[0];
        _destinationFolder = args[1];
        if (!int.TryParse(args[2], out var syncInterval))
        {
            Console.WriteLine("Invalid synchronization interval provided.");
            return;
        }
        _logFilePath = args[3];

        if (!Directory.Exists(_destinationFolder) && _destinationFolder != null)
        {
            Directory.CreateDirectory(_destinationFolder);
        }
        if (!Directory.Exists(_sourceFolder) && _sourceFolder != null)
        {
            Directory.CreateDirectory(_sourceFolder);
        }
        
        SyncFolders(_sourceFolder,_destinationFolder);
        SetTimer(syncInterval);
        
        Log($"Sync started at {DateTime.Now}", OperationType.Default);
        Console.WriteLine("Press the Enter key to exit the application...");
        Console.ReadLine();
        
        _timer.Stop();
        _timer.Dispose();
    }

    private static void SetTimer(int syncInterval)
    {
        _timer = new Timer(syncInterval);
        _timer.Elapsed += OnTimedEvent;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }
    private static void OnTimedEvent(Object source, ElapsedEventArgs e)
    {
        SyncFolders(_sourceFolder,_destinationFolder);
    }

    static void SyncFolders(string? source, string? destination)
    {
        SyncRecursive(source, destination);
    }
    
    static void SyncRecursive(string? source, string? destination)
    {
        if (source != null && destination != null)
        {
            string[] sourceFiles = Directory.GetFiles(source);
            string[] destinationFiles = Directory.GetFiles(destination);
            string?[] sourceSubDirectories = Directory.GetDirectories(source);
            string[] destinationSubDirectories = Directory.GetDirectories(destination);
        
            // Removing redundant directories
            foreach (string destinationSubDirectory in destinationSubDirectories)
            {
                string directoryName = Path.GetFileName(destinationSubDirectory);
                string sourceDirectoryPath = Path.Combine(source, directoryName);
                if (!Directory.Exists(sourceDirectoryPath))
                {
                    Directory.Delete(destinationSubDirectory, true);
                    Log($"Deleted {directoryName} from {destination}", OperationType.Delete);
                }
            }
        
            // Removing redundant files
            foreach (string file in destinationFiles)
            {
                string fileName = Path.GetFileName(file);
                string sourceFilePath = Path.Combine(source, fileName);
                string destinationFilePath = Path.Combine(destination, fileName);
                if (! File.Exists(sourceFilePath))
                {
                    File.Delete(destinationFilePath);
                    Log($"Deleted {fileName} from {destination}", OperationType.Delete);  
                }
            }
        
            // copying files
            foreach (string file in sourceFiles)
            {
                string fileName = Path.GetFileName(file);
                string destinationFilePath = Path.Combine(destination, fileName);
                if (File.Exists(destinationFilePath))
                {
                    if (File.GetLastWriteTime(file) > File.GetLastWriteTime(destinationFilePath))
                    {
                        File.Copy(file, destinationFilePath, true);
                        Log($"Updated {fileName} in {destination}", OperationType.Update);
                    }
                }
                else
                {
                    File.Copy(file, destinationFilePath);
                    Log($"Copied {fileName} to {destination}", OperationType.Copy);
                }
            }
        
            // Copying dirs
       
            foreach (string? subDirectory in sourceSubDirectories)
            {
                string subDirectoryName = Path.GetFileName(subDirectory);
                string? destinationSubDirectory = Path.Combine(destination, subDirectoryName);
                if (!Directory.Exists(destinationSubDirectory))
                {
                    Directory.CreateDirectory(destinationSubDirectory);
                    Log($"Created directory {subDirectoryName} in {destination}", OperationType.Copy);
                }
                SyncRecursive(subDirectory, destinationSubDirectory);
            }
        }
    }


    
    static void Log(string message, OperationType type)
    {
  
        int strlen = message.Length;
        Console.Write("\n");
        ConsoleLogLine(strlen);
        Console.Write("| ");
        switch (type)
        {
            case OperationType.Update :
                Console.ForegroundColor = ConsoleColor.Yellow; break;
            case OperationType.Create: case OperationType.Copy :
                Console.ForegroundColor = ConsoleColor.Green; break;
            case OperationType.Delete :
                Console.ForegroundColor = ConsoleColor.Red; break;
            default :
                Console.ForegroundColor = ConsoleColor.White; break;
        }
        Console.Write($"{message}");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(" |\n");
        ConsoleLogLine(strlen);

        if (_logFilePath != null)
            using (StreamWriter writer = File.AppendText(_logFilePath))
            {
                writer.Write("\n");
                FileLogLine(writer, strlen);
                writer.WriteLine($"| {message} |");
                FileLogLine(writer, strlen);
            }
    }

    private static void ConsoleLogLine(int strlen)
    {
        Console.Write("| ");
        for (int i = 0; i < strlen; i++)
        {
            Console.Write("-");
        }
        Console.Write(" |\n");
    }
    
    private static void FileLogLine(StreamWriter writer, int strlen)
    {
        writer.Write("| ");
        for (int i = 0; i < strlen; i++)
        {
            writer.Write("-");
        }
        writer.Write(" |\n");
    }
    
}