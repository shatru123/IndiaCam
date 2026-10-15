namespace IndiaCamExplorer.Core.Interfaces;

public interface ISecurityValidator
{
    bool IsSafePublicUrl(string? url, out string? failureReason);
}
