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

# How to install:
- Install XAMPP. (Only host need to do this in multiplayer.)
- Block connection for the game. (Only host.)
- Start XAMPP. (Only host.)
- Change the Deep Dive mission. (Only host.)
- Download an old version of the game. (Everyone.)
- Install Goldberg Emulator. (Everyone.)
- Start any VPN (This will be on you. Use Hamachi or Radmin.)
- Start the game using Goldberg Emulator. (Everyone.)

# DOWNLOAD:
- Click on the green "Code" button and press "Download ZIP".
- Extract the folder wherever you want and rename it. (I will refer to is as "DeepDiveEmulator")


# BLOCK CONNECTION:

MUST BE DONE BY HOST!

- Launch "DeepDiveEmulator\Shortcut - Hosts.bat" and use any text editor.
- Add two lines:

127.0.0.1 drg.ghostship.dk
127.0.0.1 services.ghostship.dk

- Save the file and exit. (You probably need admin rights to do that. Try to copy the file on the desktop, edit it and copy it back, overwriting the file).

WARNINGS:
Adding these lines will block the normal connection for the game on Steam. To restore connection back you need to add # before every line or delete them.

# XAMPP:

MUST BE DONE BY HOST!

- Launch "DeepDiveEmulator\Shortcut - Install.bat". (You need to do it every time you move or rename "DeepDiveEmulator" folder).
- Wait untill "Press any key to continue . . ." message pops up and press any key.
- Open "DeepDiveEmulator\Certificates".
- Start "drg.ghostship.dk.crt", press "Install Certificate...", "Next", select "Place all certificates in the following store", press "Browse...", select "Trusted Root Certification Authorities", press "OK", "Next", "Finish", "OK", "Install Certificate..." again, select "Local Machine”, press "Next", select "Place all certificates in the following store", "Browse...", "Trusted Root Certification Authorities", "OK", "Next", "Finish", “OK” and "OK".
- Start "services.ghostship.dk.crt", press "Install Certificate...", "Next", select "Place all certificates in the following store", press "Browse...", select "Trusted Root Certification Authorities", press "OK", "Next", "Finish", "OK", "Install Certificate..." again, select "Local Machine”, press "Next", select "Place all certificates in the following store", "Browse...", "Trusted Root Certification Authorities", "OK", "Next", "Finish", “OK” and "OK".
