using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using NLog;
using Suigetsu.Core.Logging;

namespace Suigetsu.Core.Desktop.Common
{
    /// <summary>
    ///     Shortcuts to <see cref="T:System.Windows.Forms.MessageBox" /> dialogs.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Dialogs
    {
        //TODO: When not informed, the dialog titles must be translated to the system language.

        private static readonly Logger Logger = LoggingManager.GetCurrentClassLogger();

        /// <summary>
        ///     Shows an information dialog.
        /// </summary>
        public static void Info(string text, string title = "Information")
        {
            Logger.Info("Information dialog: '{0}'", text);
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        ///     Shows an error dialog.
        /// </summary>
        public static void Error(string text, string title = "Error")
        {
            Logger.Info("Error dialog: '{0}'", text);
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
