using System.Net;
using IndiaCamExplorer.Infrastructure.Security;
using Xunit;

namespace IndiaCamExplorer.Tests.Security;

public class UrlSecurityValidatorTests
{
    private readonly UrlSecurityValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-valid-url")]
    [InlineData("ftp://somnath.org/stream")]
    [InlineData("file:///etc/passwd")]
    [InlineData("javascript:alert(1)")]
    public void IsSafePublicUrl_InvalidOrUnsafeSchemes_ReturnsFalse(string? url)
    {
        var result = _validator.IsSafePublicUrl(url, out var reason);
        Assert.False(result);
        Assert.NotNull(reason);
    }

    [Theory]
    [InlineData("http://localhost/stream.m3u8")]
    [InlineData("https://localhost:8080/live")]
    [InlineData("http://127.0.0.1/live")]
    [InlineData("http://127.0.0.2:5000/feed")]
    [InlineData("http://10.0.0.1/cctv")]
    [InlineData("http://10.255.0.1/stream")]
    [InlineData("http://172.16.0.1/feed")]
    [InlineData("http://172.31.255.1/feed")]
    [InlineData("http://192.168.1.1/cam")]
    [InlineData("http://192.168.0.100:8080/stream")]
    [InlineData("http://169.254.169.254/latest/meta-data/")] // AWS/Cloud metadata endpoint
    [InlineData("http://100.64.0.1/camera")]
    [InlineData("http://0.0.0.0:80/stream")]
    public void IsSafePublicUrl_PrivateAndRestrictedIps_ReturnsFalse(string url)
    {
        var result = _validator.IsSafePublicUrl(url, out var reason);
        Assert.False(result);
        Assert.NotNull(reason);
    }

    [Theory]
    [InlineData("127.0.0.1", true)]
    [InlineData("10.1.2.3", true)]
    [InlineData("172.20.0.5", true)]
    [InlineData("192.168.1.1", true)]
    [InlineData("169.254.169.254", true)]
    [InlineData("100.64.0.1", true)]
    [InlineData("8.8.8.8", false)] // Google Public DNS
    [InlineData("1.1.1.1", false)] // Cloudflare Public DNS
    [InlineData("20.88.80.12", false)] // Public IP
    public void IsPrivateOrRestrictedIp_Tests(string ipString, bool expectedRestricted)
    {
        var ip = IPAddress.Parse(ipString);
        var isRestricted = UrlSecurityValidator.IsPrivateOrRestrictedIp(ip);
        Assert.Equal(expectedRestricted, isRestricted);
    }

    [Fact]
    public void IsSafePublicUrl_LegitimatePublicIpUrl_ReturnsTrue()
    {
        var url = "https://1.1.1.1/stream.m3u8";
        var result = _validator.IsSafePublicUrl(url, out var reason);
        Assert.True(result, reason);
        Assert.Null(reason);
    }
}
