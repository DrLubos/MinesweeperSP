using Microsoft.Maui.Controls.Platform;
using Microsoft.UI.Xaml.Input;
using Microsoft.Maui.Controls;
using System.Linq;

[assembly: ResolutionGroupName("Minesweeper.MAUIApp")]
[assembly: ExportEffect(typeof(Minesweeper.MAUIApp.Platforms.Windows.RightClickEffect), "RightClickEffect")]

namespace Minesweeper.MAUIApp.Platforms.Windows
{
    public class RightClickEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Control is Microsoft.UI.Xaml.Controls.Button button)
            {
                button.RightTapped += OnRightTapped;
            }
        }

        protected override void OnDetached()
        {
            if (Control is Microsoft.UI.Xaml.Controls.Button button)
            {
                button.RightTapped -= OnRightTapped;
            }
        }

        private void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (Element is Button button && button.Effects.FirstOrDefault(eff => eff is Minesweeper.MAUIApp.Effects.RightClickEffect) is Minesweeper.MAUIApp.Effects.RightClickEffect effect)
            {
                effect.OnRightClicked();
            }
        }
    }
}