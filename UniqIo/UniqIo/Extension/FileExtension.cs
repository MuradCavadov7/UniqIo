namespace UniqIo.Extention;

public static class FileExtension
{
  public static bool IsValidType(this string contentType)
    {

        if (contentType.StartsWith("image") )
        {
            return true;
        }
        return false;
    }

    public static bool IsValidSize( this long kb)
    {
        if (kb>5120)
        {
            return false;
        }
        return true;
    }
    public static string Upload(this IFormFile file,string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string newFileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
        string fullPath = Path.Combine(path, newFileName);
        using (Stream stream = File.Create(fullPath))
        {
            file.CopyTo(stream);
        }
        return newFileName;
    }
}
