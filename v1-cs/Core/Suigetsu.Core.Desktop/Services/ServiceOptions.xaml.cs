using System.Linq;
using System.Windows.Controls;

namespace Suigetsu.Core.Desktop.Services
{
    public partial class ServiceOptions
    {
        public ServiceOptions(ServiceController serviceController)
        {
            InitializeComponent();

            foreach(var v in serviceController.Actions.Where(x => x.Value.Visible))
            {
                var action = v;

                var button = new Button
                {
                    Content = v.Value.Description
                };

                button.Click += (sender, args) => action.Value.Action();
                StackPanel1.Children.Add(button);
            }
        }
    }
}
