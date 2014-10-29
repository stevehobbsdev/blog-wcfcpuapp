using System.Web.Http;
using Microsoft.AspNet.SignalR;
using WcfCpuApp.Hubs;
using WcfCpuApp.Models;

namespace WcfCpuApp.Api
{
    public class CpuInfoController : ApiController
    {
        public void Post(CpuInfoPostData cpuInfo)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<CpuInfo>();
            context.Clients.All.cpuInfoMessage(cpuInfo.MachineName, cpuInfo.Processor, cpuInfo.MemUsage, cpuInfo.TotalMemory);
        }
    }
}
