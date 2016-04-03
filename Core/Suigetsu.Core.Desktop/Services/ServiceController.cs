using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using NLog;
using Suigetsu.Core.Common;
using Suigetsu.Core.Desktop.Common;
using Suigetsu.Core.Desktop.Net;
using Suigetsu.Core.Logging;

namespace Suigetsu.Core.Desktop.Services
{
    //TODO rethink interaction between servicecontroller and servicewrapper
    [ExcludeFromCodeCoverage]
    public class ServiceController
    {
        private static readonly Logger Logger = LoggingManager.GetCurrentClassLogger();
        private readonly ServiceWrapper _serviceWrapper;
        public readonly Dictionary<string, ServiceAction> Actions;

        private ServiceController(IEnumerable<string> args, string name)
        {
            Name = name;
            DisplayName = Name;

            _serviceWrapper = new ServiceWrapper(this)
            {
                ServiceName = Name
            };

            Args = ArgSegments.Parse(args);

            Actions = new Dictionary<string, ServiceAction>
            {
                {
                    "status", new ServiceAction
                    {
                        Description = "Show Status",
                        Visible = true,
                        Action =
                            () =>
                            Dialogs.Info
                                (String.Format
                                     ((string)"Service '{0}' status: {1}. Installed: {2}.",
                                      (object)Name,
                                      (object)Services.GetServiceStatus(Name),
                                      Services.ServiceIsInstalled(Name)))
                    }
                },
                {
                    "install", new ServiceAction
                    {
                        Description = "Install",
                        Visible = true,
                        Action = () =>
                        {
                            Services.Install(Name, DisplayName, $"{Assembly.GetEntryAssembly().Location} -runasservice", Description);
                            Dialogs.Info($"Service '{Name}' installed successfully.");
                        }
                    }
                },
                {
                    "start", new ServiceAction
                    {
                        Description = "Start",
                        Visible = true,
                        Action = () =>
                        {
                            Services.StartService(Name);
                            Dialogs.Info($"Service '{Name}' started successfully.");
                        }
                    }
                },
                {
                    "stop", new ServiceAction
                    {
                        Description = "Stop",
                        Visible = true,
                        Action = () =>
                        {
                            Services.StopService(Name);
                            Dialogs.Info($"Service '{Name}' stopped successfully.");
                        }
                    }
                },
                {
                    "uninstall", new ServiceAction
                    {
                        Description = "Uninstall",
                        Visible = true,
                        Action = () =>
                        {
                            Services.Uninstall(Name);
                            Dialogs.Info($"Service '{Name}' uninstalled successfully.");
                        }
                    }
                },
                {
                    "runasservice", new ServiceAction
                    {
                        Description = "Run As Service",
                        Visible = false,
                        Action = () => ServiceBase.Run(_serviceWrapper)
                    }
                }
            };
        }

        public ServiceController(IEnumerable<string> args) : this(args, Assembly.GetEntryAssembly().GetName().Name) {}

        private Dictionary<string, string> Args { get; }
        public string Name { get; }
        public string DisplayName { private get; set; }
        public string Description { private get; set; }
        public Action<ServiceController> OnGuiStart { private get; set; }
        public Action<ServiceController> OnServiceStart { get; set; }
        public RemoteInvokeServer RemoteInvoke { get; set; }
        public Action<ServiceController> OnServiceStop { get; set; }

        public void CreateTimer(ServiceTimerParameters parameters)
        {
            _serviceWrapper.CreateTimer(parameters);
        }

        private void ShowMenu()
        {
            var form = new ServiceOptions(this);
            try
            {
                form.ShowDialog();
            }
            catch(Exception e)
            {
                Dialogs.Error(e.Message);
            }
        }

        public void Start()
        {
            try
            {
                foreach(var v in Actions.Where(v => Args.ContainsKey(v.Key)))
                {
                    v.Value.Action();
                    Environment.Exit(0);
                }
            }
            catch(Services.ServiceException e)
            {
                Dialogs.Error(e.Message);
                Environment.Exit(1);
            }

            if(OnGuiStart != null)
            {
                OnGuiStart(this);
            }
            else
            {
                ShowMenu();
            }
        }

        public class MailErrorReporting {}

        public class ServiceTimerParameters
        {
            public bool ErrorReporting;

            public Predicate<DateTime> Validate;

            public ServiceTimerParameters(int interval, Action action)
            {
                Interval = interval;
                Action = action;
            }

            public int Interval { get; }
            public Action Action { get; }
        }

        public class ServiceAction
        {
            public Action Action;
            public string Description;
            public bool Visible;
        }
    }
}
