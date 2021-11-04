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
using Valkyrie.Utils;
using System.IO;
using Forms = System.Windows.Forms;

namespace Valkyrie
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DetailPanel.Visibility = Visibility.Collapsed;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var html = await WebHelper.LoadHtmlAsync("https://www.bh3.com/valkyries").ConfigureAwait(true);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var body = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='valkyries-item']");

            var avatars = body.SelectNodes("//a")
                .Where(x => x.Attributes["href"].Value.Contains("valkyries"));

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

                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(imgsrc)),
                    Tag = "https://www.bh3.com" + link,
                    Name = "image" + index
                };
                //this.RegisterName(image.Name, image);

                border.Child = image;
                ValkyriesAvatarBox.Children.Add(border);

                index++;
            }
        }

        private async void Avatar_Click(object sender, MouseButtonEventArgs e)
        {
            var image = sender as Image;

            var html = await WebHelper.LoadHtmlAsync((string)image.Tag).ConfigureAwait(true);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var root = doc.DocumentNode;

            // 显示女武神立绘
            var imgsrc = root.SelectSingleNode("//div[@class='big-img']");

            DetailPanel.Visibility = Visibility.Visible;
            ValkyrieProtrait.Source = new BitmapImage(new Uri(imgsrc.SelectSingleNode(".//img").Attributes["src"].Value));

            // 显示女武神信息
            var info = root.SelectSingleNode(".//div[@class='valkyries-detail-bd__card']/div");
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
            if (e.ChangedButton == MouseButton.Left)
            {
                DetailPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            using (var open = new Forms.SaveFileDialog())
            {
                open.Filter = "PNG|*.png";
                open.FileName = ValkyrieName.Content.ToString() + ".png";
                if (open.ShowDialog() == Forms.DialogResult.OK)
                {
                    var path = open.FileName;
                    var bitmap = ValkyrieProtrait.Source as BitmapImage;
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    using (var stream = File.Create(path))
                    {
                        encoder.Save(stream);
                    }
                }
            }
        }
    }
}
