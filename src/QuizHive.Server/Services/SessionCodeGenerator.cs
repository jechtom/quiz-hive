using System.Collections.Immutable;
using System.Security.Cryptography;

namespace QuizHive.Server.Services
{
    public class SessionCodeGenerator
    {
        readonly char[] options 
            = Enumerable.Range('0', 10).Select(i => (char)i).ToArray();

        readonly int length = 6;

        public string GenerateCode()
            => RandomNumberGenerator.GetString(options, length);
    }
}
