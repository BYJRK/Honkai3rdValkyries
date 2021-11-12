using System.Windows.Input;
using Valkyrie.Model;

namespace Valkyrie.ViewModel
{
    class SidePanelViewModel : ViewModelBase
    {
        public ICommand ViewLargeImage { get; set; }
        public ICommand HideLargeImage { get; set; }
        public ICommand HideSidePanel { get; set; }
        public ICommand SaveLargeImage { get; set; }
        public ICommand ViewPage { get; set; }
        public Character SelectedCharacter { get; set; }
    }
}
