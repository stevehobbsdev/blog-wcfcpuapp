using System.ServiceModel;

namespace WcfCpuApp.Services
{
    [ServiceContract]
    public interface ICpuService
    {
        [OperationContract]
        void SendCpuReport(string machineName, double processor, ulong memUsage, ulong totalMemory);
    }
}
