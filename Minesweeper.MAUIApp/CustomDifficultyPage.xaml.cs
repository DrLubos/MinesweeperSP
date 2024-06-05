
using Label = Microsoft.Maui.Controls.Label;

namespace Minesweeper.MAUIApp;

public class CustomDifficultyPage : ContentPage
{
    private readonly Entry _entryRow = new() { Keyboard = Keyboard.Numeric };
    private readonly Entry _entryCols = new() { Keyboard = Keyboard.Numeric };
    private readonly Entry _entryMines = new() { Keyboard = Keyboard.Numeric };

    public event EventHandler<CustomDifficultyEventArgs>? DifficultySelected;

    public CustomDifficultyPage()
    {
        CreateLayout();
    }

    private void CreateLayout()
    {
        var stackLayout = new StackLayout
        {
            Padding = new Thickness(20),
            Spacing = 10
        };
        var labelRow = new Label
        {
            Text = "Rows:"
        };
        var labelCols = new Label
        {
            Text = "Columns:"
        };
        var labelMines = new Label
        {
            Text = "Mines:"
        };
        var button = new Button
        {
            Text = "Apply"
        };
        button.Clicked += OnApplyClick;
        var cancel = new Button
        {
            Text = "Cancel"
        };
        cancel.Clicked += (_, _) => Navigation.PopAsync();
        stackLayout.Children.Add(labelRow);
        stackLayout.Children.Add(_entryRow);
        stackLayout.Children.Add(labelCols);
        stackLayout.Children.Add(_entryCols);
        stackLayout.Children.Add(labelMines);
        stackLayout.Children.Add(_entryMines);
        stackLayout.Children.Add(button);
        stackLayout.Children.Add(cancel);
        Content = stackLayout;
    }

    private void OnApplyClick(object? sender, EventArgs? e)
    {
        if (int.TryParse(_entryRow.Text, out int rows) &&
            int.TryParse(_entryCols.Text, out int cols) &&
            int.TryParse(_entryMines.Text, out int mines))
        {
            if (rows < 1 || cols < 1 || mines < 1)
            {
                Application.Current?.MainPage?.DisplayAlert("Error", "All values must be greater than 0", "OK");
                return;
            }

            if (mines >= rows * cols)
            {
                Application.Current?.MainPage?.DisplayAlert("Error", "Number of mines must be less than the number of tiles", "OK");
                return;
            }
            DifficultySelected?.Invoke(this, new CustomDifficultyEventArgs(rows, cols, mines));
            Navigation.PopAsync();
        }
        else
        {
            Application.Current?.MainPage?.DisplayAlert("Error", "Invalid input", "OK");
        }
    }
}