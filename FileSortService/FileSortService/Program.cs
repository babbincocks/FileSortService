using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace FileSortService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {

            if (Environment.UserInteractive)
            {
                FileSortService newService = new FileSortService();
                newService.TestStartandStop(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new FileSortService()
                };
                ServiceBase.Run(ServicesToRun);
            }
            
        }
    }
}
