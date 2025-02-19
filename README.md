## Requirements to Open the Project

### Unity Version
- **Unity 2022.3.58f1 LTS**

### Notes
- When opening the project, Unity might ask if you want to use the new input system. **Press NO.**
- **The Penguin data gets stored here in the Windows Registry:**  
  - **Built .exe client:** `HKEY_CURRENT_USER\SOFTWARE\OpenCPI\CP Island`  
  - **Unity Editor:** `HKEY_CURRENT_USER\SOFTWARE\Unity\UnityEditor\OpenCPI\CP Island`
- To launch the game in the Unity editor:
  - Open `Assets/Game/Core/Scenes/Boot.unity`
  - Hit the Play button.

### Future Unity Engine Versions Notes
- **Bake lightmaps in Unity 2022 ONLY** because of Enlighten. This keeps them closer to the original.
- Lightmaps in Unity 2023+ (with Progressive CPU/GPU) look weird—too bright or noisy.
- The Unity 6 branch is good for regular gameplay. Pre-rendered lightmaps in the `EventLightmaps` folder work fine in Unity 6.
- **DO NOT update to Unity 7 or newer**. They drop legacy input system support, and we'd have to recode everything.

### New and fixed features within the game
- What's new:
    - Discord RPC support. Here is an example of the Discord RPC:
      
      ![image](https://github.com/user-attachments/assets/ea786664-c5fa-47e9-8d23-5776ab85b840)

    - Added the unreleased igloo furniture ```grand father clock``` from version 1.6.1
    - Added 3 custom party hat recolors (Halloween Party Hat, Holiday Party Hat, and Anniversary 19 Party Hat)
    - Added 4 custom duck tube recolors (Blue, Green, Pink, and Purple)
    - Refined the lightmap baking process
    - The Classic Arcade machine has been moved from near Franky's Pizza in Island Central to the sewer in Island Central
    - Added and optimized support for native macOS Arm Silicon (Apple M1, M2, M3, M4, and newer chips)
    - Changed the Waddle On login coins award from ```1000000``` to ```0```
    - Added 2 new lighting options to the igloos. Those are ```Holiday``` and ```Rainbow Migration```. The ```Holiday``` lighting can be unlocked at Penguin level 20 and the ```Rainbow Migration``` lighting can be unlocked at Penguin level 27
    - Added an optional skybox in the project to allow a day/night cycle that will cycle every 15 minutes
    - Added 32 new Penguin colors
    - Unlock the Valentines Day chair at level 27. The ID for the chair is 278 and it will sell at the Igloo furniture shop for 40 coins
    - And most importantly, the game is no longer in the original 32-bit state! This recreation is in a 64-bit state
    - Version 1.13.1
    - Changed the default 3 igloo save slots to 10
    - Changed the default 130 max igloo furniture limit to 500

- What has been fixed:
    - The spawn points have been moved so you will no longer spawn into the void and endlessly fall randomly like in the original
    - The ```shoulder pack``` blueprint will correctly work now
    - The ```modern coffee table``` and ```kitchen island ``` igloo furnitures can now be obtained instead of having it give you the ```teleporter``` igloo furniture
    - Fixed missing scripts from the Mt. Blizzard Halloween 2018 decorations (this was causing errors in the original client)
    - Fixed the Halloween 2018 Pumpkins flicker speed to match how they wanted it in the original (the editor and built client shows 2 different results. So the built client would make it too fast)
    - Added missing colliders to certain world and quest objects
    - Performance improvements
    - Added the missing Summer Splashdown chat phases and Rookie sound effects to the Regular sewer in Island Central
    - Fixed original errors within the ```Unlit Dynamic Object No FOG``` and Igloo ```CubeMap``` shaders
    - Fixed the Disney Store banners and for sale items, they originally stopped working on: ```January 1, 2020```. Now they will stop working on: ```December 31, 4065```
    - Fixed the coins and collectibles that would spawn once a day (this broke when the servers went offline). They will now spawn once every 24 hours
    - Fixed the microphone and guitar interactables collision in Island Central. The collisions were swapped in the original
    - Fixed the collider on the ```boss computer chair``` igloo furniture
    - Fixed the ```boss computer chair``` from spawning a little bit into the ground
    - Fixed the ```cushion``` and ```stool``` igloo furniture being labeled as a "tube" when it should be labeled as a "ManipulatableObject"
    - Fixed the ```diamond flower pot``` igloo item collision
    - Fixed the ```modern coffee table``` and the ```kitchen island``` igloo furniture so that you can properly place items on top of them
    - Fixed the ```Indoor wall light``` igloo item so that it can be placed properly on the walls of your igloo
    - Fixed the collision on the ```CrystalCave``` igloo building
    - Fixed the cosmic daily spin chest reward. Now can now be obtainable
      
## System Requirements

### Windows
- Latest version of Visual Studio Community.
- [Git for Windows](https://git-scm.com/downloads/win) installed. **Restart your PC after installing Git.**

### macOS
- Latest version of Xcode for your macOS.

### Linux
- Make sure your system is updated:
  ```bash
  sudo apt update && sudo apt upgrade
  ```
- Install Git:
  ```bash
  sudo apt install git
  ```
- For other distros, check the [official Git installation guide](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git).

