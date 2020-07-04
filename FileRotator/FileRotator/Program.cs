using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Configuration;
using NLog;

namespace FileRotator
{
    class Program
    {
        static void Main(string[] args)
        {
            logger.LogLine("Backup Rotator V2.0-beta1 Build 13");
            if (!Directory.Exists(SourceDir))
            {
                logger.LogLine(2, "Source directory does not exist");
                return;
            }
            if (!Directory.Exists(DestDir))
            {
                logger.LogLine(1, "The destination directory does not exist, creating.");
                Directory.CreateDirectory(DestDir);
            }
            if (!Directory.Exists(DestDir + "\\Daily"))
            {
                logger.LogLine(1, "The daily backup directory does not exist, creating.");
                Directory.CreateDirectory(DestDir + "\\Daily");
            }
            if (!Directory.Exists(DestDir + "\\Weekly"))
            {

                logger.LogLine(1, "The weekly backup directory does not exist, creating.");
                Directory.CreateDirectory(DestDir + "\\Weekly");
            }
            if (!Directory.Exists(DestDir + "\\Monthly"))
            {
                logger.LogLine(1, "The monthly backup directory does not exist, creating.");
                Directory.CreateDirectory(DestDir + "\\Monthly");
            }
            if (!Directory.Exists(DestDir + "\\Yearly"))
            {
                logger.LogLine(1, "The yearly backup directory does not exist, creating.");
                Directory.CreateDirectory(DestDir + "\\Yearly");
            }
            string[] dirsToMove = Directory.GetDirectories(SourceDir);
            if (dirsToMove.Length == 0)
            {
                logger.LogLine(1, "No source files are present to backup.");
            }
            else
            {
                foreach (string str in dirsToMove)
                {
                    bool hasError = false;
                    bool dailyError = false;
                    bool weeklyError = false;
                    bool monthlyError = false;
                    bool yearlyError = false;
                    string dirName = str.Substring(str.LastIndexOf('\\') + 1);
                    int[] intDate;
                    try
                    {
                        string[] date = dirName.Split(SplitChar);
                        int[] iDate = { Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(date[2]) };
                        intDate = iDate;
                    }
                    catch (Exception ex)
                    {
                        logger.LogLine(2, "There was an error processing the date format of the source folders: " + ex.Message);
                        return;
                    }
                    DateTime Date;
                    try
                    {
                        DateTime dtDate = new DateTime(intDate[0], intDate[1], intDate[2]);
                        Date = dtDate;
                    }
                    catch (Exception ex)
                    {
                        logger.LogLine(2, "There was an error parsing the date into the program: " + ex.Message);
                        return;
                    }
                    logger.LogLine("Running daily backup on file: " + str);
                    dailyError = runDailyBackup(str);
                    if (Date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        logger.LogLine("Running weekly backup on file: " + str);
                        weeklyError = runWeeklyBackup(str);
                    }
                    if (Date.Day == 1)
                    {
                        logger.LogLine("Running monthly backup on file: " + str);
                        monthlyError = runMonthlyBackup(str);
                    }
                    if (Date.DayOfYear == 1)
                    {
                        logger.LogLine("Running yearly backup on file: " + str);
                        yearlyError = runYearlyBackup(str);
                    }
                    if (dailyError || weeklyError || monthlyError || yearlyError)
                    {
                        logger.LogLine(2, "An error occurred during the backup of the source file: " + str + ". As a result, the source file will not be deleted.");
                        hasError = true;
                    }
                    if (!hasError && RemoveSourceFiles)
                    {
                        logger.LogLine("Removing the source directory: " + str);
                        Directory.Delete(str, true);
                    }
                }
            }
            if (RunBackupCleanup())
            {
                logger.LogLine(2, "The backup cleanup encountered an error.");
            }
        }
        static bool runDailyBackup(string DirToBackup)
        {
            try
            {
                string dirName = DirToBackup.Substring(DirToBackup.LastIndexOf('\\') + 1);
                if (!Directory.Exists(DestDir + "\\Daily"))
                    Directory.CreateDirectory(DestDir + "\\Daily");
                DirectoryCopy(DirToBackup, DestDir + "\\Daily\\" + dirName, true);
            }
            catch (Exception ex)
            {
                logger.LogLine(2, "An error occured during the daily backup: " + ex.Message);
                return true;
            }
            return false;
        }
        static bool runWeeklyBackup(string DirToBackup)
        {
            try
            {
                string dirName = DirToBackup.Substring(DirToBackup.LastIndexOf('\\') + 1);
                if (!Directory.Exists(DestDir + "\\Weekly"))
                    Directory.CreateDirectory(DestDir + "\\Weekly");
                DirectoryCopy(DirToBackup, DestDir + "\\Weekly\\" + dirName, true);
            }
            catch (Exception ex)
            {
                logger.LogLine(2, "An error occured during the weekly backup: " + ex.Message);
                return true;
            }
            return false;
        }
        static bool runMonthlyBackup(string DirToBackup)
        {
            try
            {
                string dirName = DirToBackup.Substring(DirToBackup.LastIndexOf('\\') + 1);
                if (!Directory.Exists(DestDir + "\\Monthly"))
                    Directory.CreateDirectory(DestDir + "\\Monthly");
                DirectoryCopy(DirToBackup, DestDir + "\\Monthly\\" + dirName, true);
            }
            catch (Exception ex)
            {
                logger.LogLine(2, "An error occured during the monthly backup: " + ex.Message);
                return true;
            }
            return false;
        }
        static bool runYearlyBackup(string DirToBackup)
        {
            try
            {
                string dirName = DirToBackup.Substring(DirToBackup.LastIndexOf('\\') + 1);
                if (!Directory.Exists(DestDir + "\\Yearly"))
                    Directory.CreateDirectory(DestDir + "\\Yearly");
                DirectoryCopy(DirToBackup, DestDir + "\\Yearly\\" + dirName, true);
            }
            catch (Exception ex)
            {
                logger.LogLine(2, "An error occured during the yearly backup: " + ex.Message);
                return true;
            }
            return false;
        }
        static bool RunBackupCleanup()
        {
            try
            {
                if (DailyBackupsToKeep >= 0)
                {
                    foreach (var dir in new DirectoryInfo(DestDir + "\\Daily").GetDirectories().OrderByDescending(x => x.Name).Skip(DailyBackupsToKeep))
                    {
                        logger.LogLine("Removing the daily backup " + dir.Name + " as it conforms to the backup cleanup requirements.");
                        dir.Delete(true);
                    }
                }
                if (WeeklyBackupsToKeep >= 0)
                {
                    foreach (var dir in new DirectoryInfo(DestDir + "\\Weekly").GetDirectories().OrderByDescending(x => x.Name).Skip(WeeklyBackupsToKeep))
                    {
                        logger.LogLine("Removing the weekly backup " + dir.Name + " as it conforms to the backup cleanup requirements.");
                        dir.Delete(true);
                    }
                }
                if (MonthlyBackupsToKeep >= 0)
                {
                    foreach (var dir in new DirectoryInfo(DestDir + "\\Monthly").GetDirectories().OrderByDescending(x => x.Name).Skip(MonthlyBackupsToKeep))
                    {
                        logger.LogLine("Removing the monthly backup " + dir.Name + " as it conforms to the backup cleanup requirements.");
                        dir.Delete(true);
                    }
                }
                if (YearlyBackupsToKeep >= 0)
                {
                    foreach (var dir in new DirectoryInfo(DestDir + "\\Yearly").GetDirectories().OrderByDescending(x => x.Name).Skip(YearlyBackupsToKeep))
                    {
                        logger.LogLine("Removing the yearly backup " + dir.Name + " as it conforms to the backup cleanup requirements.");
                        dir.Delete(true);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogLine(2, "An error occured during the backup cleanup: " + ex.Message);
                return true;
            }
            return false;
        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);
                DirectoryInfo[] dirs = dir.GetDirectories();
                if (!dir.Exists)
                {
                    logger.LogLine(2, "The backup source directory does not exist or could not be found.");
                    return;
                }
                if (!Directory.Exists(destDirName))
                {
                    logger.LogLine("Creating backup directory: " + destDirName);
                    Directory.CreateDirectory(destDirName);
                }
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    logger.LogLine("Copying file: " + file.Name + " to directory: " + destDirName);
                    string temppath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, false);
                }
                if (copySubDirs)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        logger.LogLine("Begining copy of subdirectory: " + subdir.Name + " within directory: " + destDirName);
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogLine(2, "An error has occured while performing the file copy: " + ex.Message);
            }
        }
    }


}
