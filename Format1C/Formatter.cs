namespace Format1C;

/// <summary>
///     Класс для форматирования файля бд
/// </summary>
internal static class Formatter
{
    /// <summary>
    ///     Логика удаления бд
    /// </summary>
    /// <param name="content"> Контент для поиска </param>
    /// <param name="databasesToRemove"> Бд для удаления </param>
    /// <returns> Контент, после удаления бд </returns>
    internal static string DeleteDataBaseInfo(string content, List<string> databasesToRemove)
    {
        var databasesContent = GetDatabasesContentDictionary(content);

        var result = string.Empty;

        foreach (var keyValue in databasesContent.Where(keyValue => databasesToRemove.All(databaseToRemove => databaseToRemove != keyValue.Key)))
        {
            result += $"[{keyValue.Key}]\n";
            result += $"{keyValue.Value}\n";
        }

        return result;
    }

    /// <summary>
    ///     Логика составления словаря название бд - контент
    /// </summary>
    /// <param name="content"> Контент </param>
    /// <returns> Словарь название бд - контент </returns>
    private static Dictionary<string, string> GetDatabasesContentDictionary(string content)
    {
        var result = new Dictionary<string, string>();
        var sections = content.Split(['['], StringSplitOptions.RemoveEmptyEntries);

        foreach (var section in sections)
        {
            var sectionParts = section.Split([']'], 2);

            if (sectionParts.Length != 2)
                continue;

            var sectionName = sectionParts[0].Trim();
            var sectionContent = sectionParts[1].Trim();
            result[sectionName] = sectionContent;
        }

        return result;
    }
}