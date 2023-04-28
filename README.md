# DeepDiveEmulator
Main focus of this project is adding an ability to replay old Deep Dives in singleplayer or multiplayer with every original parameter. In addition to that program is able to recreate some always online functionality: creation of Deep Dives, creation of Weekly Assignments, creation of Events.

# Supported Operational Systems:
Windows 10 - x64

# Frequently Asked questions:
- Why use this project, when a modification for replaying missions exist?

  Modification will work only for Deep Dives in the current update. If you try to create Deep Dive from previous updates, new elements will interfere with creation, resulting in completely different Deep Dive.

- Can I be banned?
  No. Game does not have an anti-cheat (an anti-cheat is not installed during game installation), game also is not dependant on central server for progress (you can play offline, and progress will be preserved in online). In addition to that, all connection will be blocked (apart from local area network) for the game if launched from the program. You will not connect to Steam or any official website.
  Your account may be suspended only if you will try to distribute project files or advertise the project on official game websites (use common sense).

# How Program Works:
By default, on launch, game will connect to an official website and receive commands that are used by the game to create Deep Dives, Event and Weekly Assignments. The program will start a website on your computer, that will broadcast information only to your local IP (only to your computer) or to a custom IP of your choice and redirect connection from official website to your own. This will allow you to create Deep Dives, Events and Weekly Assignments, whenever you want.

Recreation of old Deep Dives and Events will require you to download old game version (original or compatible with this Deep Dive or Event). This is because new versions have new elements that will interfere with Deep Dive creation and they don't support old Events because of their removal in favour of new ones.

# Limitations:
1) The program will work only with Steam game version.

  This is because of inability to download old game versions and recreate multiplayer functionality in Microsoft Store.

2) The program will need admin rights (run as administrator) for automatic installation.

  This is because to add/remove redirects program will try to edit windows hosts file, that is protected by default and to add/remove certificates it will try to access Local Machine Certificate Store, which requires admin rights.

  Manual installation exists as alternative.

3) Game must be launched from the program with Goldberg Steam Emulator.

  This is because if you try to launch an old game version from Steam, it will force you to download update. If you launch the game with game executable (FSD.exe or FSD-Win64-Shipping.exe), it may display "Waiting for Steam to load" message and will not load into the lobby.

  Launching the new game version through Steam is not advised but is possible with limitations: connection to random lobbies will be blocked, connection to friends will require the use of Steam friends list and possible bugs tied to current game version.

4) Ability to use mods will be blocked.

  This is because you can't download old versions of mods from mod.io and different game versions have their own behaviour. Some game versions will delete all installed mods from your computer when they won't be able to connect to mod.io (when it will be launched with Goldberg Steam Emulator). Some will block mod support by default. Some will crash the game on start with or without mods ("-disablemodding" launch parameter is used to avoid that).

  A way to use mods does exist but it requires you to download mods that were preserved for the specific game version and editing a lot of files for the game. This will be one of the program features in the future.

# Legal Info:
This project is assembled for educational purposes only. I did not use any code from the game. I am not responsible for any harm that comes to your account or anything that comes from using the programs from this project. If you are afraid of damaging/losing your account or any vital data don't install/use this project.
