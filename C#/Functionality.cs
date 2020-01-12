using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace WToon
{
    class Functionality
    {

        public static bool done = false;

        async public static void StartDownload()
        {
            MainWindow.AppWindow.Verbose.Text += "Starting...\n";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3724.8 Safari/537.36");

            string url = "https://webtoons.com";

            foreach (MainWindow.Toon cartoon in MainWindow.toons)
            {

                if (!Directory.Exists(MainWindow.saveFolder + cartoon.Title))
                {
                    Directory.CreateDirectory(MainWindow.saveFolder + cartoon.Title);
                }

                string final_page = cartoon.Url + "&page=99999";

                HttpResponseMessage response = await client.GetAsync(url + final_page);
                HttpContent content = response.Content;
                string base_page_string = await content.ReadAsStringAsync();

                HtmlDocument base_page = new HtmlDocument();
                base_page.LoadHtml(base_page_string);

                var last_page_element = base_page.DocumentNode.SelectSingleNode(@"//div[@class=""paginate""]/a[@onclick=""return false;""]/span");

                int last_page = Convert.ToInt32(last_page_element.InnerText);
                List<string> episodes = new List<string>();

                for (int page = 1; page < last_page + 1; page++)
                {
                    if (MainWindow.cancel)
                    {
                        MainWindow.AppWindow.Verbose.Text += "Stopped downloading.\n";
                        MainWindow.cancel = false;
                        return;
                    }

                    response = await client.GetAsync(url + cartoon.Url + "&page=" + Convert.ToString(page));
                    content = response.Content;
                    base_page_string = await content.ReadAsStringAsync();
                    base_page.LoadHtml(base_page_string);

                    var all_episodes_element = base_page.DocumentNode.SelectSingleNode(@"//ul[@id=""_listUl""]");

                    try
                    {
                        var episode_urls = all_episodes_element.SelectNodes(@"//a");

                        foreach (var link in episode_urls)
                        {
                            string linkValue = link.Attributes["href"].Value;

                            if (linkValue.IndexOf("episode") != -1)
                            {
                                episodes.Add(linkValue);
                            }
                        }
                        episodes.RemoveAt(episodes.Count - 1);
                    }
                    catch
                    {
                        break;
                    }
                }

                episodes.Reverse();

                int current_episode = 1;

                foreach (var episode in episodes)
                {

                    if (MainWindow.cancel)
                    {
                        MainWindow.AppWindow.Verbose.Text += "Stopped downloading.\n";
                        MainWindow.cancel = false;
                        return;
                    }

                    MainWindow.AppWindow.Verbose.Text += cartoon.Title + " - " + "Current Episode: " + current_episode + '\n';


                    if (!Directory.Exists(MainWindow.saveFolder + cartoon.Title + @"\episode-" + current_episode.ToString()))
                    {
                        Directory.CreateDirectory(MainWindow.saveFolder + cartoon.Title + @"\episode-" + current_episode.ToString());
                    }

                    response = await client.GetAsync(episode);
                    content = response.Content;
                    base_page_string = await content.ReadAsStringAsync();
                    base_page.LoadHtml(base_page_string);

                    var image_div = base_page.GetElementbyId("_imageList");

                    var images = image_div.SelectNodes(@"//img[@alt=""image""][@class=""_images""]");

                    int image_count = 0;

                    foreach (var image in images)
                    {
                        string imgUrl = image.Attributes["data-url"].Value;
                        string extension = "";

                        if (imgUrl.Contains(".jpg"))
                        {
                            extension = ".jpg";
                        }
                        else
                        {
                            extension = ".png";
                        }

                        if (!File.Exists(MainWindow.saveFolder + cartoon.Title + @"\episode-" + current_episode.ToString() + @"\" + image_count.ToString() + extension))
                        {
                            using (WebClient downloadClient = new WebClient())
                            {
                                downloadClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3724.8 Safari/537.36");
                                downloadClient.Headers.Add("Referer", "https://webtoons.com/");
                                downloadClient.DownloadFile(new Uri(imgUrl), MainWindow.saveFolder + cartoon.Title + @"\episode-" + current_episode.ToString() + @"\" + image_count.ToString() + extension);
                                image_count++;
                            }
                        }
                        else
                        {
                            image_count++;
                        }
                    }

                    if (MainWindow.AppWindow.HtmlCheckbox.IsChecked.Value)
                    {
                        string imageCount = MainWindow.saveFolder + cartoon.Title + @"\episode-" + current_episode.ToString() + @"\num_images.txt";

                        if (!System.IO.File.Exists(imageCount) && image_count != 0)
                        {
                            using (StreamWriter outputFile = new StreamWriter(imageCount))
                            {
                                outputFile.Write(image_count.ToString());
                            }
                        }

                        if (!System.IO.File.Exists(imageCount) && image_count == 0)
                        {
                            using (StreamWriter outputFile = new StreamWriter(imageCount))
                            {
                                outputFile.Write(image_count.ToString());
                            }
                        }

                        string datFile = MainWindow.saveFolder + cartoon.Title + @"\episode-" + current_episode.ToString() + @"\start_webserver.bat";

                        if (!System.IO.File.Exists(datFile))
                        {
                            using (StreamWriter outputFile = new StreamWriter(datFile))
                            {
                                outputFile.Write("python -m http.server");
                            }
                        }

                        string htmlFile = MainWindow.saveFolder + cartoon.Title + @"\episode-" + current_episode.ToString() + @"\index.html";

                        if (!System.IO.File.Exists(htmlFile))
                        {
                            using (StreamWriter outputFile = new StreamWriter(htmlFile))
                            {
                                outputFile.Write(@"<!DOCTYPE html>
<html lang = ""en"">
<head>
<meta charset = ""UTF -8"">
<meta name = ""viewport"" content = ""width =device-width, initial-scale=1.0"">
<meta http - equiv = ""X -UA-Compatible"" content = ""ie =edge"">
<title> COMIQS! </title>
</head>
<body>
<style>
html, body {
            background-color: black;
            height: 100%;
            }

::-webkit-scrollbar-track {
    box-shadow: inset 0 0 60px rgba(0, 0, 0, 0.3);
    background-color: black;
    border-radius: 5px;
}

::-webkit-scrollbar {
    width: 5px;
    background-color: black;
}

::-webkit-scrollbar-thumb {
    border-radius: 5px;
    background-color: crimson;
}

.container {
            height: 100%;
            display: flex;
            flex-direction: column;
            align-items: center;
            }
.images {
            width: auto;
            height: auto;
            }
</style>

<div class=""container"">
</div>

<script>
""use strict"";
let container = document.querySelector('.container');

    fetch('num_images.txt').then(response => response.text()).then(
        function(data) {
        const images = parseInt(data);
        let img = 1;
        setTimeout(function() {
            while (img < images)
            {
                let new_img = container.appendChild(document.createElement('img'));
                new_img.className = 'images';
                new_img.setAttribute('src', './' + img + '.jpg');
                img++;
            }
        }, 0)
    }

);
</script>
</body>
</html>");
                            }
                        }
                    }

                    current_episode++;
                }
            }

            MainWindow.AppWindow.Verbose.Text += "Done!";
            MainWindow.AppWindow.StopButton.IsEnabled = false;

            MainWindow.AppWindow.StartBtn.IsEnabled = false;
            MainWindow.toons.Clear();
            MainWindow.AppWindow.QueueList.Items.Clear();
            MainWindow.AppWindow.ClearQueue.IsEnabled = false;
            if (MainWindow.AppWindow.DeleteSelected.IsEnabled)
            {
                MainWindow.AppWindow.DeleteSelected.IsEnabled = false;
            }
        }
    }
}
