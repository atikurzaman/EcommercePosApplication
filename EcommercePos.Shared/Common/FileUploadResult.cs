namespace EcommercePos.Shared.Common;

public class FileUploadResult
{
    public bool Success { get; set; }
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public string? FullPath { get; set; }
    public string? ContentType { get; set; }
    public long FileSize { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ImageResult
{
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsMain { get; set; }
}

