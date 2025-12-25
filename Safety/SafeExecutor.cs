using BookOrg.Debugging;
using System.Windows;

namespace LIMS.Safety
{
    /// <include file='../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="SafeExecutor"]/*'/>
    public static class SafeExecutor
    {
        /// <summary>
        /// Safely executes the specified action.
        /// </summary>
        /// <param name="action">The action to be executed (in the try block).</param>
        /// <param name="errorMessage">The error message to be displayed if the action fails.</param>
        /// <param name="finallyAction">The action to be executed in the end (in the finally block).</param>
        public static void Execute(Action action, string errorMessage, Action? finallyAction = null)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                HandleError(ex, errorMessage);
            }
            finally
            {
                finallyAction?.Invoke();
            }
        }

        /// <summary>
        /// Safely executes an asynchronous action.
        /// </summary>
        /// <param name="action">The action to be executed (in the try block).</param>
        /// <param name="errorMessage">The error message to be displayed if the action fails.</param>
        /// <param name="finallyAction">The action to be executed in the end (in the finally block).</param>
        public static async Task ExecuteAsync(Func<Task> action, string errorMessage, Func<Task>? finallyAction = null)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                HandleError(ex, errorMessage);
            }
            finally
            {
                if (finallyAction != null) await finallyAction();
            }
        }

        /// <summary>
        /// Shows the error to the user and logs it.
        /// </summary>
        /// <param name="ex">The exception to be logged.</param>
        /// <param name="errorMessage">The error message to be displayed (and logged).</param>
        private static void HandleError(Exception ex, string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Logger.Error($"{errorMessage}: {ex.Message}");
        }
    }
}
