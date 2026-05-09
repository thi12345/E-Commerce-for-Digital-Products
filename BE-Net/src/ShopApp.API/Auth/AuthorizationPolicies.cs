namespace ShopApp.API.Auth;

public static class AuthorizationPolicies
{
    public const string AdminOnly = nameof(AdminOnly);
    public const string CustomerOrAdmin = nameof(CustomerOrAdmin);

    public const string AdminRole = "Admin";
    public const string CustomerRole = "Customer";
}
