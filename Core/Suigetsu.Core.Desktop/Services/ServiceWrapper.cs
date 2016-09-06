using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.ServiceProcess;
using System.Threading;
using NLog;
using Suigetsu.Core.Logging;
using Timer = System.Timers.Timer;

namespace Suigetsu.Core.Desktop.Services
{
    [ExcludeFromCodeCoverage]
    public class ServiceWrapper : ServiceBase
    {
        private static readonly Logger Logger = LoggingManager.GetCurrentClassLogger();

        private readonly ServiceController _serviceController;

        private readonly List<Timer> _timerList = new List<Timer>();

        public ServiceWrapper(ServiceController serviceController)
        {
            _serviceController = serviceController;
        }

        public void CreateTimer(ServiceController.ServiceTimerParameters parameters)
        {
            if(!CanStop)
            {
                return;
            }

            //TODO: make ticks run in another thread //System.Threading.Thread.CurrentThread.ManagedThreadId

            lock(_timerList)
            {
                var timer = new Timer(parameters.Interval);
                _timerList.Add(timer);

                var errorHandlingAction = new Action
                    (() =>
                     {
                         try
                         {
                             if(parameters.Validate != null && !parameters.Validate(DateTime.Now))
                             {
                                 return;
                             }

                             parameters.Action();
                         }
                         catch(Exception e)
                         {
                             Logger.Error(e);
                             if(parameters.ErrorReporting)
                             {
                                 //TODO make error reporting class with += support, mail + webservice
                                 /*
                                 try
                                 {
 
                                     var client = new SmtpClient
                                     {
                                         Port = ?,
                                         DeliveryMethod = SmtpDeliveryMethod.Network,
                                         UseDefaultCredentials = false,
                                         Host = "?",
                                         Credentials = new NetworkCredential("?", "?")
                                     };
                                     var mail = new MailMessage("?", "?")
                                     {
                                         Subject = $"Service Exception: {AssemblyUtils.GetEntryAssembly().GetName().Name}",
                                         Body = e.ToString()
                                     };
                                     mail.CC.Add("?");
                                     
                                     client.Send(mail);
 
                                 }
                                     catch (Exception)
                                     {
                                         Logger.Error(e);
                                     }
                                 }
                          */
                             }
                         }
                     });

                timer.Elapsed += (sender, args) =>
                {
                    Logger.Trace("Timer {0} tick: {1}", sender.GetHashCode(), args.SignalTime);
                    errorHandlingAction();
                };
                errorHandlingAction();
                timer.Enabled = true;
            }
        }

        protected override void OnStart(string[] args)
        {
            if(_serviceController.OnServiceStart == null && _serviceController.RemoteInvoke == null)
            {
                return;
            }

            new Thread(() =>
                       {
                           _serviceController.RemoteInvoke?.Start();

                           if(_serviceController.OnServiceStart != null)
                           {
                               try
                               {
                                   Logger.Trace("OnServiceStart");
                                   _serviceController.OnServiceStart(_serviceController);
                               }
                               catch(Exception e)
                               {
                                   Logger.Error("Unhandled error on the service '{0}': {1}.", _serviceController.Name, e);
                               }
                           }
                       })
            {
                IsBackground = true
            }.Start();
        }

        protected override void OnStop()
        {
            foreach(var v in _timerList)
            {
                v.Enabled = false;
            }

            _serviceController.RemoteInvoke?.Stop();

            if(_serviceController.OnServiceStop != null)
            {
                Logger.Trace("OnServiceStop");
                _serviceController.OnServiceStop(_serviceController);
            }
        }
    }
}
