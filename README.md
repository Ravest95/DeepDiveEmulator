# DeepDiveEmulator
Main focus of this project is to add an ability to replay any Deep Dive in singleplayer or multiplayer for the game Deep Rock Galactic with every parameter that they had at the time of release. Name, region, objectives, warnings, anomalies, enemies, resources and most importantly terrain will stay 95% the same as the were back in the past (some randomness can still persist). In addition to that some other finctions preserved too: creation of random Deep Dives, creation of random  Weekly Assignments, restarting Events.

Programs used in the project:
- XAMPP
- Golber Emulator

# How it works:
To create a Deep Dive mission game gets information from the website. You will block that connection and emulate your own website with the information you want.

Warnings/Limitation:
- Installation is manual.
- You will need to install unofficial website certificates (they are requerd for the game to pick up information from you website).
- You will not be able to use any mods (you can't use mods offline or download mods for old game versions, even if you wanted to use them).
- You will need to type commands manually to change the Deep Dive mission.

# INSTALLATION AND USE:
Some steps are requerd to do once, others every time you want to use the tool. In addition to that they are devided in two categories: "EVERYONE" (every person must do them) and "HOST" (only lobby host need to do that, this also includes people who want to start Weekly Assignments). Check "WARNINGS / INFORMATION" section in every step.

# DOWNLOAD:
- Click on the green "Code" button and press "Download ZIP".
- Extract the folder wherever you want and rename it. (I will refer to is as "DeepDiveEmulator")

WARNINGS / INFORMATION:
Must be done by EVERYONE!

# BLOCK CONNECTION:
- Launch "DeepDiveEmulator\Shortcut - Hosts.bat" and use any text editor.
- Add two lines:

127.0.0.1 drg.ghostship.dk

127.0.0.1 services.ghostship.dk

- Save the file and exit. (You probably need admin rights to do that. Try to copy the file on the desktop, edit it and copy it back, overwriting the file).

WARNINGS / INFORMATION:
Must be done by HOST! Connection must be blocked in order for the tool to work. Adding these lines will block the normal connection for the game on Steam (to restore connection back you need to add # before every line or delete them).

# XAMPP:
INSTALLATION:
- Launch "DeepDiveEmulator\Shortcut - Install.bat". (You need to do it every time you move or rename "DeepDiveEmulator" folder).
- Wait untill "Press any key to continue . . ." message pops up and press any key.
- Open "DeepDiveEmulator\Certificates".
- Start "drg.ghostship.dk.crt", press "Install Certificate...", "Next", select "Place all certificates in the following store", press "Browse...", select "Trusted Root Certification Authorities", press "OK", "Next", "Finish", "OK", "Install Certificate..." again, select "Local Machine”, press "Next", select "Place all certificates in the following store", "Browse...", "Trusted Root Certification Authorities", "OK", "Next", "Finish", “OK” and "OK".
- Start "services.ghostship.dk.crt", press "Install Certificate...", "Next", select "Place all certificates in the following store", press "Browse...", select "Trusted Root Certification Authorities", press "OK", "Next", "Finish", "OK", "Install Certificate..." again, select "Local Machine”, press "Next", select "Place all certificates in the following store", "Browse...", "Trusted Root Certification Authorities", "OK", "Next", "Finish", “OK” and "OK".

WARNINGS / INFORMATION:
Must be done by HOST! XAMPP must be started with "Apache" turned green for the tool to work.




# PROGRESS:
INSTALLATION FOR EVERYONE:
- Open Steam.
- Right click on "Deep Rock Galactic".
- Select “Properties”, “Local Files”, “Browse”.
- Open "FSD" fodler.
- Copy "Saved" folder. (Make sure to copy, NOT move).
- Paste copied folder to "Deep Rock Galactic - 1.35.69455.0\FSD".
- Open "Deep Rock Galactic - 1.35.69455.0\FSD\Saved\SaveGames".
- Rename most resent file to "Player".
- Delete other files in "Deep Rock Galactic - 1.35.69455.0\FSD".

WARNINGS / INFORMATION:
Even if you use your current version you still need to rename save file.

# GOLDBERG EMULATOR:
INSTALLATION FOR EVERYONE:
- Open "DeepDiveEmulator\Goldberg Emulator"
- Copy all files here.
- Paste files into "Deep Rock Galactic - VERSION\Engine\Binaries\ThirdParty\Steamworks\Steamv151\Win64". ("Steamv151" can have different name - "Steamv149", "Steamv147").
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
- Press "Shift + Tab". (This will open Golberg Emulator overlay).
- Right click on the person name in "Friends" list.
- Select "Invite".

USE FOR EVERYONE:
- Start the game using shortcut.
- Wait when you will receive the invite.
- Press "ESC".
- Select "Join" button.

WARNINGS / INFORMATION:
Virtual Private Network program is required in order to play with friends. If Golberg Emulator overlay will not appear the problem is in another program that uses overlay, for example: RivaTuner Statistics Server, MSI Afterburner.
