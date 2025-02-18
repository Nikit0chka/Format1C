namespace Format1C;

/// <summary>
///     Основной класс
/// </summary>
file abstract class Program
{
    /// <summary>
    ///     Часть пути к файлу 1C
    /// </summary>
    private const string AppDataPartPathTo1CFile = @"1C\1CEStart\ibases.v8i";

    /// <summary>
    ///     Название файла конфигурации
    /// </summary>
    private const string ConfigurationFileName = "Config.txt";

    /// <summary>
    ///     Точка входа
    /// </summary>
    private static async Task Main()
    {
        try
        {
            Console.WriteLine("info: Ехала...");

            var currentDomain = GetCurrentDomainDirectory();
            var configurationContent = await ReadConfigurationFileAsync(currentDomain);
            var databasesToRemove = ParseConfigurationContent(configurationContent);

            LogDatabasesToRemove(databasesToRemove);

            var appDataPath = GetAppDataPath();
            var fullPathTo1CFile = GetFullPathTo1CFile(appDataPath);

            await Process1CFileAsync(fullPathTo1CFile, databasesToRemove);

            Console.WriteLine("info: Файл записан! Пока!");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"err: Произошла ошибка: {ex.Message}");
            Console.ReadKey();
        }
    }

    /// <summary>
    ///     Логика получения текущего домена
    /// </summary>
    /// <returns> </returns>
    private static string GetCurrentDomainDirectory()
    {
        Console.WriteLine("info: Получение директории домена...");
        var currentDomain = AppDomain.CurrentDomain.BaseDirectory;
        Console.WriteLine($"info: Текущая директория домена: {currentDomain}");
        return currentDomain;
    }

    /// <summary>
    ///     Логика чтения файла конфигурации
    /// </summary>
    /// <param name="currentDomainDirectory"> Текущая директория домена </param>
    /// <returns> </returns>
    /// <exception cref="Exception"> </exception>
    private static async Task<string> ReadConfigurationFileAsync(string currentDomainDirectory)
    {
        Console.WriteLine("info: Чтение конфигурации...");
        var configurationFilePath = Path.Combine(currentDomainDirectory, ConfigurationFileName);

        try
        {
            return await File.ReadAllTextAsync(configurationFilePath);
        }
        catch (FileNotFoundException ex)
        {
            throw new($"Файл не найден: {ex.Message}");
        }
        catch (IOException ex)
        {
            throw new($"Ошибка ввода-вывода: {ex.Message}");
        }
    }

    /// <summary>
    ///     Логика парса контента конфигурации
    /// </summary>
    /// <param name="configurationContent"> Контент конфигурации </param>
    /// <returns> </returns>
    private static List<string> ParseConfigurationContent(string configurationContent) => configurationContent.Split(";", StringSplitOptions.RemoveEmptyEntries).ToList();

    /// <summary>
    ///     Вывод бд для удаления
    /// </summary>
    /// <param name="databasesToRemove"> Бд для удаления </param>
    private static void LogDatabasesToRemove(List<string> databasesToRemove)
    {
        Console.Write("info: Бд для удаления: ");

        foreach (var databaseToRemove in databasesToRemove)
        {
            Console.Write($"{databaseToRemove} ");
        }

        Console.WriteLine();
    }

    /// <summary>
    ///     Получение пути к AppData
    /// </summary>
    /// <returns> </returns>
    private static string GetAppDataPath()
    {
        Console.WriteLine("info: Получения пути к AppData...");
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        Console.WriteLine("info: Путь к AppData: " + appDataPath);
        return appDataPath;
    }

    /// <summary>
    ///     Получение полного пути к файлу 1С
    /// </summary>
    /// <param name="appDataPath"> Путь к AppData </param>
    /// <returns> </returns>
    /// <exception cref="Exception"> </exception>
    private static string GetFullPathTo1CFile(string appDataPath)
    {
        var fullPathTo1CFile = Path.Combine(appDataPath, AppDataPartPathTo1CFile);

        if (!File.Exists(fullPathTo1CFile))
            throw new($"Файл 1С по пути {fullPathTo1CFile} отсутствует!");

        return fullPathTo1CFile;
    }

    /// <summary>
    ///     Обработка файла 1С
    /// </summary>
    /// <param name="fullPathTo1CFile"> Полный путь к файлу 1C </param>
    /// <param name="databasesToRemove"> Бд для удаления </param>
    /// <exception cref="Exception"> </exception>
    private static async Task Process1CFileAsync(string fullPathTo1CFile, List<string> databasesToRemove)
    {
        Console.WriteLine("info: Чтение содержимого файла...");

        string content;

        try
        {
            content = await File.ReadAllTextAsync(fullPathTo1CFile);
        }
        catch (FileNotFoundException ex)
        {
            throw new($"Файл 1С не найден!: {ex.Message}");
        }
        catch (IOException ex)
        {
            throw new($"Ошибка ввода-вывода в 1С файл!: {ex.Message}");
        }

        Console.WriteLine("info: Формирование очищенного файла...");
        var newContent = Formatter.DeleteDataBaseInfo(content, databasesToRemove);

        Console.WriteLine("info: Запись в файл...");

        try
        {
            await File.WriteAllTextAsync(fullPathTo1CFile, newContent);
        }
        catch (FileNotFoundException ex)
        {
            throw new($"Файл 1C не найден: {ex.Message}");
        }
        catch (IOException ex)
        {
            throw new($"Ошибка ввода-вывода в файл 1С: {ex.Message}");
        }
    }
}