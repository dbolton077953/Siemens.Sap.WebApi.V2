using Microsoft.AspNetCore.Authorization;

namespace Siemens.Sap.WebAPI
{
    public class Policies
    {
        /// <summary>
        /// Represents the admin policy.
        /// </summary>
        public const string SapApiAdminRole = "Siemens Sap WebApi Admin";

        /// <summary>
        /// Defines the admin view policy.
        /// </summary>
        /// <returns>The admin view policy.</returns>
        public static AuthorizationPolicy AdminViewPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole("Siemens Sap WebApi Admin")
                .Build();
        }
    }
}
