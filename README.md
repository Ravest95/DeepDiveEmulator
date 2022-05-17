# DeepDiveEmulator
This project is aimed to add the ability to replay old deep dive missions in singleplayer or multiplayer for the game Deep Rock Galactic.

Originally taken from here: https://github.com/0P3N50URC3-F0R3V3R/blackbeard#blackbeard-event-emulator (I dont how github stuff works and I have no idea how to link to the original project).
I didn't take original program functionality (It is broken on new versions of the game) just updated "server emulation" part and added info on the deep dives. You will need to type commands manually.

# How it works:
Basically to create a deep dive mission game gets information from the site. You will block that connection and emulate you own site with the information you want.
As a drawback all game connection will be blocked (you won't be able to connect to anyone unless you will use some additional tools).
In addition to that you will not be able to use any mods. You don't have the ability to get the old versions of the mods anyway but even if you do you will not be able to start the game with them (new versions don't load with mod support turned on).

# How to install program:
- Click on the green Code button and press Download ZIP.
- Extract the folder wherever you want.
- Launch the DeepDiveEmulator\xampp\setup_xampp.bat. (You will need to do it every time you move program to new folder).
- Open DeepDiveEmulator\Certificates.
- For every file here you will need to do two things:
a) Open it, press "Install Certificate...", select "Current User", press "Next", select "Place all certificates in the following store", hit "Browse...", select "Trusted Root Certification Authoriries", press "OK", press "Next", press "Finish", press "OK".
b) Open it, press "Install Certificate...", select "Local Machine" (this is diffrent), press "Next", select "Place all certificates in the following store", hit "Browse...", select "Trusted Root Certification Authoriries", press "OK", press "Next", press "Finish", press "OK".
(Remember to do it for every file).
- Open DeepDiveEmulator\Hosts File Folder.lnk (its just a link to the folder with hosts file).
- Open hosts file with any text editor.
- After everything add too lines below and save the file. (You will probably need Admin rights to do that. Try to copy the file on the desktop, edit it and copy it back, overiting the file.)

127.0.0.1 services.ghostship.dk
127.0.0.1 services.ghostship.dk

(This will block all connection for the game, if you want to use multiplayer read the section for it in the "How to download old version of the game", if you want to restore connection remove those two line from file.)
