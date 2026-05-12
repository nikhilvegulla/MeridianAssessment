namespace MeridianAssessment;

public class FileHelper
{
        public static void WriteToFile(string filePath, string content)
        {
            try
            {
                File.WriteAllText(filePath, content);
                Console.WriteLine($"Content successfully written to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }

        public static void WriteAllBytes(string filePath, byte[] data)
        {
            try
            {
                File.WriteAllBytes(filePath, data);
                Console.WriteLine($"Bytes successfully written to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing bytes to file: {ex.Message}");
            }
        }

    public static string ReadFromFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                Console.WriteLine($"Content successfully read from {filePath}");
                return content;
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading from file: {ex.Message}");
            return string.Empty;
        }
    }

    public static byte[]? ReadAllBytes(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                var bytes = File.ReadAllBytes(filePath);
                Console.WriteLine($"Bytes successfully read from {filePath}");
                return bytes;
            }

            Console.WriteLine($"File not found: {filePath}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading bytes from file: {ex.Message}");
            return null;
        }
    }

    public static void RemoveFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine($"File successfully removed: {filePath}");
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing file: {ex.Message}");
        }
    }
}