namespace ShopApp.Infrastructure.Services;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
    public int ExpirationMinutes { get; init; } = 60;

    public static JwtOptions FromConfiguration(Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        var expirationValue = configuration[$"{SectionName}:ExpirationMinutes"];
        var expirationMinutes = int.TryParse(expirationValue, out var value) ? value : 60;

        return new JwtOptions
        {
            Issuer = configuration[$"{SectionName}:Issuer"] ?? string.Empty,
            Audience = configuration[$"{SectionName}:Audience"] ?? string.Empty,
            Key = configuration[$"{SectionName}:Key"] ?? string.Empty,
            ExpirationMinutes = expirationMinutes
        };
    }
}
