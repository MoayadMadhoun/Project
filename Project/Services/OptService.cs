using System.Security.Cryptography;
namespace Project.Services
{
    public class OptService
    {

        public string GenerateOtp()
        {
            int otp = RandomNumberGenerator.GetInt32(100000, 1000000);

            return otp.ToString();
        }
    }
}
