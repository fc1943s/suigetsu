using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
//using NLog;

namespace Suigetsu.Core.Desktop.Common
{
    [ExcludeFromCodeCoverage]
    public static class Dialogs
    {
        //private static readonly Logger Logger = Logging.Logger.GetCurrentClassLogger();

        public static void Info(string text, string title = "Informação")
        {
           // Logger.Info("Information dialog: '{0}'", text);
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Error(string text, string title = "Erro")
        {
         //   Logger.Info("Error dialog: '{0}'", text);
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
