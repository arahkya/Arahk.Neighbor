using Arahk.Neighbor.Infrastructure.Security;
using FluentAssertions;

namespace Arahk.Neighbor.Infrastructure.Tests;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_ThenVerify_Succeeds()
    {
        var hasher = new Pbkdf2PasswordHasher();
        var hash = hasher.Hash("Pass1234");
        hasher.Verify("Pass1234", hash).Should().BeTrue();
        hasher.Verify("wrong", hash).Should().BeFalse();
    }
}
