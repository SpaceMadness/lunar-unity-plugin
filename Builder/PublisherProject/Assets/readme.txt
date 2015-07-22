# Lunar plugin for Unity v0.0.3b (C) 2015 Alex Lementuev, SpaceMadness

https://github.com/SpaceMadness/lunar-unity-plugin

## Project Goals

Imagine, you've just added this new cool feature and want to test it but keep losing the game before you actually get there. Maybe it would be easier if zombies die faster, things go slower or you simply can't get killed? Or maybe you just want to skip all of these and jump straight into the action? On top of that, you don't really want to mess around with Editor, modify scenes or "break" anything.

If so - you should give Lunar a try! It's a free and open source Quake3-flavored-Unix-style command line solution for your game.

- Create custom commands and execute them from the terminal window
- Bind commands to the hot keys
- Create aliases to execute batches
- Define user variables which persist across game launches
- Change some aspects of your game while testing

## Quick Start

### Open Terminal window
Use Editor's menu:  Window ▶ Lunar ▶ Terminal

### Create command:
* Create new C# script `Commands.cs` in `Assets/Editor` folder:  

    using LunarPlugin;

    [CCommand("test")]
    class Cmd_test : CCommand
    {
        void Execute()
        {
            PrintIndent("Hello, Unity!");
        }
    }

* Open terminal window and type command's name:  

    > test
      Hello, Unity!

* List commands:

    > cmdlist

### Create key bindings:

* Bind to a single key:

    > bind t test

* Bind command to a short cut:

    > bind ctrl+t test

### Create config variable

* Create new C# script `CVars.cs` in `Assets/Scripts` folder:  

    using LunarPlugin;

    [CVarContainer]
    static class Cvars
    {
    	public static readonly CVar myBool = new CVar("myBool", false);
    	public static readonly CVar myInt = new CVar("myInt", 10);
    	public static readonly CVar myFloat = new CVar("myFloat", 3.14f);
    	public static readonly CVar myString = new CVar("myString", "Hello!");
    }

* Open terminal window and list variables:  
    > cvarlist
      myBool 0
      myFloat 3.14
      myInt 10
      myString Hello!

* Set variable's value:  

    > myString "Hello, Lunar!"

* Get variable's value:

    > myString
      myString is:"Hello, Lunar!" default:"Hello!"

## User Guide

https://github.com/SpaceMadness/lunar-unity-plugin/wiki

## Showcases

- [Project: Unity 2D Roguelike](http://goo.gl/je1cpc)  
Demonstrates some possible use cases of the plugin for Unity's 2D Roguelike [tutorial](https://unity3d.com/learn/tutorials/projects/2d-roguelike).  
Features:
  * Start Nth day.
  * Start Next/Prev day.
  * Restart day.
  * Override food amount.
  * Enable/disable enemies' movements (with visual feedback).
  * Hot keys bindings.
  
- [Project: Stealth](http://goo.gl/KlQgRn)  
Demonstrates some possible use cases of the plugin for Unity's Stealth [tutorial](https://unity3d.com/learn/tutorials/projects/stealth).  
Features:
  * Enable/Disable security cameras (with visual feedback).
  * Enable/Disable laser fences (with visual feedback).
  * Enable/Disable enemy AI.
  * Enable/Disable all above items at the same time.
  * Smart auto completion.
  * Hot keys bindids.
  
- [Project: Space Shooter](http://goo.gl/AzlZJp)  
Demonstrates some possible use cases of the plugin for Unity's Space Shooter [tutorial](https://unity3d.com/learn/tutorials/projects/space-shooter).  
Features:
  * Execute Editor menu item from the terminal window.  
  * Set Time.timeScale with numeric hot keys (from 0.1 to 1.0).
  * Toggle God Mode for the player (with visual feedback).
  * Command bindings and alias for God Mode.

- [Angry Bots](http://goo.gl/AB9ULT)  
Demonstrates some possible use cases of the plugin for Unity's Angry Bots [demo](https://www.assetstore.unity3d.com/en/#!/content/12175).  
Features:  
  * Quickly move between checkpoints.
  * JavaScript calls from C#.

## Social media

- Dev Blog: [lunar-plugin.tumblr.com](http://lunar-plugin.tumblr.com/)
- YouTube Channel: [Space Madness](https://www.youtube.com/channel/UCNZ2ja_pI9wqKsZcnVnt_zQ)
- Twitter: [@LunarPlugin](https://twitter.com/intent/follow?screen_name=LunarPlugin&user_id=2939274198)
- Facebook: [Unity Lunar Plugin](https://www.facebook.com/LunarPlugin)
 
## Contacts

For any other questions: lunar.plugin@gmail.com