using HtmlAgilityPack;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Valkyrie.Model;
using Valkyrie.Utils;
using Forms = System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Valkyrie.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Character> Characters { get; private set; } = new ObservableCollection<Character>();
        public ICommand AvatarBoxClick { get; private set; }
        public ICommand ViewLargeImage { get; private set; }
        public ICommand HideLargeImage { get; private set; }
        public ICommand SaveLargeImage { get; private set; }
        public ICommand HideSidePanel { get; private set; }
        public ICommand Loaded { get; private set; }
        public ICommand ViewPage { get; private set; }

        public SidePanelViewModel SidePanelViewModel { get; set; } = new SidePanelViewModel();

        public Visibility LargeImageVisibility { get; private set; } = Visibility.Hidden;

        public MainWindowViewModel()
        {
            AvatarBoxClick = new RelayParamCommand(async (obj) =>
            {
                var character = obj as Character;
                if (character.ImageSource is null)
                    await character?.LoadInfoAsync();
                SidePanelViewModel.SelectedCharacter = character;
            });

            ViewLargeImage = new RelayCommand(() => LargeImageVisibility = Visibility.Visible);
            HideLargeImage = new RelayCommand(() => LargeImageVisibility = Visibility.Hidden);

            SaveLargeImage = new RelayParamCommand(async (obj) =>
            {
                using (var open = new Forms.SaveFileDialog())
                {
                    open.Filter = "PNG|*.png";
                    open.FileName = SidePanelViewModel.SelectedCharacter.Name + ".png";
                    if (open.ShowDialog() == Forms.DialogResult.OK)
                    {
                        var path = open.FileName;
                        var bitmap = (obj as Image)?.Source as BitmapImage;
                        if (bitmap is null)
                            bitmap = SidePanelViewModel.SelectedCharacter.ImageSource as BitmapImage;
                        var encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmap));
                        using (var stream = File.Create(path))
                        {
                            encoder.Save(stream);
                        }
                    }
                }
            });

            HideSidePanel = new RelayCommand(() => SidePanelViewModel.SelectedCharacter = null);

            Loaded = new RelayCommand(async () => await LoadCharactersAsync());

            ViewPage = new RelayCommand(() => Process.Start(SidePanelViewModel.SelectedCharacter.PageUrl));

            SidePanelViewModel.ViewLargeImage = ViewLargeImage;
            SidePanelViewModel.HideLargeImage = HideLargeImage;
            SidePanelViewModel.HideSidePanel = HideSidePanel;
            SidePanelViewModel.SaveLargeImage = SaveLargeImage;
            SidePanelViewModel.ViewPage = ViewPage;
        }

        public async Task LoadCharactersAsync()
        {
            var html = await WebHelper.LoadHtmlAsync("https://www.bh3.com/valkyries");

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var body = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='valkyries-item']");

            var avatars = body.SelectNodes("//a")
                .Where(x => x.Attributes["href"].Value.Contains("valkyries"));

            foreach (var a in avatars)
            {
                var imgsrc = a.SelectSingleNode(".//img").Attributes["src"].Value;
                var link = @"https://www.bh3.com" + a.Attributes["href"].Value;

                Characters.Add(new Character(imgsrc, link));
            }
        }
    }
}
