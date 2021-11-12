using HtmlAgilityPack;
using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Valkyrie.Utils;

namespace Valkyrie.Model
{
    class Character
    {
        public Uri AvatarSource { get; private set; }
        public string PageUrl { get; private set; }
        public string Name { get; private set; }
        public string Birthday { get; private set; }
        public string Armor { get; private set; }
        public string Skill { get; private set; }
        public ImageSource ImageSource { get; private set; }
        public Uri[] SkillsSource { get; private set; }

        public Character(string avatarUrl, string pageUrl)
        {
            AvatarSource = new Uri(avatarUrl);
            PageUrl = pageUrl;
        }

        public async Task LoadInfoAsync()
        {
            string html = await WebHelper.LoadHtmlAsync(PageUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var root = doc.DocumentNode;

            // 女武神立绘
            var imgsrc = root.SelectSingleNode("//div[@class='big-img']");

            ImageSource = new BitmapImage(new Uri(imgsrc.SelectSingleNode(".//img").Attributes["src"].Value));

            // 女武神信息
            var info = root.SelectSingleNode(".//div[@class='valkyries-detail-bd__card']/div");

            var children = info.SelectNodes("div");

            Armor = children[0].InnerText.Trim();
            Name = children[2].InnerText.Trim();
            Birthday = children[3].InnerText.Trim();
            Skill = children[5].InnerText.Trim();

            // 技能图标
            var skills = info.SelectSingleNode(".//div[@class='skills']").SelectNodes("./div");
            SkillsSource = new Uri[]
            {
                new Uri(skills[0].SelectSingleNode(".//img[2]").Attributes["src"].Value),
                new Uri(skills[1].SelectSingleNode(".//img[2]").Attributes["src"].Value),
                new Uri(skills[2].SelectSingleNode(".//img[2]").Attributes["src"].Value),
                new Uri(skills[3].SelectSingleNode(".//img[2]").Attributes["src"].Value)
            };
        }
    }
}
