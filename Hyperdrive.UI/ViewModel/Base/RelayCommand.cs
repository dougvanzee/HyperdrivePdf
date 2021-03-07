using System;
using System.Windows.Input;

namespace Hyperdrive.UI
{
    /// <summary>
    /// A basic command that runs an Action
    /// </summary>
    class RelayCommand : ICommand
    {
        #region Private Members

        // The action to run
        private Action mAction;

        #endregion

        #region Public Events

        /// <summary>
        /// The event that's fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        public RelayCommand(Action action)
        {
            mAction = action;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// A relay command that can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter) { return true; }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter) { mAction(); }

        #endregion
    }
}
