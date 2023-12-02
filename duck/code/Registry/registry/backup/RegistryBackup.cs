using System;
using System.Diagnostics;
using System.IO;

namespace duck.code.RegistryBackuper.registry.backup
{
    public class RegistryBackup
    {
        private static readonly string[] RootKeys = new string[]
        {
        "HKEY_CLASSES_ROOT",
        "HKEY_CURRENT_USER",
        "HKEY_LOCAL_MACHINE",
        "HKEY_USERS",
        "HKEY_CURRENT_CONFIG"
        };
        private static readonly string backupDirectory = "./data/registry/backup/";

        public static Boolean OpenBackupDirectory()
        {
            try
            {
                Process.Start("explorer.exe", Path.GetFullPath(backupDirectory));
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Could not open backup directory");
                return false;
            }
            return true;
        }

        public static Boolean BackupFullRegistry()
        {
            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            string timestamp = GetTimestamp();
            var tasks = new List<Task<bool>>();
            foreach (var rootKey in RootKeys)
            {
                string backupFilePath = Path.Combine(backupDirectory, timestamp + rootKey.Replace("\\", "_") + ".reg");
                var task = Task.Run(() =>
                {
                    Tuple<bool, string> result = ExportRegistryKey(rootKey, backupFilePath);
                    Console.WriteLine(result.Item2);
                    return result.Item1;
                });
                tasks.Add(task);
            }

            try
            {
                Task.WaitAll(tasks.ToArray());
                foreach (var task in tasks)
                {
                    if (!task.Result)
                    {
                        return false;
                    }
                }
            }
            catch (AggregateException)
            {
                return false;
            }
            return true;
        }

        private static Tuple<bool, string> ExportRegistryKey(string registryKeyPath, string backupFilePath)
        {
            Tuple<bool, string> result = new Tuple<bool, string>(false, "Error not set");
            if (string.IsNullOrEmpty(registryKeyPath) || string.IsNullOrEmpty(backupFilePath))
            {
                result = new Tuple<bool, string>(false, "Error: registryKeyPath or backupFilePath is null or empty");
            }

            ProcessStartInfo startInfo = new()
            {
                FileName = "reg.exe",
                Arguments = $"export \"{registryKeyPath}\" \"{backupFilePath}\" /y",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            if (startInfo == null)
            {
                result = new Tuple<bool, string>(false, "Error: startInfo is null");
            }
            else
            {
                using Process? process = Process.Start(startInfo);
                if (process == null)
                {
                    result = new Tuple<bool, string>(false, "Error: process is null");
                }
                else
                {
                    process.WaitForExit();
                    string error = process.StandardError.ReadToEnd();
                    if (process.ExitCode != 0)
                    {
                        result = new Tuple<bool, string>(false, $"Error: {error}");
                    }
                    else
                    {
                        result = new Tuple<bool, string>(true, $"Success: {backupFilePath}");
                    }
                }
            }
            return result;
        }
        private static string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}