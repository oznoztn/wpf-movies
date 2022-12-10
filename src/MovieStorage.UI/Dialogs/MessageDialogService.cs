using System.Windows;

namespace MovieStorage.UI.Dialogs;

public class MessageDialogService : IMessageDialogService
{
    public MessageDialogResult ShowYesNoDialog(string title, string message)
    {
        return new YesNoDialog(title, message)
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow
        }.ShowDialog().GetValueOrDefault()
            ? MessageDialogResult.Yes
            : MessageDialogResult.No;
    }
}