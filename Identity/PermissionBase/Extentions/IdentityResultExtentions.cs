using Microsoft.AspNetCore.Identity;

namespace PermissionBase.Extentions
{
    public static class IdentityResultExtentions
    {
        public static string[] GetErrorsDescription(this IdentityResult result)
        {
            return result.Errors.Select(x => x.Description).ToArray();
        }
    }
}
