using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HtmlAgilityPack;
using System.Diagnostics;

namespace Valkyria
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

            foreach (var a in avatars)
            {
                var imgsrc = a.SelectSingleNode(".//img").Attributes["src"].Value;
                var link = a.Attributes["href"].Value;

                Image image = new Image();
                image.Source = new BitmapImage(new Uri(imgsrc));
                image.Height = 100;
                image.Margin = new Thickness(0, 10, 0, 10);
                image.Tag = "https://www.bh3.com" + link;
                image.MouseUp += Avatar_Click;
                ValkyriesAvatarBox.Children.Add(image);
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
            ValkyriaTachie.Source = new BitmapImage(new Uri(imgsrc.SelectSingleNode("//img").Attributes["src"].Value));

            // 显示女武神信息
            var info = root.SelectSingleNode("//div[@class='valkyries-detail-bd__card']/div");
            Debug.WriteLine(info);

            var children = info.SelectNodes("div");

            ValkyriaName.Content = children[0].InnerText.Trim();
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
