using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HtmlAgilityPack;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace Valkyrie
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DetailPanel.Visibility = Visibility.Collapsed;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var html = await WebHelper.LoadHtml("https://www.bh3.com/valkyries");

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var body = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='valkyries-item']");

            var avatars = body.SelectNodes("//a").Where(x => x.Attributes["href"].Value.Contains("valkyries"));

            int index = 0;
            foreach (var a in avatars)
            {
                var imgsrc = a.SelectSingleNode(".//img").Attributes["src"].Value;
                var link = a.Attributes["href"].Value;

                // 将图片先添加到 border 里，便于修改 margin 且不会变动图片的中心位置
                Border border = new Border
                {
                    Background = Brushes.Transparent,
                    Height = 100,
                    Width = 100
                };

                Image image = new Image();
                image.Source = new BitmapImage(new Uri(imgsrc));
                image.Margin = new Thickness(0);
                image.Tag = "https://www.bh3.com" + link;
                image.MouseUp += Avatar_Click;
                
                // 修改图片的光标样式
                image.Cursor = Cursors.Hand;

                image.Name = "image" + index;
                this.RegisterName(image.Name, image);

                ThicknessAnimation zoomin = new ThicknessAnimation
                {
                    From = new Thickness(0),
                    To = new Thickness(-10),
                    Duration = new Duration(TimeSpan.FromSeconds(.1))
                };
                ThicknessAnimation zoomout = new ThicknessAnimation
                {
                    From = new Thickness(-10),
                    To = new Thickness(0),
                    Duration = new Duration(TimeSpan.FromSeconds(.1))
                };

                Storyboard.SetTargetName(zoomin, image.Name);
                Storyboard.SetTargetProperty(zoomin, new PropertyPath(Image.MarginProperty));
                Storyboard.SetTargetName(zoomout, image.Name);
                Storyboard.SetTargetProperty(zoomout, new PropertyPath(Image.MarginProperty));

                Storyboard zoominStoryboard = new Storyboard();
                zoominStoryboard.Children.Add(zoomin);
                Storyboard zoomoutStoryboard = new Storyboard();
                zoomoutStoryboard.Children.Add(zoomout);

                image.MouseEnter += (object o, MouseEventArgs args) => zoominStoryboard.Begin(image);
                image.MouseLeave += (object o, MouseEventArgs args) => zoomoutStoryboard.Begin(image);

                border.Child = image;
                ValkyriesAvatarBox.Children.Add(border);

                index++;
            }
        }

        private async void Avatar_Click(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;

            var html = await WebHelper.LoadHtml((string)image.Tag);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var root = doc.DocumentNode;

            // 显示女武神立绘
            var imgsrc = root.SelectSingleNode("//div[@class='big-img']");

            DetailPanel.Visibility = Visibility.Visible;
            ValkyrieTachie.Source = new BitmapImage(new Uri(imgsrc.SelectSingleNode("//img").Attributes["src"].Value));

            // 显示女武神信息
            var info = root.SelectSingleNode("//div[@class='valkyries-detail-bd__card']/div");
            Debug.WriteLine(info);

            var children = info.SelectNodes("div");

            ValkyrieName.Content = children[0].InnerText.Trim();
            VName.Text = children[2].InnerText.Trim();
            VAge.Text = children[3].InnerText.Trim();
            VArmor.Text = children[4].InnerText.Trim();
            VSkill.Text = children[5].InnerText.Trim();

            // 显示技能图标
            var skills = info.SelectSingleNode(".//div[@class='skills']").SelectNodes("./div");
            Skill1.Source = new BitmapImage(new Uri(skills[0].SelectSingleNode(".//img[2]").Attributes["src"].Value));
            Skill2.Source = new BitmapImage(new Uri(skills[1].SelectSingleNode(".//img[2]").Attributes["src"].Value));
            Skill3.Source = new BitmapImage(new Uri(skills[2].SelectSingleNode(".//img[2]").Attributes["src"].Value));
            Skill4.Source = new BitmapImage(new Uri(skills[3].SelectSingleNode(".//img[2]").Attributes["src"].Value));
        }

        private void DetailPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DetailPanel.Visibility = Visibility.Collapsed;
        }
    }
}
