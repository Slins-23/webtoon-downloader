# Project Objective

Lately I have been learning JavaScript, and because of that, I have only been coding in HTML, CSS, and JavaScript, sometimes only through Node.js...

As I like web crawling, I figured it would be good practice to code something in Python, as means to not lose consistency.

I really like comics and this kind of stuff, though lately (past ~4 months), I haven't had any contact with these kinds of things, because I have been coding EVERYDAY (not because I was forced to, I like coding and I also self-imposed coding everyday in order to not lose consistency and obviously get better at programming, I legit get depressed if I can't code for at least 1-2 hours everyday, lol), which leads me to either spend most of the day fixing a script, creating one, or learning new technologies.

Well, enough of that, so, based off of that, I decided make this web crawler, which for specificied comics (hosted at "https://webtoons.com"), the script would download every released episode for each of the specified comics, create a folder for the comic, and a folder for each episode, which would store all the images for that episode...

Also create an "index.html" file, in order to mimic Webtoon's experience, in which the images are one below another in the browser. I will do that and change the background to black, because it's better and more comfortable for the eyes.

# How I would go about achieving it

1. Go to the last page of the current title, find how many pages there are;

2. For each of the pages, get the links for each episode;

3. After all links were stored, go to each of the episodes, create a folder for each one of them (in order);

4. For each episode, download all the cartoon images;

5. Create an index.html and load the images through JavaScript;

# Problems, needs for optimization, future plans

1. While not very troublesome for someone to just copy/paste the titles and links, it's quite boring and not hard for someone to copy the wrong link/mistype the title

    * Solution: The best option would be for the person to type the title into the script, which would use that as a search query (on "https://webtoons.com"), then check if the title is in the search results, if so, get the link and title for that series; Maybe login into the user's account and download every title to which that person has subscribed to.

2. The "threaded" version of the script is incomplete, or rather there's something that needs to be fixed (works fine for the first few titles, gets stuck when loading next batch)

    * Solution: Need to check the script for that, I haven't had the time yet

3. It takes quite a while for the script to complete everything (specially if you have many titles in the queue), if you retry to many times (re-run the script too many times in a short amount of time), your IP's requests will be denied by the server

    * Solution: Find better ways for the images to be downloaded, after fixing the "threaded" version of the script, possibly also add threaded options for other sections of the script

