using System.Text;

namespace Attrecto
{
    public static class PasswordGenerator
    {
        private static readonly int generatedPasswordLength = 16;

        public static string Generate()
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#%*()$?+-=";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < generatedPasswordLength; i++)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
