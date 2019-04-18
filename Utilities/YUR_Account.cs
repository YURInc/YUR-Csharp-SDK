namespace YUR.SDK.Unity.Utilities
{
    public static class YUR_Login_Validation
    {
        /// <summary>
        /// Check if Email is valid for account Login and Creation
        /// </summary>
        /// <param name="Email">User's Email</param>
        /// <returns>Returns true if Password is Valid</returns>
        public static bool IsValidEmail(string Email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(Email);
                return addr.Address == Email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Password is valid for account login and creation
        /// </summary>
        /// <param name="Password">User's Password</param>
        /// <returns>Returns true if Password is Valid</returns>
        public static bool IsValidPassword(string Password)
        {
            return (Password.Length >= 6 && Password.Length <= 64 ? true : false);
        }

        /// <summary>
        /// Check if login credentials are valid
        /// </summary>
        /// <param name="Email">Users Email</param>
        /// <param name="Password">Users Password</param>
        /// <returns>Email and Password are valid if true</returns>
        public static bool IsEmailLoginValid(string Email, string Password)
        {
            return IsEmailLoginValid(Email, Password, out string empty);
        }

        /// <summary>
        /// Check if login credentials are valid
        /// </summary>
        /// <param name="Email">Users Email</param>
        /// <param name="Password">Users Password</param>
        /// <param name="Issue">Out's description of what was wrong</param>
        /// <returns>Email and Password are valid if false</returns>
        public static bool IsEmailLoginValid(string Email, string Password, out string Issue)
        {
            bool valid = false;
            if (IsValidEmail(Email))
            {
                Issue = Email + " is Valid";
                valid = true;
            } 
            else
                Issue = Email + " is not valid";

            if(IsValidPassword(Password))
            {
                Issue += "\nValid Password";

            }
            else
            {
                Issue += "\nInvalid Password";
                valid = false;
            }

            return valid;
        }

    }
}
