using Microsoft.AspNet.SignalR;

namespace WcfCpuApp.Hubs
{
    public class CpuInfo : Hub
    {
        public void SendCpuInfo(string machineName, double processor, int memUsage, int totalMemory)
        {
            this.Clients.All.cpuInfoMessage(machineName, processor, memUsage, totalMemory);
        }
    }
}