# DeepDiveEmulator
Main focus of this project is adding an ability for the game Deep Rock Galactic to replay old Deep Dives in singleplayer or multiplayer with every original parameter. In addition to that application is able to recreate some always online functionality: creation of Deep Dives, creation of Weekly Assignments, creation of Events.

Even if no file or code was used, main idea came from another project ([link](https://github.com/0P3N50URC3-F0R3V3R/blackbeard)).

# Operational Systems:
Windows 10 - 1809 (17763.379) - x64

# Frequently Asked questions:
- Why use this project, when a modification exist?

  Modification will work only for Deep Dives in the current update. If you try to create Deep Dive from previous updates, new elements will interfere with creation, resulting in completely different Deep Dive.

- Can I be banned?
  No. Game does not have an anti-cheat (an anti-cheat is not installed during game installation), and is not dependant on central server for progress (you can play offline, and progress will be preserved in online). In addition to that, all connection for the game will be blocked (apart from local area network), you will not connect to Steam or any official website.
  Your account may be suspended only if you try to distribute project files or advertise the project on official game websites (use common sense).

# How it Works:
By default, on launch, game will connect to an official website and receive commands that are used by the game to create Deep Dives, Event and Weekly Assignments. The application will start a website on your computer, that will broadcast information only to your local IP (only to your computer) or to a custom IP of your choice and redirect connection from official website to your own. This will allow you to create Deep Dives, Events and Weekly Assignments, whenever you want.

Recreation of old Deep Dives and Events will require you to download old game version (original or compatible with this Deep Dive or Event). This is because new versions have new elements that will interfere with Deep Dive creation and they don't support old Events because of their probable removal in favour of new ones.

# Limitations:
- The application will work only with Steam game version.

  This is because of inability to download old game versions and recreate multiplayer functionality in Microsoft Store.

- The application will ask for admin rights.

  This is because to add/remove redirects application will try to edit windows `hosts` file, that is protected by default and to add/remove certificates it will try to access Local Machine Certificate Store, which requires admin rights.

- Game must be launched from the application using Goldberg Steam Emulator.

  This is because of inability to block Steam updates for the game completely. If you launch the game with game executable (FSD.exe or FSD-Win64-Shipping.exe), it may display "Waiting for Steam to load" message and will not load into the lobby (game has this behaivor on and off with each update).

# Legal Info:
This project is assembled for educational purposes only. I did not use any code from the game. I am not responsible for any harm that comes to your account or anything that comes from using the applications from this project. If you are afraid of damaging/losing your account or any vital data don't install/use this project.
