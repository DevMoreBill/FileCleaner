using System;
using System.IO;
using System.Linq;

public class FolderCleaner
{
    public static void Main(string[] args)
    {
        // Пример использования:
        Console.Write("Введите путь к папке: ");
        string folderPath = Console.ReadLine();

        try
        {
            long initialSize = GetFolderSize(folderPath);
            Console.WriteLine($"Размер папки до очистки: {GetFileSizeString(initialSize)}");

            Console.WriteLine("Вы действительно хотите очистить папку от древнего 30-минутного зла? (да/нет)");
            string answer = Console.ReadLine();

            if (answer.ToLower() == "да")
            {
                CleanFolder(folderPath);

                long finalSize = GetFolderSize(folderPath);
                long freedSpace = initialSize - finalSize;

                //// Вывод количества удаленных файлов не работает???
                //int deletedFilesCount = GetDeletedFilesCount(folderPath);
                //Console.WriteLine($"Удалено файлов: {deletedFilesCount}");

                Console.WriteLine($"Освобождено места: {GetFileSizeString(freedSpace)}");
                Console.WriteLine($"Размер папки после очистки: {GetFileSizeString(finalSize)}");
            }
            else
            {
                Console.WriteLine("Очистка папки отменена.");
            }
        }
        catch (IOException e)
        {
            Console.Error.WriteLine($"Ошибка: {e.Message}");
        }
        catch (UnauthorizedAccessException e)
        {
            Console.Error.WriteLine("Недостаточно прав доступа к папке.");
        }

    }

    public static void CleanFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Console.Error.WriteLine("Папка не существует.");
            return;
        }

        int deletedFilesCount = 0;
        TimeSpan thresholdTime = TimeSpan.FromMinutes(30);
        DateTime now = DateTime.Now;

        // Удаление файлов
        foreach (string filePath in Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories))
        {
            if (now - File.GetLastWriteTime(filePath) > thresholdTime)
            {
                File.Delete(filePath);
                deletedFilesCount++;
            }
        }

        // Удаление пустых папок
        foreach (string directoryPath in Directory.EnumerateDirectories(folderPath, "*", SearchOption.AllDirectories).Reverse())
        {
            if (Directory.GetFiles(directoryPath).Length == 0 && Directory.GetDirectories(directoryPath).Length == 0)
            {
                Directory.Delete(directoryPath);
                deletedFilesCount++;
            }
        }
    }

    // Функция для подсчета размера папки
    public static long GetFolderSize(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new IOException("Неверный путь к папке.");
        }

        long size = 0;
        foreach (string file in Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories))
        {
            size += new FileInfo(file).Length;
        }
        return size;
    }

    //// Функция для получения количества удаленных файлов
    //public static int GetDeletedFilesCount(string folderPath)
    //{
    //    int deletedFilesCount = 0;
    //    TimeSpan thresholdTime = TimeSpan.FromMinutes(30);
    //    DateTime now = DateTime.Now;

    //    foreach (string filePath in Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories))
    //    {
    //        if (now - File.GetLastWriteTime(filePath) > thresholdTime)
    //        {
    //            deletedFilesCount++;
    //        }
    //    }
    //    return deletedFilesCount;
    //}

    // Функция для форматирования размера в человеческий вид
    public static string GetFileSizeString(decimal size)
    {
        string[] sizes = { "Б", "КБ", "МБ", "ГБ", "ТБ" };
        int order = 0;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            size /= 1024;
            order++;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}
