using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class ServiceDiscovery : MonoBehaviour
{
    private UdpClient udpClient;
    private Action<string, string> action;

    private string receivedIp;
    private string receivedPort;
    private bool messageReceived = false;

    private const string multicastAddress = "224.0.0.251";
    private const int multicastPort = 5353;

    public void StartListening(Action<string, string> action)
    {
        try
        {
            udpClient = new UdpClient();
            udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, multicastPort));
            udpClient.JoinMulticastGroup(IPAddress.Parse(multicastAddress));

            this.action = action;

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
        if (udpClient == null)
        {
            return;
        }

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
                receivedIp = ip.ToString();
                receivedPort = port.ToString();
                messageReceived = true;

                StopListening();
            }
            else
            {
                udpClient?.BeginReceive(OnReceive, null);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error receiving UDP message: {ex.Message}");
        }
    }

    private void Update()
    {
        if (messageReceived)
        {
            action?.Invoke(receivedIp, receivedPort);
            messageReceived = false;
        }
    }

    private void OnApplicationQuit()
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

