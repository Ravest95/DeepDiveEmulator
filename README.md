# DeepDiveEmulator
This project is aimed to find an ability to replay old deep dive missions in singleplayer or multiplayer for the game Deep Rock Galactic. I want to recreate the deep dives 1 to 0.95. Why 0.95? To limit the number or versions, you need to download. That will result in small changes like the position of some stones/grass, some enemies like Glyphid Brood Nexus. Thats it really, other things like Misson Name, Region, Mission Type, Warnings, Anomaly and most importantly Level terrain will be 100% same as it was back in the past.

Originally made here: https://github.com/0P3N50URC3-F0R3V3R/blackbeard#blackbeard-event-emulator (I don't know how github stuff works and I have no idea how to link to the project, sorry too stupid for that).
I didn't take original program functionality (It is broken on new versions of the game) just redid/updated "server emulation" part, added info on the deep dives maybe made installation a bit easier. I use XAMPP (Open source package for emulating the game server - https://www.apachefriends.org/ru/index.html) and Golber Emulator (Steam emulator that emulates steam online features on LAN - https://gitlab.com/Mr_Goldberg/goldberg_emulator).

# How it works:
Basically, to create a deep dive mission game gets information from the site. You will block that connection and emulate your own site with the information you want.

Drawbacks/Limitation:
- Installation is a bit tricky, and you will probably trigger virus warning. This is because of many things like: Creating fake website, using fake certificates (btw they will expire in 100 years in case you are imortal), using Goldberg Emulator to convert Steam connection to LAN. If you are afraid of this don't use the program.
- All game connection will be blocked (you won't be able to connect to anyone unless you will use some additional tools).
- You will not be able to use any mods. You don't have the ability to get the old versions of the mods anyway but even if you do you will not be able to start the game with them (new versions don't load with mod support turned on) + some versions just refuse to start mods offline.
- You will need to type commands manually to change the deep dive mission.

# How to install the program:
- Click on the green Code button and press Download ZIP.
- Extract the folder wherever you want.
- Launch the DeepDiveEmulator\xampp\setup_xampp.bat. (You will need to do it every time you move the program to new folder).
- Wait untill message "Press any key to continue . . ." will pop up and press any key.
- Open DeepDiveEmulator\Certificates.
- For every file here you will need to do two things:

a) Open it, press "Install Certificate...", select "Current User", press "Next", select "Place all certificates in the following store", hit "Browse...", select "Trusted Root Certification Authorities", press "OK", press "Next", press "Finish", press "OK".

b) Open it, press "Install Certificate...", select "Local Machine" (this is different), press "Next", select "Place all certificates in the following store", hit "Browse...", select "Trusted Root Certification Authorities", press "OK", press "Next", press "Finish", press "OK".

(Remember to do it for every file).

- Open DeepDiveEmulator\Hosts File Folder.lnk (its just a link to the folder with hosts file).
- Open hosts file with any text editor.
- After everything add too lines below and save the file. (You will probably need Admin rights to do that. Try to copy the file on the desktop, edit it and copy it back, overwriting the file.)

127.0.0.1 drg.ghostship.dk

127.0.0.1 services.ghostship.dk

(This will block all connection for the game, if you want to use multiplayer read the section for it in the "How to download the old version of the game", if you want to restore the connection remove those two line from file.)

# How to start the program:
- Launch "DeepDiveEmulator\xampp\xampp-control.exe".
- Select Language and press "Save". (This will be shown only one time).
- In the new menu press "Start" on the right of the "Apache".

