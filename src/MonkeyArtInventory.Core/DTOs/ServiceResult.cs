namespace MonkeyArtInventory.Core.DTOs;

public record ServiceResult(bool Success, string Message)
{
    public static ServiceResult Ok(string message = "OK") => new(true, message);
    public static ServiceResult Fail(string message) => new(false, message);
}
