namespace MovieStorage.UI.Dialogs;

public interface IMessageDialogService
{
    MessageDialogResult ShowYesNoDialog(string title, string message);
}