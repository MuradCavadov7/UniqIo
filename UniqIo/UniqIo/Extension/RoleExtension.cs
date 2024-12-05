using UniqIo.Views.Account.Enums;

namespace UniqIo.Extension;

public  static class RoleExtension
{
    public static string GetRole(this Roles role)
    {
        return role switch
        {
            Roles.User => nameof(Roles.User),
            Roles.Admin => nameof(Roles.Admin),
            Roles.Moderator => nameof(Roles.Moderator)

        };
    }
}