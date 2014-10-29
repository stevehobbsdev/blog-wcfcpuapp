using WcfCpuApp.Hubs;

namespace WcfCpuApp.Services
{
    public class CpuService : ICpuService
    {
        public void SendCpuReport(string machineName, double processor, ulong memUsage, ulong totalMemory)
        {
            var context = SignalR.GlobalHost.ConnectionManager.GetHubContext<CpuInfo>();
            context.Clients.cpuInfoMessage(machineName, processor, memUsage, totalMemory);
        }
    }
}
