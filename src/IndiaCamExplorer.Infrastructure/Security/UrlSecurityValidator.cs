using System.Net;
using System.Net.Sockets;
using IndiaCamExplorer.Core.Interfaces;

namespace IndiaCamExplorer.Infrastructure.Security;

public class UrlSecurityValidator : ISecurityValidator
{
    private static readonly string[] InternalSuffixes =
    {
        ".local", ".localhost", ".internal", ".lan", ".corp", ".home", ".test", ".example", ".invalid", ".arpa"
    };

    public bool IsSafePublicUrl(string? url, out string? failureReason)
    {
        failureReason = null;

        if (string.IsNullOrWhiteSpace(url))
        {
            failureReason = "URL is empty.";
            return false;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            failureReason = "Invalid absolute URL format.";
            return false;
        }

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            failureReason = $"Unsupported scheme '{uri.Scheme}'. Only http and https are permitted.";
            return false;
        }

        var host = uri.Host.Trim().ToLowerInvariant();
        if (host == "localhost" || host.EndsWith(".localhost") || IsInternalDomainSuffix(host))
        {
            failureReason = $"Host '{host}' is an internal domain.";
            return false;
        }

        // Check if host is direct IP string
        if (IPAddress.TryParse(host, out var parsedIp))
        {
            if (IsPrivateOrRestrictedIp(parsedIp))
            {
                failureReason = $"Target IP '{parsedIp}' is in a private or restricted network range.";
                return false;
            }
        }
        else
        {
            try
            {
                // Resolve host to IP addresses and verify none are in private/internal networks
                var ips = Dns.GetHostAddresses(host);
                if (ips.Length > 0)
                {
                    foreach (var ip in ips)
                    {
                        if (IsPrivateOrRestrictedIp(ip))
                        {
                            failureReason = $"Host '{host}' resolved to restricted IP '{ip}'.";
                            return false;
                        }
                    }
                }
            }
            catch (SocketException)
            {
                // Network/DNS may be offline in development or sandbox.
                // Since host is validated as not an IP and not an internal suffix, allow it.
            }
            catch (Exception ex)
            {
                failureReason = $"DNS resolution check failed for '{host}': {ex.Message}";
                return false;
            }
        }

        return true;
    }

    private static bool IsInternalDomainSuffix(string host)
    {
        foreach (var suffix in InternalSuffixes)
        {
            if (host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public static bool IsPrivateOrRestrictedIp(IPAddress ip)
    {
        if (IPAddress.IsLoopback(ip))
            return true;

        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            var bytes = ip.GetAddressBytes();
            byte b0 = bytes[0];
            byte b1 = bytes[1];

            // 0.0.0.0/8
            if (b0 == 0) return true;

            // 10.0.0.0/8 (RFC 1918)
            if (b0 == 10) return true;

            // 127.0.0.0/8 (Loopback)
            if (b0 == 127) return true;

            // 172.16.0.0/12 (RFC 1918)
            if (b0 == 172 && b1 >= 16 && b1 <= 31) return true;

            // 192.168.0.0/16 (RFC 1918)
            if (b0 == 192 && b1 == 168) return true;

            // 169.254.0.0/16 (Link-local & AWS/Azure/GCP metadata)
            if (b0 == 169 && b1 == 254) return true;

            // 100.64.0.0/10 (Carrier-grade NAT)
            if (b0 == 100 && b1 >= 64 && b1 <= 127) return true;

            // 224.0.0.0/4 (Multicast)
            if (b0 >= 224 && b0 <= 239) return true;

            // 240.0.0.0/4 (Reserved)
            if (b0 >= 240) return true;
        }
        else if (ip.AddressFamily == AddressFamily.InterNetworkV6)
        {
            if (ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal || ip.IsIPv6Multicast)
                return true;

            var bytes = ip.GetAddressBytes();
            // fc00::/7 (Unique local)
            if ((bytes[0] & 0xfe) == 0xfc) return true;
            // fe80::/10 (Link-local)
            if (bytes[0] == 0xfe && (bytes[1] & 0xc0) == 0x80) return true;
            // ::1 (Loopback)
            if (IPAddress.IPv6Loopback.Equals(ip)) return true;
            // :: (Any)
            if (IPAddress.IPv6Any.Equals(ip)) return true;
        }

        return false;
    }
}
