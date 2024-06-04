using Microsoft.Maui.Controls;
using System;

namespace Minesweeper.MAUIApp.Effects
{
    public class RightClickEffect : RoutingEffect
    {
        public event EventHandler RightClicked;

        public RightClickEffect() : base("Minesweeper.MAUIApp.RightClickEffect")
        {
        }

        public void OnRightClicked()
        {
            RightClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}