using System.Security.Cryptography;
using Arahk.Neighbor.Application.Interfaces;
using Arahk.Neighbor.Domain.Constants;

namespace Arahk.Neighbor.Infrastructure.Security;

public class RandomOtpGenerator : IOtpGenerator
{
    public string GenerateSixDigitCode()
    {
        var value = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return value.ToString($"D{AuthConstants.OtpLength}");
    }
}
