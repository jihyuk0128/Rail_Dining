using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using UnityEngine;

public class Udp
{
    private const int BROADCAST_PORT = 8888;
    private const int TIMEOUT_MS = 3000; // 3초 대기

    public static (string ip, int port)? FindServer()
    {
        try
        {
            using (UdpClient udp = new UdpClient())
            {
                udp.EnableBroadcast = true;
                udp.Client.ReceiveTimeout = TIMEOUT_MS;

                byte[] sendData = Encoding.UTF8.GetBytes("DISCOVER_SERVER");
                IPEndPoint broadcastEP = new IPEndPoint(IPAddress.Broadcast, BROADCAST_PORT);

                // 서버 찾기 신호 송신
                udp.Send(sendData, sendData.Length, broadcastEP);
                Debug.Log("[UDP] 서버 검색 중...");

                // 응답 대기
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] recvData = udp.Receive(ref remoteEP);
                string response = Encoding.UTF8.GetString(recvData);

                if (response.StartsWith("SERVER_INFO:"))
                {
                    string[] parts = response.Split(':');
                    string ip = parts[1];
                    int port = int.Parse(parts[2]);
                    Debug.Log($"[UDP] 서버 발견 {ip}:{port}");
                    return (ip, port);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[UDP] 서버 탐색 실패: {ex.Message}");
        }

        return null;
    }
}
