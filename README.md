# KQB Mod
A mod for the Killer Queen Black game that provides training modes, better free play and **remote play**.

## How it works
Using some tools, extra logic is added to the base game to provide new functionallity. You must own the game and install these tools on top.

## How to use

The first time you open the game with the mod applied, Unity Mod Manager UI will show up. If it doesn't or at any time you want to open it, hit `CTRL + F10`.

### How to host LAN Games
One player has to host the game, act like the server while all other players will connect to their game. From the game's main menu select `Local` then `Free Play`. While in the lobby, feel free to configure while awaiting for the other players to connect.

The players who wish to connect to the server need to open the UMM UI, (`CTRL + F10`) and in the Networking section of the menu, input the host's ip address and enable the `Remote` checkbox. Hit `Save` and `Close`. Then go to `Local` then `Free Play` and it should connect to the host.

To setup the network and know the IP and port check the following sections.

### Port forwarding
LAN stands for Local Area Network. Within a local network it's easy to connect directly to other computes. The IPs in local networks are typically `192.168.x.x` for small networks and `10.x.x.x` for larger ones. When computers needs to reach across the Internet these addresses don't work and instead need to use a public IP address.

To know your public IP address, an easy way is to check [NordVPN what is my IP](https://nordvpn.com/what-is-my-ip/). With this address, computers across the Internet can reach you, but this address will only reach to your router, which will not forward messages to your computer. To get it to forward incoming connections you will need to configure your own router to do "port forwarding" using NAT.

Configuring port forwarding is router dependant but in essence you need to tell the router to forward connections arriving at a specific port of your choosing and send to a local ip address and port. The local IP address should be the one assigned to your PC and the local port `5000` which is what KQB uses.

Players connecting will have to input your public IP and port chosen.

### Remote LAN
An easier way is to use a software like [Hamachi](https://www.vpn.net/) to setup a 'remote LAN'. You will have to create an account and install. Then you can create a network that other's can join. Then the IP address seen in Hamachi can be used to connect to other computers in the network without setting up port forwaring. The problem seems to be that Hamachi only offers up to 5 computers in the free tier while KQB can host up to 8 players. You may either pay for it, find another similar software or figure out port forwarding.

### Firewalls
It's possible even after setting up all the networking that a Firewall blocks the network traffic on that port. You will need to figure out your PC's configuration to enable incoming traffic on port `5000`.

#### Why this works
The game is designed internally to separate clients from servers, this allows the online play. This is even how the game runs locally. When playing in any local mode, like Local Play or the Tutorial, the game runs a local server and then connects to it through the "loopback ip" (`127.0.0.1` or `localhost`) using the port `5000`. The mod simply allows the option to input a different ip and port.

## How to install
This section is for players who want to improve their experiencie of KQB.

**Summary:**
- Download KQB Mod from the release section
- Download [Unity Mod Manager](https://www.nexusmods.com/site/mods/21/) (tested with 0.25.0.0)
- Install the mod through the UI, guide [here](https://www.youtube.com/watch?v=dB74FWu-pDw)
	- If the game is not listed yet, add Killer Queen Black to `UnityModManagerConfig.xml` (see below)

### Nexus Mods
To apply the mod, we will need Unity Mod Manager (UMM). Its official releases are hosted in Nexus Mods and the only way to access the files is to create and account. **Do not try to avoid this, downloading UMM from any other site is a risk.** There are plenty of other sketchy sites that host UMM that may add harmful code to it. While this seems like a hassle it's the safer option.

When creating you account, a step will ask you to pay for a premium account but you don't have to if you don't want to, there is a free option hidden all the way down.

### Unity Mod Manager
Download and install the latest version of [Unity Mod Manager](https://www.nexusmods.com/site/mods/21/?tab=files) from Nexus Mods, choose the 'Slow Download (FREE)' option. You will need an account, see previous section. Check the [wiki](https://wiki.nexusmods.com/index.php/Category:Unity_Mod_Manager) and [source code](https://github.com/newman55/unity-mod-manager/).

> DO NOT FALL FOR OTHER WEBSITES, THE OFFICIAL VERSION LIVES IN NEXUS MODS SITE

While Killer Queen Black is not in the list of supported games, you can add it manually by editing the `UnityModManagerConfigLocal.xml` file to look like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<!-- Add a game profile to make it available locally in UMM. https://wiki.nexusmods.com/index.php/How_to_add_new_game_(UMM) -->
<!-- Rename the file so it won't overwrite it. The file name can be anything, but it must start with UnityModManagerConfig -->

<Config xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
	<GameInfo Name="Killer Queen Black">
		<Folder>Killer Queen Black</Folder>
		<ModsDirectory>Mods</ModsDirectory>
		<ModInfo>Info.json</ModInfo>
		<GameExe>Killer Queen Black.exe</GameExe>
		<EntryPoint>[UnityEngine.UIModule.dll]UnityEngine.Canvas.cctor:Before</EntryPoint>
		<StartingPoint>[Assembly-CSharp.dll]LoadingBarScript.Awake:Before</StartingPoint>
		<GameVersionPoint>[GameLogic.dll]GameLogic.AnalyticsUtility.Version()</GameVersionPoint>
	</GameInfo>
</Config>
```

Next time you open UMM, Killer Queen Black will be an option.

## How to develop the mod
This section if for people who want to contribute to the mod or create their own by enhancing the code.

### Killer Queen Black
You will need to own the game. All the work here is for the Steam version on Windows.

### Visual Studio
This project was built with Visual Studio 2019 Community Edition. Other versions may work just as fine.

### dnSpy
To disassemble the game's source code I use [dnSpy](https://github.com/dnSpy/dnSpy).

### Code Injection
While the Unity Mod Manager provides an entry to the game's execution, the code we write needs to be injected in the game's functions. For that I used [Harmony 2](https://github.com/pardeike/Harmony), a code injection library that allows to run code before, instead or after original functions.

## History
This mod was created in late 2020. The initial goal was having a training mode similar to Rocket League's training packs where one could practice berry throwing. While developing it, ended up adding a better free play mode, local game hosting with remote joining, rendering hitboxes as well as the berry training packs.

I wanted to publish and share the mod, but first conacted the game developers to make sure there where ok with it. They told me to hold off for a bit while they thought about it. A few days later new rules to the official Discord channel were posted, saying any type of modding would get you banned from online play and the community. I never shared the mod with anyone and kept it private to avoid any conflict for myself and any potential users.

Now by October 2022 it seems the [game servers are shutting down](https://www.gamedeveloper.com/pc/-i-killer-queen-black-i-will-shut-down-in-november-confirms-liquid-bit). With local play being the only option, the community won't be able to continue playing together, with the only option left being in the same location to play on the same computer. It doesn't have to be this way, the game is already built in such a way that LAN play works. For the community and the people who love this game and want to continue playing, I'm making the mod public.
