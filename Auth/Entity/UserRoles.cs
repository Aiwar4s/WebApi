namespace WebApi.Auth.Entity;

public static class UserRoles
{
    public const string Admin = nameof(Admin);
    public const string BasicUser = nameof(BasicUser);

    public static readonly IReadOnlyCollection<string> All = new[] { Admin, BasicUser };
}
