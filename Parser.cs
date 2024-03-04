using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using System.ComponentModel;



namespace course
{
    class Parser
    {

        public async Task<BindingList<partsCategory>> ParseCategories(string url)
        {

            List<string> categoriesUrl = await getCategoriesUrl(url);
            BindingList<partsCategory> output = new BindingList<partsCategory>();
            List<string> pics = await getPicsUrls(url);
            foreach (string u in categoriesUrl)
            {
                output.Add(await getCategoryInfo(u, pics.FirstOrDefault()));
                pics.RemoveAt(0);
            }
            pics.Clear();

            return output;
        }

        public async Task<BindingList<part>> ParseParts(string url)
        {
            List<string> partsUrl = await getPartsUrl(url);
            BindingList<part> output = new BindingList<part>();
            List<string> pics = await getPartsPics(url);

            foreach(string u in partsUrl)
            {
                output.Add(await getPartInfo(u, pics.FirstOrDefault()));
                pics.RemoveAt(0);
            }
            pics.Clear();
            return output;
        }

        private async Task<part> getPartInfo(string url, string iUrl) 
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();

            IBrowsingContext context = BrowsingContext.New(config);

            IDocument doc = await context.OpenAsync(url);

            IElement name = doc.QuerySelector("h1");
            IElement price = doc.QuerySelector("div.prices_block");
            IElement availability = doc.QuerySelector("div.item-stock");
            IElement desc = doc.QuerySelector("div.detail_text");


            part p = new part();
            p.Name = name.TextContent;

            p.Price = string.Join("", price.TextContent.Trim().Where(c => char.IsDigit(c)));
            if (p.Price.Length > 6)
                p.Price = "Не удалось получить цену";

            p.Availability = availability.TextContent;
            if (desc != null)
                p.Description = desc.TextContent;
            else
                p.Description = "";

            p.DateAdded = DateTime.Now;

            p.ImageUrl = iUrl;
            return p;
        }

        private async Task<partsCategory> getCategoryInfo(string url, string iUrl) 
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();

            IBrowsingContext context = BrowsingContext.New(config);

            IDocument doc = await context.OpenAsync(url);

            IElement name = doc.QuerySelector("h1");


            partsCategory p = new partsCategory
            {
                Name = name.TextContent,
                Url = url,
                ImageUrl = iUrl
            };
            return p;
        }


        private async Task<List<string>> getPartsUrl(string url)
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();

            IBrowsingContext context = BrowsingContext.New(config);

            IDocument doc = await context.OpenAsync(url);

            IEnumerable<IElement> aElements = doc.All.Where(block =>
            block.LocalName == "a"
            && block.ParentElement.LocalName == "div"
            && block.ParentElement.ClassList.Contains("item-title"));

            List<string> output = new List<string>();
            foreach (IElement a in aElements.ToList())
                output.Add($"https://toybike.ru{a.GetAttribute("href")}");
            return output;
        }
    

        private async Task<List<string>> getCategoriesUrl(string url)
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();

            IBrowsingContext context = BrowsingContext.New(config);

            IDocument doc = await context.OpenAsync(url);

            IEnumerable<IElement> aElements = doc.All.Where(block =>
            block.LocalName == "a"
            && block.ParentElement.LocalName == "div"
            && block.ParentElement.ClassList.Contains("name"));

            List<string> output = new List<string>();
            foreach (IElement a in aElements.ToList())
                output.Add($"https://toybike.ru{a.GetAttribute("href")}");
            return output;
        }

        private async Task<List<string>> getPicsUrls(string url)
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();

            IBrowsingContext context = BrowsingContext.New(config);

            IDocument doc = await context.OpenAsync(url);


            IEnumerable<IElement> pics = doc.All.Where(block =>
            block.LocalName == "img"
            && block.ParentElement.LocalName == "a"
            && block.ParentElement.ClassList.Contains("thumb"));

            List<string> output = new List<string>();

            foreach(IElement a in pics.ToList())
                output.Add($"https://toybike.ru{a.GetAttribute("src")}");
            return output;

        }
        private async Task<List<string>> getPartsPics(string url)
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();

            IBrowsingContext context = BrowsingContext.New(config);

            IDocument doc = await context.OpenAsync(url);


            IEnumerable<IElement> pics = doc.All.Where(block =>
            block.LocalName == "img"
            && block.ParentElement.LocalName == "a"
            && block.ParentElement.ClassList.Contains("thumb"));

            List<string> output = new List<string>();

            foreach (IElement a in pics.ToList())
                output.Add($"https://toybike.ru{a.GetAttribute("src")}");
            return output;
        }
    }


}

