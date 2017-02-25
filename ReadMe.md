Unity Size Explorer
===
[![Build status](https://ci.appveyor.com/api/projects/status/6hp8elqmalmko9b7?svg=true)](https://ci.appveyor.com/project/aschearer/unitysizeexplorer)

![Unity Size Explorer example](https://github.com/aschearer/unitysizeexplorer/blob/master/Examples/Screenshot1.PNG)

Quick Start
---
Just want to run the tool using a pre-compiled binary?

[Download Unity Size Explorer](https://github.com/aschearer/unitysizeexplorer/releases/latest)

Background
---
While developing [Tumblestone][1] for mobile devices I needed to greatly reduce the amount of disk space the game required. For iOS in particular games must be at or below 100 mb. Players must be on wifi in order to download games above 100 mb -- initially, Tumblestone was over 1 gb! I developed this tool to help reduce Tumblestone from 1 gb to 100 mb.

Normally, you can view Unity Editor's log after building to see some stats on a game's file size. This is what it looks like:

    Textures      33.1 mb	 54.1% 
    Meshes        0.0 kb	 0.0% 
    Animations    0.0 kb	 0.0% 
    Sounds        8.3 mb	 13.6% 
    Shaders       172.8 kb	 0.3% 
    Other Assets  8.2 mb	 13.4% 
    Levels        82.1 kb	 0.1% 
    Scripts       4.7 mb	 7.7% 
    Included DLLs 6.4 mb	 10.5% 
    File headers  201.5 kb	 0.3% 
    Complete size 61.3 mb	 100.0% 

    Used Assets and files from the Resources folder, sorted by uncompressed size:
     2.1 mb	 3.4% Assets/Spritesheets/v2/Spritesheet1.png
     2.1 mb	 3.4% Assets/Spritesheets/v2/Spritesheet2.png
     2.0 mb	 3.3% Assets/Spritesheets/v2/Spritesheet3.png
     2.0 mb	 3.3% Assets/Spritesheets/v2/Spritesheet4.png
     // list continues for every file included in the game

This is quite helpful. It tells you how big your game is and breaks things down by high level categories. You can even look through the list of assets that follows to find the worst offendors. When first starting to optimize file size this is sufficient as oftentimes the largest files (listed at the top) can be optimized yielding big improvements.

Howvever in the quest to fit under 100 mb I reached a point where the list of files was no longer very useful. That's because there was no longer one or two big files to optimize. Everything non-essential was already quite small, and if I wanted to carve off another 10 or 15 mb I needed to strike at whole folders or class of files. Unity's log file, which lists every file sorted by size, makes it very hard to see the bigger picture.

Further aggravating things is the fact that Unity only generates the above after building. In my case building for iOS often took 5 to 10 minutes. This meant that my workflow was drawn out as I made a change, built, then compared the old and new log files. Sometimes the results worked as expected and I could continue. Other times the changes had a smaller impact and were discarded. I needed a way to more quickly and accurately gauge what impact an optimization would have without having to spend 10 minutes per an iteration.

That's where this tool comes in. It reads the log file and generates a tree view and pie chart. The tree view lists every file grouped by folder -- just like in the file system. You can expand or collapse folders to view sub-folders and files. You can check on or off a given file or folder, excluding the corresponding size from the projected game file size. Plus there's a nice pie chart visually showing how much space everything uses relative to the rest of the project.

Questions, Bug Reports or Feature Requests?
---
Do you have feature requests, questions or would you like to report a bug? Please post them on the [issue list][4].

Contributing
---
As this projected is maintained by one person, I cannot fix every bug or implement every feature on my own. So contributions are really appreciated!

A good way to get started:

1. Fork the Unity Size Explorer repo. 
1. Create a new branch in you current repo from the 'master' branch.
1. 'Check out' the code with Git or [GitHub Desktop](https://desktop.github.com/)
1. Push commits and create a Pull Request (PR)

License
---
Unity Size Explorer is open source software, licensed under the terms of MIT license. 
See [License.txt](License.txt) for details.

How to Build
---
Unity Size Explorer is a WPF program written for Windows. It requires .NET 4.5.2. Use Visual Studio and open solution file under `Source`. You may need to restore Nuget packages on first run.

How to Run
---
  1. Double-click UnitySizeExplorer.exe
  1. Go to File > Open
  1. Navigate to Unity Editor's log file. (Typically under $HOME\AppData\Local\Unity\Editor)
  1. Check or uncheck items in the tree view to add/remove them from the pie chart and estimated file size. Expand folders to work with their children directly.
  1. It helps if you first filter out very small files as they clutter the UI and can make the tool sluggish. Go to Filter > Small. These items will still be counted in the final size, but will be hidden from the tree view and pie chart.
  1. **Note that you must clear the contents of the log file before building in Unity to ensure that there is only one size entry.**

[1]: http://tumblestonegame.com
[2]: https://github.com/aschearer/unitysizeexplorer/releases/latest
[3]: https://github.com/aschearer/unitysizeexplorer
[4]: https://github.com/aschearer/unitysizeexplorer/issues
