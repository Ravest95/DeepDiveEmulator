# DeepDiveEmulator
This project is aimed for finding an ability to replay old Deep Dive missions in singleplayer and multiplayer for the game Deep Rock Galactic.
Originally took the idea from here: https://github.com/0P3N50URC3-F0R3V3R/blackbeard#blackbeard-event-emulator (I don't know how github stuff works and I have no idea how to link to the project, sorry too stupid for that). All I did is removed the Blackbeard program (it will not work with new versions of the game), updated "website emulation", added info on the deep dives.

Programs used in the package:
- XAMPP - https://www.apachefriends.org/ru/index.html (Open source package for emulating website)
- Golber Emulator - https://gitlab.com/Mr_Goldberg/goldberg_emulator (Steam emulator that emulates steam online features on LAN).

# How it works:
To create a Deep Dive mission game gets information from the website. You will block that connection and emulate your own website with the information you want.

Drawbacks/Limitation:
- Installation is a bit tricky, and you will probably trigger virus warning. (Using fake certificates for website or using Goldberg Emulator.)
- All connection for the game will be blocked. (This includes Steam version and old versions you will download.) To restore the connection you will need to edit "hosts" file manually.
- You will not be able to use any mods. (You can't use mods offline for some versions of the game.)
- You will need to type commands manually to change the Deep Dive mission.
- Deep Dive replication is not 100% accurate. (It depends on several things. You will have region, missions, objectives, warnings, anomalies, terrain, position of resources same as it was in the past.)

a) Version of the game. (Affects every aspect of the Deep Dive. To limit the number of versions you need to download I wrote every version you need for 95% accuracy. It may not have the same objective/enemy position but other things will be the same.)

b) Date and time when you started the Deep Dive. (Not tested 100%. Date and time will effect the objective/enemy position.)

c) Event that took place during the Deep Dive. (Events will affect the objective/enemy position.)

d) Random generation. (Some objectives like Black box on Salvage operation will have different position, no matter what you do.)

- Certificates that are used for website recreation will expire in 100 years. (In case you are immortal.)

# How to download:
- Click on the green "Code" button and press Download ZIP.
- Extract the folder wherever you want.
- Rename the "DeepDiveEmulator-main" to whatever you want.

# How to install:
- Install XAMPP. (Only host need to do this in multiplayer.)
- Block connection for the game. (Host.)
- Start XAMPP. (Host.)
- Change the Deep Dive mission. (Host.)
- Download an old version of the game. (Everyone.)
- Install Goldberg Emulator. (Everyone.)
- Start any VPN (This will be on you. Use Hamachi or Radmin.)
- Start the game using Goldberg Emulator. (Everyone.)

# How to install XAMPP:
- Launch the "DeepDiveEmulator\xampp\setup_xampp.bat". (You will need to do it every time you move the program or rename the folder).
- Wait untill message "Press any key to continue . . ." pops up and press any key.
- Open "DeepDiveEmulator\Certificates".
- For every file here you will need to do two things:

a) Open it, press "Install Certificate...", select "Current User", press "Next", select "Place all certificates in the following store", hit "Browse...", select "Trusted Root Certification Authorities", press "OK", press "Next", press "Finish", press "OK".

b) Open it, press "Install Certificate...", select "Local Machine" (this part is different), press "Next", select "Place all certificates in the following store", hit "Browse...", select "Trusted Root Certification Authorities", press "OK", press "Next", press "Finish", press "OK".

- Restart your PC.

# How to block connection for the game:
- Launch "DeepDiveEmulator\Shortcut - Hosts.bat" and use any text editor.
- Add two lines after everything in the file and save it. (You will probably need admin rights to do that. Try to copy the file on the desktop, edit it and copy it back, overwriting the file.)

127.0.0.1 drg.ghostship.dk

127.0.0.1 services.ghostship.dk

- Restart your PC.

(If you want to restore the connection, remove those two lines from file and restart PC.)

# How to start XAMPP:
- Launch "DeepDiveEmulator\Shortcut - XAMPP.bat".
- Press "Start" on the right of the "Apache".

# How to change the Deep Dive missions:
- Open "DeepDiveEmulator\List.xlsx". (This is Excel file with all information about deep dives, you can press "+" or "-" above the tabs to see/hide info).
- Copy the command from the "Command" tab for the Deep Dive you need.
- Launch "DeepDiveEmulator\Shortcut - Deep Dive - drg.ghostship.dk.bat" and use any text editor.
- Launch "DeepDiveEmulator\Shortcut - Deep Dive - services.ghostship.dk.bat" and use any text editor.
- For both files remove the old command, add new one and save them.
- Restart the game. (In case of multiplayer only host needs to restart the game.)

# How to download an old version of the game:
(If the guide below won't work, you will need to use https://github.com/SteamRE/DepotDownloader/releases).

- Open Steam.
- Open your browser and open new tab.
- Type in:

steam://nav/console

- Press "Open". (This will open a console in the Steam).
- Type in command bellow. Don't press "Enter". You need to change "MANIFEST" to the right number.

download_depot 548430 548431 MANIFEST

- Open "DeepDiveEmulator\List.xlsx".
- In the file copy the value for the deep dive you want in the "Manifest Custom" tab (If you can't see the tab press the "+" above the "Name" tab).
- Open Steam again and change "MANIFEST" to the value you copied.

download_depot 548430 548431 6260931986219987085

- Press Enter.
- Wait until "Depot download complete" message will pop up.
- Open the folder that is written on the right of the "Depot download complete".
- Move the "depot_548431" wherever you want.
- Rename "depot_548431" to whatever you want. (I suggest something like "Deep Rock Galactic - 1.36.70906.0" to track the version.)

# How to install Goldberg Emulator:
- Open the "DeepDiveEmulator\Goldberg Emulator".
- Copy every file to the "Deep Rock Galactic - 1.36.70906.0\Engine\Binaries\ThirdParty\Steamworks\Steamv151\Win64". ("Steamv151" can look different depending on the version of the game).
- Open "Win64\steam_settings\settings\account_name.txt".
- Change "Player" to anything you want. (This is you name in the game).
- Open Steam.
- Open your Steam profile.
- Right Click on empty space and click on copy URL page.
- Paste the URL to any text editor.
- Remove everything except numbers.
- Open "steam_settings\settings\user_steam_id.txt".
- Change "00000000000000000" to your Steam Id.
- Open Steam
- Right click on Deep Rock Galactic, select "Properties", "Local Files", "Browse"
- Open "FSD" folder.
- Copy "Saved" folder.
- Paste it into "Deep Rock Galactic - 1.36.70906.0". (This will copy you progress and settings. Be advised that some game versions will change the settings anyway.)

# How to start the game using Goldberg Emulator.
- Start the game from "Deep Rock Galactic - 1.35.64089.0\Engine\Binaries\ThirdParty\Steamworks\Steamv147\Win64\steamclient_loader.exe". (Create a shortcut for quick excess.)
- To connect to other people, you need to press SHIFT + TAB, right click on the person and select "Send Invite", other people will need to accept it. (If you can't see the menu when you press SHIFT + TAB you need to turn off RivaTuner Statistics Server before launch.)

# Legal stuff
This package assembled for educational purposes only. I did not use any code from the game. I am not responsible for any harm that comes to your account or anything that comes from using the program. If you are afraid of damaging/loosing your account or any vital data don't use the program.
