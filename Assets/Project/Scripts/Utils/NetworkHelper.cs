using System.Net.NetworkInformation;
using Ping = System.Net.NetworkInformation.Ping;

public static class NetworkHelper
{
    /// <summary>
    /// ���� ��������� ����������� �������. 
    /// � ����� ������� �� �������, ����� ��� �������������
    /// ���� �� ����� ������� ���������� ��� webgl
    /// </summary>
    public static bool PingHostAvailable(string nameOrAddress, out long pingTime)
    {
        bool pingable = false;
        Ping pinger = null;
        pingTime = 0;

        try
        {
            pinger = new Ping();
            PingReply reply = pinger.Send(nameOrAddress);

            pingTime = reply.RoundtripTime;

            pingable = reply.Status == IPStatus.Success;
        }
        catch (PingException)
        {
            // Discard PingExceptions and return false;
        }
        finally
        {
            if (pinger != null)
            {
                pinger.Dispose();
            }
        }

        return pingable;
    }   
}