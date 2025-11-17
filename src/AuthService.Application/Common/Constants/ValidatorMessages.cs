

namespace AuthService.Application.Common.Constants
{
    /// <summary>
    /// Validations constant messages
    /// </summary>
    public static class ValidatorMessages
    {
        // Name
        public const string NameRequired = "Name is required";
        public const string NameRequiresMinLenght = "Name must be at least 2 characters";

        // Username
        public const string UsernameRequired = "Username is required;";
        public const string UsernameRequiredMinLenght = "Username must be at least 2 characters";

        // Email
        public const string EmailRequired = "Email is required";
        public const string InvalidEmailFormat = "Invalid email format";

        // Password
        public const string PasswordRequired = "Password is required";
        public const string PasswordRequiresUppercase = "Must contain at least one uppercase letter";
        public const string PasswordRequiresLowercase = "Must contain at least one lowercase letter";
        public const string PasswordRequiresDigit = "Must contain at least one digit";
        public const string PasswordRequiresNonAlphanumeric = "Must contain at least one non-alphanumeric character";
    }
}
