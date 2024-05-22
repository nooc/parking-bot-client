namespace ParkingBot.Exceptions;

internal class ApplicationPermissionError(string source, string message) : Exception(message)
{
    public override string Message => $"{source}: {base.Message}";
}
