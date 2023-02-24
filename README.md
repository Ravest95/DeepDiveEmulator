# DeepDiveEmulator
Main focus of the project is to add an ability for the game Deep Rock Galactic to replay any Deep Dive in singleplayer or multiplayer with every parameter they had at the time of release. Name, region, objectives, warnings, anomalies, enemies, resources and most importantly terrain will stay 95% the same as the were back in the past (some randomness can still persist due to nature of the game). In addition some other functions preserved too: creation of random Deep Dives, creation of random  Weekly Assignments, restarting Events.

Programs used in the project:
- XAMPP - 3.3.0
- Goldberg Emulator - a855cde6

Limitations:
- Installation is manual.
- You will need to install unofficial website certificates. (Required for the game to pick up information from you website. Maybe you will get warnings from Windows).
- You will not be able to use mods. (You can't download old versions of the mods).
- You will need to type commands manually to change the Deep Dive mission.
- You need to install Goldberg Emulator to remove always online requirement and keep ability for getting information from website. (Maybe you will get warnings from Windows).

# FREQUENTLY ASKED QUESTIONS:
HOW THIS PROJECT WORKS?
To create a Deep Dive mission game gets information from the website. You will block that connection and emulate your own website with the information you want.

WHY USE THIS PROJECT WHEN A MODIFICATION FOR REPLAYING MISSIONS EXIST?
Modification will work only for deep dives in the current update. If you try to start deep dive from last year on the modern game version, new elements will interfere with cave generation, resulting in completely different mission. You need to download the version of the game when that deep dive was released or at least version that is close to it, but you can't download old modification for it.

CAN I BE BANNED?
No. Game does not have any anti-cheat, it also is not dependant on any kind of central server for major functionality (you can play offline, and progress will be preserved in online too). You can't be banned at all. In addition to that if you will use the project, all connection will be blocked (apart from local area network).

# INSTALLATION AND USE:
Some steps are required to do once, others every time you want to use the tool. In addition to that they are divided in two categories: "EVERYONE" (every person must do them) and "HOST" (only lobby host need to do that, this also includes people who want to start Weekly Priority Assignments alongside host). Also check "WARNING" section in every step.

# DOWNLOAD:
INSTALLATION FOR EVERYONE:
- Click on the green "Code" button and press "Download ZIP".
- Extract the folder wherever you want and rename it. (I will refer to is as "DeepDiveEmulator").

# BLOCK CONNECTION:
INSTALLATION FOR HOST:
- Launch "DeepDiveEmulator\Shortcut - Hosts.bat" and select any text editor.
- Add lines:
```
127.0.0.1	drg.ghostship.dk
127.0.0.1	services.ghostship.dk
```
- Save the file and exit. (You probably need admin rights to do that. Try to copy the file on the desktop, edit it and copy it back, overwriting the file).

USE FOR HOST TO TURN ON:
- Remove `#` in front of each line.

USE FOR HOST TO TURN OFF:
- Add `#` in front of each line.

WARNING:
Connection must be blocked in order for the tool to work. Adding these lines will block the normal connection for the game on Steam (you will not be able to join anyone in the game while connection is off).

# XAMPP:
INSTALLATION FOR HOST:
- Launch "DeepDiveEmulator\Shortcut - Install.bat". (You need to do it every time you move or rename "DeepDiveEmulator" folder).
- Wait until "Press any key to continue . . ." message pops up and press any key.
- Open "DeepDiveEmulator\Certificates".
- Start "drg.ghostship.dk.crt", press "Install Certificate...", "Next", select "Place all certificates in the following store", press "Browse...", select "Trusted Root Certification Authorities", press "OK", "Next", "Finish", "OK", "Install Certificate..." again, select "Local Machine”, press "Next", select "Place all certificates in the following store", "Browse...", "Trusted Root Certification Authorities", "OK", "Next", "Finish", “OK” and "OK".
- Start "services.ghostship.dk.crt", press "Install Certificate...", "Next", select "Place all certificates in the following store", press "Browse...", select "Trusted Root Certification Authorities", press "OK", "Next", "Finish", "OK", "Install Certificate..." again, select "Local Machine”, press "Next", select "Place all certificates in the following store", "Browse...", "Trusted Root Certification Authorities", "OK", "Next", "Finish", “OK” and "OK".

USE FOR HOST TO TURN ON:
- Launch "DeepDiveEmulator\Shortcut - XAMPP.bat"
- Select first "Start" button. ("Apache" must turn green).

USE FOR HOST TO TURN OFF:
- Select first "Stop" button. ("Apache" must turn grey).
- Right click on the XAMPP's tray icon and select "Quit" button.

WARNING:
XAMPP must be started for the tool to work.

# GAME VERSION:
INSTALLATION FOR EVERYONE FOR CURRENT VERSION:
- Open Steam.
- Right click on “Deep Rock Galactic".
- Select “Properties”, “Local Files”, “Browse”.
- Go back one folder to "Deep Rock Galactic".
- Copy "Deep Rock Galactic" folder wherever you want and rename it. (I will refer to is as "Deep Rock Galactic - VERSION". Suggested name "Deep Rock Galactic - Current").

INSTALLATION FOR EVERYONE FOR OLD VERSION TO FIND MANIFEST:
- Open "DeepDiveEmulator\List.xlsx".
- Find out what version you need. (Look at the colour for major updates. Sometimes there can be different shades of colour, that means you need another version for those Deep Dives or you can skip them).
- Copy the manifest on the right of the version you need.

INSTALLATION FOR EVERYONE FOR OLD VERSION USING STEAM CONSOLE:
- Open Steam.
- Copy `steam://nav/console` and paste it as URL in your Web Browser:
- Press "Enter".
- Click "Open Steam" button. (This will open console inside Steam).
- Copy `download_depot 548430 548431 [MANIFEST]` and paste it in Steam console. (Don't press "Enter" yet).
- Open "DeepDiveEmulator\List.xlsx" and copy value from "Manifest Custom" tab for the Deep Dive you want. (You can use value from "Manifest Standard" instead to recreate Deep Dive 100% perfectly).
- Change `[MANIFEST]` to value you copied from "DeepDiveEmulator\List.xlsx".
- Press "Enter".
- Wait until "Depot download complete" message will pop up.
- Open the folder that is written on the right of the "Depot download complete" up to "depot_548431".
- Move the "depot_548431" folder and rename it. (I will refer to is as "Deep Rock Galactic - VERSION". Suggested name "Deep Rock Galactic - 1.35.69455.0").

INSTALLATION FOR EVERYONE FOR OLD VERSION USING DEPOTDOWNLOADER: (Needs your Steam account name and password).
- Open `https://github.com/SteamRE/DepotDownloader/releases` link:
- Click on "depotdownloader.zip" on up-to-date tool.
- Extract archive wherever you want and rename it. (I will refer to is as "DepotDownloader").
- Copy "DeepDiveEmulator\DepotDownloader\DepotDownloader.bat" to "DepotDownloader".
- Edit "DepotDownloader\DepotDownloader.bat" with any text editor.
- Change `[MANIFEST]` to manifest from "List.xlsx".
- Change `[ACCOUNT_NAME]` to your Steam name. (This is account name, not the nickname).
- Change `[PASSWORD]` to your Steam password.
- Save the file and exit.
- Start "DepotDownloader\DepotDownloader.bat".
- Enter your Steam Guard password if you have it.
- Wait until program will close itself.
- Move the folder inside "DepotDownloader\depots\548431" and rename it. (I will refer to is as "Deep Rock Galactic - VERSION". Suggested name "Deep Rock Galactic - 1.35.69455.0").

WARNING:
Old game versions are required for recreating old deep dives. You can press "+" to see more information about Deep Dives like: original Deep Dive game version, Deep Dive parameters, Deep Dive seed. Sometimes Steam Console or DepotDownloader won't work you will need to use other method.

# PROGRESS:
INSTALLATION FOR EVERYONE:
- Open Steam.
- Right click on "Deep Rock Galactic".
- Select “Properties”, “Local Files”, “Browse”.
- Open "FSD" folder.
- Copy "Saved" folder. (Make sure to copy, NOT move).
- Paste copied folder to "Deep Rock Galactic - 1.35.69455.0\FSD".
- Open "Deep Rock Galactic - 1.35.69455.0\FSD\Saved\SaveGames".
- Rename most resent file to "Player".
- Delete other files in "Deep Rock Galactic - 1.35.69455.0\FSD".

WARNING:
Even if you use your current version you still need to rename save file.

# GOLDBERG EMULATOR:
INSTALLATION FOR EVERYONE:
- Open "DeepDiveEmulator\Goldberg Emulator"
- Copy all files here.
- Paste files into "Deep Rock Galactic - VERSION\Engine\Binaries\ThirdParty\Steamworks\Steamv151\Win64". ("Steamv151" can have different name: "Steamv149", "Steamv147").
- Open "Deep Rock Galactic - VERSION\Engine\Binaries\ThirdParty\Steamworks\Steamv151\Win64\steam_settings\settings\account_name.txt"
- Change "Player" to anything you want. (This is your name inside the game).
- Open "Deep Rock Galactic - VERSION\Engine\Binaries\ThirdParty\Steamworks\Steamv151\Win64".
- Create shortcut for "steamclient_loader.exe".

# SINGLEPLAYER:
USE:
- Start the game using shortcut.

# MULTIPLAYER:
INSTALLATION FOR EVERYONE:
- Install Virtual Private Network of your choice. (For example: Hamachi, Radmin VPN).

USE FOR HOST:
- Start the game using shortcut.
- Press "Shift + Tab". (This will open Goldberg Emulator overlay).
- Right click on the person name in "Friends" list.
- Select "Invite".

USE FOR EVERYONE:
- Start the game using shortcut.
- Wait until you will receive the invite.
- Press "ESC".
- Select "Join" button.

WARNING:
Virtual Private Network program is required in order to play with friends. If Goldberg Emulator overlay will not appear the problem is in another program that uses overlay, for example: RivaTuner Statistics Server, MSI Afterburner.

# CONTROLS:
USE FOR HOST TO CHANGE DEEP DIVE:
- Open "DeepDiveEmulator\List.xlsx" and copy command from "Command Deep Dive" tab for the Deep Dive you want.
- Launch "DeepDiveEmulator\Shortcut - DeepDiveEmulator - Deep Dive - drg.ghostship.dk.bat".
- Remove everything in the file and add command you copied.
- Launch "DeepDiveEmulator\Shortcut - DeepDiveEmulator - Deep Dive - services.ghostship.dk.bat".
- Remove everything in the file and add command you copied.

USE FOR HOST TO CHANGE EVENT:
- Open "DeepDiveEmulator\List.xlsx" and copy command from "Command Event" tab for the Deep Dive you want.
- Launch "DeepDiveEmulator\Shortcut - DeepDiveEmulator - Event - drg.ghostship.dk.bat".
- Remove everything in the file and add the command you copied.
- Launch "DeepDiveEmulator\Shortcut - DeepDiveEmulator - Event - services.ghostship.dk.bat".
- Remove everything in the file and add the command you copied.

USE FOR HOST TO CHANGE ASSIGNMENT:
- Launch "DeepDiveEmulator\Shortcut - DeepDiveEmulator - Assignment - drg.ghostship.dk.bat".
- Change `0` to any number you want between `0` and `4,294,967,295`. If you want to have same assignments with the host, use the same value as the host).
- Launch "DeepDiveEmulator\Shortcut - DeepDiveEmulator - Assignment - services.ghostship.dk".
- Change `0` to any number you want between `0` and `4,294,967,295`.

USE FOR HOST TO CHANGE FREE BEERS:
- Launch "DeepDiveEmulator\Shortcut - DeepDiveEmulator - Free Beers.bat".
- Swap `false` with `true` to enable Free Beers
- Swap `true` with `false` to disable Free Beers.

WARNING:
Every time you change the files you need to restart the game. (People who don’t have XAMPP installed don’t need to do that). Some events or absence of them will influence the deep dive generation. You don't actually need to change drg.ghostship.dk and services.ghostship.dk at the same time, it is made to so you would not care about what game version uses what type of website.

# LEGAL STUFF:
This project assembled for educational purposes only. I did not use any code from the game. I am not responsible for any harm that comes to your account or anything that comes from using the programs from this project. If you are afraid of damaging/loosing your account or any vital data don't install / use this project.
