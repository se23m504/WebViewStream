using System.Collections;
using System.Collections.Generic;

public class MdnsService
{
    public string IpAddress { get; set; }
    public int Port { get; set; }
    public string Path { get; set; }
    public string Host { get; set; }

    public MdnsService(string ipAddress, int port, string path, string host)
    {
        IpAddress = ipAddress;
        Port = port;
        Path = path;
        Host = host;
    }

    public override string ToString()
    {
        return $"IpAddress: {IpAddress}, Port: {Port}, Path: {Path}, Host: {Host}";
    }

    public override bool Equals(object obj)
    {
        return obj is MdnsService service &&
               IpAddress == service.IpAddress &&
               Host == service.Host &&
               Port == service.Port &&
               Path == service.Path;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 31 + (IpAddress?.GetHashCode() ?? 0);
        hash = hash * 31 + Port.GetHashCode();
        hash = hash * 31 + (Path?.GetHashCode() ?? 0);
        hash = hash * 31 + (Host?.GetHashCode() ?? 0);
        return hash;
    }
}
