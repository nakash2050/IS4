using System;
using Microsoft.AspNetCore.Authorization;

namespace Api.AuthUtils.PolicyProvider
{
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        internal const string PolicyPrefix = "PERMISSION_";
        private const string Separator = "_";

        public PermissionAuthorizeAttribute(PermissionOperator permissionOperator, params string[] permissions)
        {
            // E.g: PERMISSION_1_Create_Update..
            Policy = $"{PolicyPrefix}{(int)permissionOperator}{Separator}{string.Join(Separator, permissions)}";
        }

        public PermissionAuthorizeAttribute(string permission)
        {
            // E.g: PERMISSION_1_Create..
            Policy = $"{PolicyPrefix}{(int)PermissionOperator.And}{Separator}{permission}";
        }

        public static PermissionOperator GetOperatorFromPolicy(string policyName)
        {
            var @operator = int.Parse(policyName.AsSpan(PolicyPrefix.Length, 1));
            return (PermissionOperator)@operator;
        }

        public static string[] GetPermissionsFromPolicy(string policyName)
        {
            return policyName.Substring(PolicyPrefix.Length + 2)
                .Split(new[] {Separator}, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}