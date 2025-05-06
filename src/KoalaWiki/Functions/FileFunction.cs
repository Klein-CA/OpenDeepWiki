using System.ComponentModel;
using System.Text;
using KoalaWiki.KoalaWarehouse;
using Microsoft.SemanticKernel;

namespace KoalaWiki.Functions;

/// <summary>
/// 文件操作功能类，提供读取文件内容的相关方法。
/// </summary>
/// <param name="gitPath">Git仓库的根路径。</param>
public class FileFunction(string gitPath)
{
    /// <summary>
    /// 异步读取指定路径的文件内容，并返回一个字典，键为文件路径，值为文件内容。
    /// </summary>
    /// <param name="filePaths">文件路径数组。</param>
    /// <returns>返回一个字典，键为文件路径，值为文件内容。</returns>
    [KernelFunction, Description("Read the specified file content")]
    [return: Description("Return the dictionary. The key is the directory name")]
    public async Task<Dictionary<string, string>> ReadFilesAsync(
        [Description("File Path")] string[] filePaths)
    {
        try
        {
            if (DocumentContext.DocumentStore?.Files != null)
            {
                DocumentContext.DocumentStore.Files.AddRange(filePaths);
            }

            var dic = new Dictionary<string, string>();
            foreach (var filePath in filePaths)
            {
                var item = Path.Combine(gitPath, filePath.TrimStart('/'));
                if (!File.Exists(item))
                {
                    continue;
                }

                Console.WriteLine($"Reading file: {item}");

                var info = new FileInfo(item);

                // 判断文件大小
                if (info.Length > 1024 * 1024 * 1)
                {
                    throw new Exception($"File too large: {item} ({info.Length / 1024 / 1024}MB)");
                }

                await using var stream = new FileStream(item, FileMode.Open, FileAccess.Read);
                using var reader = new StreamReader(stream);
                dic.Add(item, await reader.ReadToEndAsync());
            }

            return dic;
        }
        catch (Exception ex)
        {
            // 处理异常
            Console.WriteLine($"Error reading file: {ex.Message}");
            throw new Exception($"Error reading file: {ex.Message}");
        }
    }

    /// <summary>
    /// 异步读取指定路径的文件内容。
    /// </summary>
    /// <param name="filePath">文件路径。</param>
    /// <returns>返回文件内容，如果文件不存在或文件过大，返回相应的错误信息。</returns>
    [KernelFunction, Description("Read the specified file content")]
    public async Task<string> ReadFileAsync(
        [Description("File Path")] string filePath)
    {
        try
        {
            if (DocumentContext.DocumentStore?.Files != null)
            {
                DocumentContext.DocumentStore.Files.Add(filePath);
            }

            filePath = Path.Combine(gitPath, filePath.TrimStart('/'));
            Console.WriteLine($"Reading file: {filePath}");

            var info = new FileInfo(filePath);
            // 判断文件是否存在
            if (!info.Exists)
            {
                return $"File not found: {filePath}";
            }

            // 判断文件大小
            if (info.Length > 1024 * 1024 * 1)
            {
                return $"File too large: {filePath} ({info.Length / 1024 / 1024}MB)";
            }

            await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            // 处理异常
            Console.WriteLine($"Error reading file: {ex.Message}");
            return $"Error reading file: {ex.Message}";
        }
    }

    /// <summary>
    /// 从指定行数开始读取文件内容。
    /// </summary>
    /// <param name="filePath">文件路径。</param>
    /// <param name="startLine">开始行号。</param>
    /// <param name="endLine">结束行号。</param>
    /// <returns></returns>
    [KernelFunction, Description("Read the file content from the specified number of lines")]
    public async Task<string> ReadFileFromLineAsync(
        [Description("File Path")] string filePath,
        [Description("Start Line Number")] int startLine = 0,
        [Description("End Line Number")] int endLine = 5)
    {
        try
        {
            filePath = Path.Combine(gitPath, filePath.TrimStart('/'));
            Console.WriteLine($"Reading file from line {startLine}: {filePath}");
            var lines = await File.ReadAllLinesAsync(filePath);

            if (startLine < 0 || startLine >= lines.Length)
            {
                return $"Invalid start line: {startLine}";
            }

            if (endLine < startLine || endLine >= lines.Length)
            {
                return $"Invalid end line: {endLine}";
            }

            var result = new StringBuilder();
            for (var i = startLine; i <= endLine; i++)
            {
                result.AppendLine(lines[i]);
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            // 处理异常
            Console.WriteLine($"Error reading file: {ex.Message}");
            return $"Error reading file: {ex.Message}";
        }
    }
}