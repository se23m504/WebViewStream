using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ServiceDiscovery : MonoBehaviour
{
    public EndpointLoader endpointLoader;

    private UdpClient udpClient;
    private const int multicastPort = 5353;
    private const string multicastAddress = "224.0.0.251";

    private void Start()
    {
        // StartListening();
    }

    private void StartListening()
    {
        try
        {
            udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, multicastPort));
            udpClient.JoinMulticastGroup(IPAddress.Parse(multicastAddress));

            Debug.Log("Listening for service announcements...");

            udpClient.BeginReceive(OnReceive, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error starting UDP listener: {ex.Message}");
        }
    }

    private void OnReceive(IAsyncResult result)
    {
        try
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, multicastPort);
            byte[] receivedBytes = udpClient.EndReceive(result, ref remoteEndPoint);
            string receivedMessage = Encoding.UTF8.GetString(receivedBytes);

            Debug.Log($"Received message: {receivedMessage} from {remoteEndPoint}");

            string[] parts = receivedMessage.Split(':');
            if (parts.Length == 3 && IPAddress.TryParse(parts[1], out IPAddress ip))
            {
                int port = int.Parse(parts[2]);
                ConnectToService(ip.ToString(), port);
            }

            // udpClient.BeginReceive(OnReceive, null);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error receiving UDP message: {ex.Message}");
        }
    }

    private void ConnectToService(string ipAddress, int port)
    {
        Debug.Log($"Connecting to service at {ipAddress}:{port}");
        endpointLoader.UpdateApiUrl($"http://{ipAddress}:{port}/api/endpoints");
    }

    private void OnApplicationQuit()
    {
        udpClient?.DropMulticastGroup(IPAddress.Parse(multicastAddress));
        udpClient?.Close();
    }
}

