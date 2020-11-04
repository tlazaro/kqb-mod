# KQB Mod
A mod for the Killer Queen Black game that provides training modes, better free play and **remote play**.

## How it works
Using some tools, extra logic is added to the base game to provide new functionallity. You must own the game and install these tools on top.

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
