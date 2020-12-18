using System;
using System.Windows.Input;
using Scroller.ViewModels;

namespace Scroller.Commands
{
    public class MuteCommand : ICommand
    {
        private readonly MainWindowViewModel m_ViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MuteCommand"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public MuteCommand(MainWindowViewModel viewModel)
        {
            m_ViewModel = viewModel;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            m_ViewModel.HandleMuteCommand((bool)parameter);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
