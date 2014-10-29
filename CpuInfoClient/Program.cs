using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpuInfoClient
{
	class Program
	{
		static bool _running = true;
		static PerformanceCounter _cpuCounter, _memUsageCounter;

		static void Main(string[] args)
		{
			// Required for a self-signed certificate
			System.Net.ServicePointManager.ServerCertificateValidationCallback =
				((sender, certificate, chain, sslPolicyErrors) => true);

			Thread pollingThread = null;
			CpuInfoService.CpuServiceClient serviceClient = null;

			// Hello!
			Console.WriteLine("CPU Info Client: Reporting your CPU usage today!");

			try
			{
				_cpuCounter = new PerformanceCounter();
				_cpuCounter.CategoryName = "Processor";
				_cpuCounter.CounterName = "% Processor Time";
				_cpuCounter.InstanceName = "_Total";

				_memUsageCounter = new PerformanceCounter("Memory", "Available KBytes");

				// Create the service client
				serviceClient = new CpuInfoService.CpuServiceClient();

				// Create a new thread to start polling and sending the data
				pollingThread = new Thread(new ParameterizedThreadStart(RunPollingThread));
				pollingThread.Start(serviceClient);

				Console.WriteLine("Press a key to stop and exit");
				Console.ReadKey();

				Console.WriteLine("Stopping thread..");

				_running = false;

				pollingThread.Join(5000);
				serviceClient.Close();

			}
			catch (Exception)
			{
				pollingThread.Abort();
				serviceClient.Abort();

				throw;
			}
		}

		static void RunPollingThread(object serviceClient)
		{
			// Convert the object that was passed in
			var svc = serviceClient as CpuInfoService.CpuServiceClient;
			DateTime lastPollTime = DateTime.MinValue;

			Console.WriteLine("Started polling...");

			// Start the polling loop
			while (_running)
			{
				// Poll every second
				if ((DateTime.Now - lastPollTime).TotalMilliseconds >= 1000)
				{
					double cpuTime;
					ulong memUsage, totalMemory;

					// Get the stuff we need to send
					GetMetrics(out cpuTime, out memUsage, out totalMemory);

					// Send the data
					svc.SendCpuReport(System.Environment.MachineName, cpuTime, memUsage, totalMemory);

					// Reset the poll time
					lastPollTime = DateTime.Now;
				}
				else
				{
					Thread.Sleep(10);
				}
			}
		}

		static void GetMetrics(out double processorTime, out ulong memUsage, out ulong totalMemory)
		{
			processorTime = (double)_cpuCounter.NextValue();
			memUsage = (ulong)_memUsageCounter.NextValue();
			totalMemory = 0;

			// Get total memory from WMI
			ObjectQuery memQuery = new ObjectQuery("SELECT * FROM CIM_OperatingSystem");

			ManagementObjectSearcher searcher = new ManagementObjectSearcher(memQuery);

			foreach (ManagementObject item in searcher.Get())
			{
				totalMemory = (ulong)item["TotalVisibleMemorySize"];
			}
		}
	}
}
