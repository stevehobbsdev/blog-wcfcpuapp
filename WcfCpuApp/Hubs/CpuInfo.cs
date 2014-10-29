using SignalR.Hubs;

namespace WcfCpuApp.Hubs
{
    public class CpuInfo : Hub
    {
        public void SendCpuInfo(string machineName, double processor, int memUsage, int totalMemory)
        {
            this.Clients.cpuInfoMessage(machineName, processor, memUsage, totalMemory);
        }
    }
}