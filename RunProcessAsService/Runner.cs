using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

namespace RunProcessAsService
{
    public partial class Runner : ServiceBase
    {
        private Process process;
        private string logFilePath;

        public Runner()
        {
            logFilePath = ConfigurationManager.AppSettings["logFilePath"];
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var info = new ProcessStartInfo(ConfigurationManager.AppSettings["execPath"], 
                ConfigurationManager.AppSettings["execArguments"]);
            info.CreateNoWindow = true;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;

            process = new Process();
            process.StartInfo = info;
            try
            {
                process.Start();

                Debug(string.Format("Process  running:{0}", !process.HasExited));
                Debug(string.Format("Exec path:{0}", ConfigurationManager.AppSettings["execPath"]));
                Debug(string.Format("Exec argu:{0}", ConfigurationManager.AppSettings["execArguments"]));
                Debug(string.Format("Process id: {0}", process.Id));
            }
            catch (Exception e)
            {
                Debug(string.Format("error occured when starting process:{0}", e));
            }
        }


        protected override void OnStop()
        {
            Debug("service stoped.");
            try
            {
                if (!process.HasExited)
                {
                    Debug("process stoping...");


                    process.Kill();


                    process = null;
                    Debug("process stopped");
                }
            }

            catch (Exception e)
            {
                Debug(string.Format("error occured when stopping process:{0}", e));
            }
        }

        private void Debug(string message)
        {
            if(string.IsNullOrEmpty(logFilePath)) return;
            try
            {
                File.AppendAllText(logFilePath, string.Format("{0} {1} \n", DateTime.Now, message));
            }
            catch (Exception e)
            {
                throw new ApplicationException(string.Format("cannot write log to file: {0}", logFilePath));
            }
        }
    }
}