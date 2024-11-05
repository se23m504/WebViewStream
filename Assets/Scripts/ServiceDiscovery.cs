using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ServiceDiscovery : MonoBehaviour
{
    private UdpClient udpClient;
    private Action<MdnsService> action;

    private string receivedIp;
    private string receivedPort;
    private string receivedPath;
    private string receivedHost;

    private IPAddress defaultIP;

    private const string multicastAddress = "224.0.0.251";
    private const int multicastPort = 5353;

    private Queue<MdnsService> serviceQueue = new Queue<MdnsService>();

    private IPAddress GetDefaultInterfaceIP()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus == OperationalStatus.Up)
            {
                var ipProps = ni.GetIPProperties();
                if (ipProps.GatewayAddresses.Count > 0)
                {
                    foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.Address;
                        }
                    }
                }
            }
        }

        return null;
    }

    private List<IPAddress> GetRoutableLocalIPs()
    {
        List<IPAddress> localIPs = new List<IPAddress>();

        foreach (IPAddress local in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (local.AddressFamily == AddressFamily.InterNetwork)
            {
                byte[] bytes = local.GetAddressBytes();
                if (bytes[0] == 169 && bytes[1] == 254)
                {
                    Debug.Log($"Skipping non-routable address: {local}");
                    continue;
                }

                localIPs.Add(local);
            }
        }

        return localIPs;
    }

    public void StartListening(Action<MdnsService> action)
    {
        try
        {
            defaultIP = GetDefaultInterfaceIP();
            if (defaultIP == null)
            {
                Debug.LogError("No default interface found. Cannot start multicast listener.");
                return;
            }

            udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(
                SocketOptionLevel.Socket,
                SocketOptionName.ReuseAddress,
                true
            );
            udpClient.Client.Bind(new IPEndPoint(defaultIP, multicastPort));
            udpClient.JoinMulticastGroup(IPAddress.Parse(multicastAddress), defaultIP);

            this.action = action;

            Debug.Log("Listening for service announcements...");

            SendMdnsQuery("_http._tcp.local");

            udpClient.BeginReceive(OnReceive, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error starting UDP listener: {ex.Message}");
        }
    }

    private void SendMdnsQuery(string serviceName)
    {
        byte[] query = CreateMdnsQuery(serviceName);
        Debug.Log($"Sending mDNS query for {serviceName}");

        udpClient.Send(
            query,
            query.Length,
            new IPEndPoint(IPAddress.Parse(multicastAddress), multicastPort)
        );
    }

    private byte[] CreateMdnsQuery(string serviceName)
    {
        ushort transactionId = 0;
        ushort flags = 0x0100;
        ushort questions = 1;
        byte[] header = new byte[12];
        Array.Copy(
            BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)transactionId)),
            0,
            header,
            0,
            2
        );
        Array.Copy(
            BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)flags)),
            0,
            header,
            2,
            2
        );
        Array.Copy(
            BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)questions)),
            0,
            header,
            4,
            2
        );

        byte[] name = EncodeName(serviceName);
        byte[] query = new byte[header.Length + name.Length + 4];
        Array.Copy(header, query, header.Length);
        Array.Copy(name, 0, query, header.Length, name.Length);

        query[query.Length - 4] = 0x00;
        query[query.Length - 3] = 0x0C;
        query[query.Length - 2] = 0x00;
        query[query.Length - 1] = 0x01;

        return query;
    }

    private byte[] EncodeName(string name)
    {
        string[] parts = name.Split('.');
        byte[] result = new byte[name.Length + 2];
        int offset = 0;

        foreach (string part in parts)
        {
            result[offset++] = (byte)part.Length;
            Array.Copy(Encoding.UTF8.GetBytes(part), 0, result, offset, part.Length);
            offset += part.Length;
        }

        result[offset] = 0;
        return result;
    }

    private void OnReceive(IAsyncResult result)
    {
        if (udpClient == null)
        {
            return;
        }

        try
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, multicastPort);
            byte[] receivedBytes = udpClient.EndReceive(result, ref remoteEndPoint);

            ushort flags = BitConverter.ToUInt16(
                new byte[] { receivedBytes[3], receivedBytes[2] },
                0
            );
            if (flags == 0x0100)
            {
                udpClient?.BeginReceive(OnReceive, null);
                return;
            }

            ParseMdnsResponse(receivedBytes);

            udpClient?.BeginReceive(OnReceive, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error receiving UDP message: {ex.Message}");
        }
    }

    private void AddMdnsService()
    {
        if (
            receivedIp != null
            && receivedPort != null
            && receivedHost != null
            && receivedPath != null
        )
        {
            MdnsService currentService = new MdnsService(
                receivedIp,
                int.Parse(receivedPort),
                receivedPath,
                receivedHost
            );
            serviceQueue.Enqueue(currentService);
            Debug.Log($"Added service: {currentService}");
            receivedIp = null;
            receivedPort = null;
            receivedPath = null;
            receivedHost = null;
        }
    }

    private void ParseMdnsResponse(byte[] data)
    {
        int offset = 12;
        ushort questions = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 4));
        ushort answerRRs = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 6));
        ushort additionalRRs = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, 10));

        for (int i = 0; i < questions; i++)
        {
            offset = SkipName(data, offset);
            offset += 4;
        }

        for (int i = 0; i < answerRRs; i++)
        {
            offset = ParseRecord(data, offset);
        }

        for (int i = 0; i < additionalRRs; i++)
        {
            offset = ParseRecord(data, offset);
            AddMdnsService();
        }
    }

    private int ParseRecord(byte[] data, int offset)
    {
        string name;
        (name, offset) = ReadName(data, offset);

        ushort recordType = (ushort)
            IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, offset));
        ushort recordClass = (ushort)
            IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, offset + 2));
        uint ttl = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, offset + 4));
        ushort dataLength = (ushort)
            IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, offset + 8));
        offset += 10;

        if (ttl == 0)
        {
            Debug.LogWarning($"Zero TTL for {name}");
            return offset + dataLength;
        }

        if (recordType == 1) // A Record
        {
            IPAddress ipAddress = new IPAddress(
                new ArraySegment<byte>(data, offset, dataLength).ToArray()
            );
            receivedIp = ipAddress.ToString();
            receivedHost = name;
        }
        else if (recordType == 12) // PTR Record
        {
            string target;
            (target, _) = ReadName(data, offset);
        }
        else if (recordType == 33) // SRV Record
        {
            ushort priority = (ushort)
                IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, offset));
            ushort weight = (ushort)
                IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, offset + 2));
            ushort port = (ushort)
                IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, offset + 4));
            string target;
            (target, _) = ReadName(data, offset + 6);
            receivedPort = port.ToString();
        }
        else if (recordType == 16) // TXT Record
        {
            string txtData = Encoding.UTF8.GetString(data, offset, dataLength);
            if (txtData.Contains("path"))
            {
                receivedPath = txtData.Split('=')[1];
            }
        }
        else if (recordType == 47) // NSEC Record
        {
            Debug.Log($"NSEC Record: {name}");
        }
        else
        {
            Debug.Log($"Unknown Record Type {recordType} for {name}");
        }

        return offset + dataLength;
    }

    private (string, int) ReadName(byte[] data, int offset)
    {
        StringBuilder name = new StringBuilder();
        int originalOffset = offset;
        bool jumped = false;

        while (data[offset] != 0)
        {
            if ((data[offset] & 0xC0) == 0xC0)
            {
                if (!jumped)
                {
                    originalOffset = offset + 2;
                }
                offset = ((data[offset] & 0x3F) << 8) | data[offset + 1];
                jumped = true;
            }
            else
            {
                int length = data[offset++];
                name.Append(Encoding.UTF8.GetString(data, offset, length) + ".");
                offset += length;
            }
        }

        return (name.ToString().TrimEnd('.'), jumped ? originalOffset : offset + 1);
    }

    private int SkipName(byte[] data, int offset)
    {
        while (data[offset] != 0)
        {
            if ((data[offset] & 0xC0) == 0xC0)
            {
                return offset + 2;
            }
            offset += data[offset] + 1;
        }
        return offset + 1;
    }

    private void Update()
    {
        if (serviceQueue.Count > 0)
        {
            Debug.Log($"Queued services: {serviceQueue.Count}");

            while (serviceQueue.Count > 0)
            {
                MdnsService service = serviceQueue.Dequeue();
                Debug.Log($"Invoking action with: {service}");
                action?.Invoke(service);
            }
        }
    }

    private void OnDestroy()
    {
        StopListening();
    }

    private void StopListening()
    {
        udpClient?.DropMulticastGroup(IPAddress.Parse(multicastAddress));
        udpClient?.Close();
        udpClient = null;
    }
}