# How to change the Deep Dive missions:
- Open "DeepDiveEmulator\List.xlsx". (This is Excel file with all information about deep dives).
- Copy the command from the "Command" tab for the Deep Dive you need. (If you can't see the tab press the + sign above the "Information" tab).
- Open "DeepDiveEmulator\xampp\htdocs\drg.ghostship.dk\events\deepdive" and "DeepDiveEmulator\xampp\htdocs\services.ghostship.dk\deepdive" with any text editor.
- Remove the old command, add new one for both files and save them.
- Restart the game if it was open. (In multiplayer other people don't need to do that).

# How to download old versions of the game:
(If the guide below won't work, you will need to use https://github.com/SteamRE/DepotDownloader/releases).

- Open Steam.
- Open your browser and open new tab.
- Type in:

steam://nav/console

- Press "Open". (This will open a console in the Steam).

- Type in:

download_depot 548430 548431 MANIFEST

(Don't press Enter, you need to change MANIFEST to the right number).

- Open "DeepDiveEmulator\List.xlsx".
- In the file copy the value for the deep dive you want in the "Manifest Custom" tab (If you can't see the tab press the + sign above the "Name" tab).
- Open Steam again and change "MANIFEST" with the value you copied.

EXAMPLE: download_depot 548430 548431 6260931986219987085

- Press Enter. (This will start the download).
- Wait until "Depot download complete" message will pop up.
- Open the folder that is written on the right of the "Depot download complete". (This is the old version of the game).

# How to play in singleplayer:
- Move the "depot_548431" you downloaded to the folder, where Deep Rock Galactic game is located. (You need to put the side by side. To open original game folder open Steam, Right click on the game, select "Properties", "Local Files", "Browse", then move 1 folder lower).
- Rename the "Deep Rock Galactic" to "Deep Rock Galactic - Original". (You will need to rename it back to "Deep Rock Galactic" to restore the game).
- Rename the "depot_548431" to "Deep Rock Galactic".
- Copy the folder "Deep Rock Galactic - Original\FSD\Saved" to the "Deep Rock Galactic - Original\FSD". (Make a copy don't just move it. This will copy your progress and settings. Remember that some version may change settings anyway.)
- You can start the game the same way you start it normaly. (Remember that if game will receive update, your game will be updated automaticly to the newest version.)

# How to play in multiplayer:
- Move the "depot_548431" anyware you want.
- Rename the folder to anything you want. ("Deep Rock Galactic - 1.36.70906.0" is good for tracking the version).
- Copy the folder "Deep Rock Galactic\FSD\Saved" from your original game location (Make a copy don't just move it. This will copy your progress and settings. Remember that some version may change settings anyway. To open original game folder open Steam, Right click on the game, select "Properties", "Local Files", "Browse").

- Open the "DeepDiveEmulator\Goldberg Emulator".
- Copy every file to the "Deep Rock Galactic - 1.36.70906.0\Engine\Binaries\ThirdParty\Steamworks\Steamv151\Win64". ("Steamv151" can look different depending on the version of the game).
- Open "steam_settings\settings\account_name.txt".
- Change "Player" to anything you want. (This is you name in the game).
- Open "steam_settings\settings\user_steam_id.txt".
- Change "16341198025109786" to your SteamId. (This is done to keep your progress, or you will start from 0. You can find the SteamId by opening your steam profile in the browser and looking into the URL. Open your Steam profile Right Click and copy URL page, then paste it in the browser).

(Other people who want to play with you will need to download old version, install Goldber Emulator and block the connection like you, they don't need to install DeepDiveEmulator and they don't need to install certificates. In addition to that you all need to use tools that convert LAN to VPN like: Hamachi or Radmin VPN).

To connect you need to press SHIFT + TAB, right click on the person and select "Send Invite", other people will need to exept it. (Its a bit tricky to acept the invite. If you can see the menu when you press SHIFT + TAB you need to turn off RivaTuner Statistics Server before launch, I think you can start it after the game starts. Every time you want to change the Deep Dive mission in the program you need to restart the game, othe people don't need to do that).

# Legal stuff
I did not use any code from the game. I am not responsible for any harm that comes to your account or anything that comes from using this program. If you are afraid of damaging/loosing account don't use the program. This is for educational purposes only.
