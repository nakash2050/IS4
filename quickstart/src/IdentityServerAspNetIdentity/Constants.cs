namespace IdentityServerAspNetIdentity
{
    public class Constants
    {
        public class CustomClaimNames
        {
            public const string PERMISSION = "permission";
            public const string LOCATION = "location";
        }

        public class RoleNames
        {
            public const string ADMIN = "Admin";
            public const string EMPLOYEE = "Employee";
            public const string DRIVER = "Driver";
            public const string CUSTOMER = "Customer";
        }

        public class EmployeePermissions
        {
            public const string VIEW_RIDE = "ride.view";
        }

        public class AdminPermissions
        {
            public const string MANAGE = "manage";
            public const string MODIFY_RIDE = "ride.modify";
        }

        public class DriverPermissions
        {
            public const string ACCEPT_RIDE = "ride.accept";
            public const string DISCARD_RIDE = "ride.discard";
            public const string VIEW_RIDE = EmployeePermissions.VIEW_RIDE;
        }

        public class CustomerPermissions
        {
            public const string BOOK_RIDE = "ride.book";
            public const string CANCEL_RIDE = "ride.cancel";
            public const string VIEW_RIDE = EmployeePermissions.VIEW_RIDE;
        }
    }
}