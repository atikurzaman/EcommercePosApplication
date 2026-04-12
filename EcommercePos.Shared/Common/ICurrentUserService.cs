namespace EcommercePos.Shared.Common;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserName { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}
