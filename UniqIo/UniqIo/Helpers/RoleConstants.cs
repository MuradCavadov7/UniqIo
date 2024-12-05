using Humanizer;
using UniqIo.Views.Account.Enums;

namespace UniqIo.Helpers
{
    public class RoleConstants
    {
        public const string AccessToDashboard = nameof(Roles.Admin)+ "," +nameof(Roles.Moderator);
    }
}
