namespace AuthService.Application;

public record RegisterRequest(string TenantId, string Email, string Password, string Role);
public record LoginRequest(string TenantId, string Email, string Password);
public record RefreshRequest(string TenantId, string RefreshToken);
public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc);

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken ct);
}

public interface IJwtTokenService
{
    string GenerateAccessToken(Guid userId, string email, string tenantId, IEnumerable<string> roles);
    string GenerateRefreshToken();
}
