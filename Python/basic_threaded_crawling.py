'''


THIS SCRIPT "WORKS", HOWEVER, FROM WHAT I HAVE NOTICED, AFTER LOOPING THROUGH A CERTAIN AMOUNT OF CARTOONS,
INSTEAD OF RECEIVING A NEW "BATCH", NOTHING HAPPENS.

I.E. - IT WORKS FINE FOR THE FIRST FEW CARTOONS, BUT ONCE THEY'RE DONE, THE NEW CARTOONS TO BE DOWNLOADED
DON'T GET UPDATED.



'''



import re
import urllib.request
import time
import os
import shutil
import requests
from bs4 import BeautifulSoup
from multiprocessing import Pool

url = 'https://webtoons.com'

cartoons = {'Oh Holy': '/en/romance/oh-holy/list?title_no=809', 'Trump': '/en/fantasy/trump/list?title_no=84', 'Sweet Home': '/en/thriller/sweethome/list?title_no=1285', 'Magician': '/en/fantasy/magician/list?title_no=70', 'Refund High School': '/en/drama/refundhighschool/list?title_no=1360', 'Wind Breaker': '/en/action/wind-breaker/list?title_no=372', 'Tales of the Unusual': '/en/thriller/tales-of-the-unusual/list?title_no=68', 'Age Matters': '/en/romance/age-matters/list?title_no=1364', 'Winter moon': '/en/fantasy/winter-moon/list?title_no=1093', 'Urban Animal': '/en/action/urban-animal/list?title_no=1483', 'Brothers Bond': '/en/action/brothersbond/list?title_no=1458', 'Hooky': '/en/fantasy/hooky/list?title_no=425', 'I Dont Want This Kind of Hero': '/en/fantasy/i-dont-want-this-kind-of-hero/list?title_no=98', 'The Croaking': '/en/fantasy/the-croaking/list?title_no=1494', 'Catharsis': '/en/fantasy/catharsis/list?title_no=396', 'Rebirth': '/en/fantasy/rebirth/list?title_no=1412',
'Unordinary': '/en/fantasy/unordinary/list?title_no=679', 'Save Me': '/en/drama/bts-save-me/list?title_no=1514', 'UnderPrin': '/en/fantasy/underprin/list?title_no=78', 'Dice': '/en/fantasy/dice/list?title_no=64', 'Pound': '/en/action/pound/list?title_no=1496', 'I Love Yoo': '/en/romance/i-love-yoo/list?title_no=986', 'Tower of God': '/en/fantasy/tower-of-god/list?title_no=95', 'Gosu': '/en/action/gosu/list?title_no=1099', 'Hive': '/en/thriller/hive/list?title_no=65', 'Athena Complex': '/en/fantasy/athena-complex/list?title_no=867', 'Shriek': '/en/thriller/shriek/list?title_no=772', 'Distant Sky': '/en/thriller/distant-sky/list?title_no=75', 'The Cliff': '/en/thriller/the-cliff/list?title_no=80'} # Cartoons, in the format of | "Title": "Homepage Link" |



def get_toon(cartoon): # Loops through all of the cartoons
    print(cartoon)
    if not os.path.isdir(f'E:/Webtoons/{cartoon}'): # Create folder for the cartoon if it doesn't exist
        os.mkdir(f'E:/Webtoons/{cartoon}')

    final_page = cartoons[cartoon] + '&page=999999'
    base_page = requests.get(url+final_page, headers={'User-agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3724.8 Safari/537.36'}).text
    base_page_source = BeautifulSoup(base_page, 'lxml')

    last_page = base_page_source.find('div', {'class': 'paginate'})

    last_page = last_page.find('a', {'onclick': 'return false;'})

    last_page = int(last_page.find('span').text) # Finds how many pages there are for the title

    episodes = []
    for page in range(1, last_page+1): # Goes through each page and finds the links to each of the episodes
        base_page = requests.get(url+cartoons[cartoon]+f'&page={page}', headers={'User-agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3724.8 Safari/537.36'}).text
        base_page_source = BeautifulSoup(base_page, 'lxml')
        all_episodes = base_page_source.find('ul', id='_listUl')
        all_episodes = all_episodes.findAll('li')
        old_episodes = []
        for episode in all_episodes:
            old_episodes.append(episode.find('a'))
        for episode in old_episodes:
            episodes.append(episode['href'])

    episodes = episodes[::-1] # Stores all the links for the episodes in a list

    current_episode = 1

    for episode in episodes: # Goes through each of the episodes, create a folder for each of them (if it doesn't exist), finds the links to all the cartoon's images in the page
        print(episode)
        print(f'Current episode: {cartoon} - {episode}')
        if not os.path.isdir(f'E:/Webtoons/{cartoon}/episode-{current_episode}'):
            os.mkdir(f'E:/Webtoons/{cartoon}/episode-{current_episode}')

        current_episode_page = requests.get(episode, headers={'User-agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3724.8 Safari/537.36'}).text

        current_episode_source = BeautifulSoup(current_episode_page, 'lxml')

        image_div = current_episode_source.find('div', id='_imageList')

        images = image_div.findAll('img', {'alt': 'image', 'class': '_images'})

        image_count = 0

        for image in images: # For each image link found in the episode, store it inside the current episode's folder as .jpg (if it doesn't already exist)
            if not os.path.isfile(f'E:/Webtoons/{cartoon}/episode-{current_episode}/{image_count}.jpg'):
                r = urllib.request.Request(image['data-url'], headers={'Referer': 'https://webtoons.com/'})
                response = urllib.request.urlopen(r)
                with open(f'E:/Webtoons/{cartoon}/episode-{current_episode}/{image_count}.jpg', 'wb') as f:
                    shutil.copyfileobj(response, f)
                image_count += 1
            else:
                image_count += 1
                pass
        with open(f'E:/Webtoons/{cartoon}/episode-{current_episode}/num_images.txt', 'w') as file:
            file.write(str(image_count - 1)) # Stores how many images are in the episode inside a text file, which will be used as a reference by the fetch api to load the images inside the index.html
        with open(f'E:/Webtoons/{cartoon}/episode-{current_episode}/index.html', 'w') as index:
            index.write(r'''

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <title>COMIQS!</title>
</head>
<body>
<style>
html, body {
    background-color: black;
    height: 100%;
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

<div class="container">
</div>

<script>
    "use strict";
    let container = document.querySelector('.container');

    fetch('num_images.txt').then(response => response.text()).then(
       function(data) {
           const images = parseInt(data);
           let img = 1;
           setTimeout(function () {
                   while (img < images) {
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
</html>

            ''') # Creates an index.html file to mimic Webtoon's style of displaying the cartoons (one image below another), with a black background, because white hurts the eyes much more than black, and is less comfortable to look at
        with open(f'E:/Webtoons/{cartoon}/episode-{current_episode}/run-server.bat', 'w') as faiol:
            faiol.write('python -m http.server') # Starts a local server, which is needed to load the images, using the fetch api
        current_episode += 1

if __name__ == '__main__': # Creates a pool instance, which is used to crawl multiple cartoons at the same time
    pool = Pool()
    pool.map(get_toon, cartoons)
    pool.terminate()
    pool.join()
