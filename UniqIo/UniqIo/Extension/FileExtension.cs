namespace UniqIo.Extention;

public static class FileExtension
{
    public static bool IsValidType(this IFormFile file, string contentType)
    {
        if (file.ContentType.StartsWith(contentType))
        { 
            return true;
        }
        return false;
    }

    public static bool IsValidSize(this IFormFile file, int kb)
    {
        if (file.Length <= kb*1024)
        {
            return true;
        }
        return false;
    }
    public static async Task<string> UploadAsync(this IFormFile file, params string[] paths)
    {
        string uploadpath = Path.Combine(paths);
        if (!Directory.Exists(uploadpath))
        {
            Directory.CreateDirectory(uploadpath);
        }
        string newFileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
        string fullPath = Path.Combine(uploadpath, newFileName);
        using (Stream stream = File.Create(fullPath))
        {
            await file.CopyToAsync(stream);
        }
        return newFileName;
    }
}
