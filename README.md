# Requirements to Open the Project

> [!IMPORTANT]
> ### Unity Version
> - **Unity 6000.3.28f1 LTS**

## **The backstory to OpenCPI:**
- Malcolm (wednesday2024) started working on decompiling Club Penguin Island sometime in 2019, then took a massive break until either 2022 or 2023, when he began exploring the idea of bringing Club Penguin Island to Linux so that it could run natively without the help of Wine or Steam's Proton. On May 5th, 2024, OpenCPI was born to preserve and open source the game's recreated source code, bringing parties, bug fixes, improvements, and expanded features (though we weren't called OpenCPI when it first opened. We initially named it "CP Island Localhost Server," but shortly after launching, we changed the name to the OpenCPI branding that we know today). In August of 2024, we put what we had on the Internet Archive, but we later moved it to GitHub in the fall of 2024. In April of 2025, we brought builds to Itch (which later got routed to the builds that you would find on our website). Fast forward to late September 2026, we rebranded OpenCPI to OpenCPI Interactive when we brought the game from the Built in Render Pipeline (BIRP) to the Universal Render Pipeline (URP). We have support for the following platforms: Windows (Vulkan Graphics API), Android (Vulkan Graphics API), Linux (Vulkan Graphics API), macOS (Intel + Apple's Arm Silicon), and an experimental but no longer maintained WebGL. The Android code should theoretically be ready for iOS as well but it is largely untested.

 ## **Builds can be found here:**
 - [Builds](https://opencpisland.com/)

> [!CAUTION]
> ## **How do I clone the repo?**
> - I do NOT recommend using GitHub's built in ZIP download feature. Because it tends to forget to download files which breaks the project. I only recommend using the older HTTPS git clone or the new Git CLI gh repo clone options to download the repo on Linux. If you are on Windows or macOS, I recommend using git clone or using the official GitHub Desktop app. 

> HTTPS method:
> ```git clone https://github.com/wednesday2024/CPI-Project.git```

> Git CLI method:
> ```gh repo clone wednesday2024/CPI-Project```

> [The Github Desktop app method](https://github.com/apps/desktop)

 ## **Commonly asked questions:**

 Q: Why is everything pink or not loading when I load the game?

 A: You need to run the Unity Editor menu item:```Project -> AssetBundles -> Generated -> Generate client side AssetBundles```

 ### Notes
 - **The Penguin data gets stored here in the Windows Registry:**  
  - **Built .exe client:** `HKEY_CURRENT_USER\SOFTWARE\OpenCPI Interactive\CP Island`  
  - **Unity Editor:** `HKEY_CURRENT_USER\SOFTWARE\Unity\UnityEditor\OpenCPI Interactive\CP Island`
 - **The Penguin data gets stored here on Linux for the built client but the Editor path is currently unknown because we had to rewrite the data paths to play more nicely with Linux:**
  - `/home/your_username/.config/unity3d/OpenCPI Interactive/CP Island/prefs`
 - To launch the game in the Unity editor:
   - Open `Assets/Game/Core/Scenes/Boot.unity`
  - Hit the Play button.
  - Or click the "Disney Fast Play" menu item under "Project" from any scene.
 - **Join our Discord for support, chatting, or for future updates:** [join here](https://discord.gg/jkWbd3uqTS)

> [!IMPORTANT]  
> ## System Requirements

> ### Windows
> - Latest version of Visual Studio Community.
> - [Git for Windows](https://gitforwindows.org/) installed. **Restart your PC after installing Git.**

> ### macOS
> - Latest version of Xcode for your version of macOS.

> ### Linux
> - Make sure your system is updated:
>   ```bash
>   sudo apt update && sudo apt upgrade
>   ```
> - Install Git:
>   ```bash
>   sudo apt install git gh
>   ```
>  - Make sure to install this component for X11 Window manager distros (this will be used within the editor and outside of the editor during runtime):
>    ```bash
>    sudo apt-get install libx11-dev
>    ```

> [!IMPORTANT]
> For further documentation, refer to the [OpenCPI Docs](DevAssets/Offline-Project-Instructions). If something is missing, feel free to create a fork and send a pull request.

> ## Special thanks to the following people who have made this restoration possible:

> [Galaxyrelic](https://github.com/Galaxyrelic)

> [ChavalSaturado](https://github.com/ChavalSaturado?tab=repositories)

> [PickleOnAString](https://github.com/PickleOnAString)

> [broimluna](https://github.com/broimluna)

> [shinonasada9](https://github.com/shinonasada9)

> [wednesday2024](https://github.com/wednesday2024)

> [AllinolCP](https://github.com/AllinolCP)

> [Minileandro](https://github.com/Minileandro)

> [Thunder](https://github.com/Ivorplayz)

> [Koloway](https://github.com/Koloway)

> [Maksim](https://github.com/loozmax)

> [tboskkk](https://github.com/tboskkk)

> miraculizado (Discord)

> approt (Discord)
