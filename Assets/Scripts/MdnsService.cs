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
}
