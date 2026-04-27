# Presentation API using State Machine

This is a Unity template project (Assets file) for creating presentations or other types of animations using a state machine approach. 

## Overview

Personally, I use this project for making presentations with fancy animations, as I've found that traditional tools (PowerPoint, Google Slides, etc.) didn't really fit my needs. 

The idea behind this is a little different to the classic "slides" approach for presentations. The project uses a State Machine-like approach to allow for moving, rotating, scaling, changing the color and more, of objects smoothly from one state to another. 

I like to use Inkscape to create the basic building blocks of my presentations, e.g. a background, text elements, images, logos, etc., then export to PNG, and drag that in. The project then allows me to animate those using code. 

## Features & Limitations

The following features are included:
- Moving, scaling, rotation, changing the color of objects; playing particle systems, changing TextMeshProText
- Smooth interpolation using easings from https://easings.net/
- Moving forwards and backwards in states without things breaking
- Jumping to specific states using the Tab key
- Displaying of notes using separate VS Code window
- High extensibility
- Should work cross platform? Only tested it on Windows, though
- Primarily made for 2D, but it not only also works not in 3D, [but 4D as well](https://youtu.be/Oe7MjfQokpE)

However, there are also some limitations:
- A separate program (e.g. Inkscape) is needed to make all "building blocks" (images etc.) of the presentation
- Make sure that wherever you're running the presentation on has the same aspect ratio (e.g. 16:9) as your test window
- The "notes" feature works using a little bit of a workaround, while classic presentation systems tend to have that outside the box
- Holding a presentation requires full build (otherwise, fullscreen is not possible)
- No converting to PPTX, PDF, or whatever. Either you manually screenshot each state or send your friend an EXE
- There are no animations when moving backwards or jumping to specific states
- Some parts of the code are weird or quite complex (e.g. `AnimatedStateMachine.cs`, or whatever the hell I did years ago with `Fading.cs`)

## Getting started

Create a new project with Unity version 6.2 (6000.2.1f1). Other versions probably work fine as well, this is just the one I've used. 

You might have to change the input system in the preferences, as my project uses the old one. I don't really understand the new one, apologies x)

Delete whatever you have in your Assets folder, and paste this project in. Also make sure to install TextMeshPro.

In the `Template` folder, you can find a `TemplateScene` and a `TemplateScript.cs`. If you want you can copy and rename it, or just edit it as-is. 

The important scripts are located in the Main Camera: Your `TemplateScript.cs` and the Indexer. The Indexer references the "Indexer Entry" object. 

Whatever objects you want to use in your presentation, you should put as children of that "Indexer Entry" object. You can also put objects as children recursively. You can then reference them in code using the Indexer and the object's name.

## Holding a presentation

To use this project to hold a presentation, the effective steps depend on whether or not you would like to use Notes during your presentation.

If you use notes:
1. Build the project
2. Open up VS Code
3. Connect the presentation display to your computer
4. Set the display mode to "Extend" (e.g. using Win+P on Windows)
5. Make sure VS Code is at your computer's display
6. Set the presentation display to the main display (e.g. using System>Display>Multiple displays>Make this my main display)
7. Open the presentation binary 

If you don't use notes:
1. Build the project
2. Connect the presentation display to your monitor
3. Set the display mode to "Duplicate" (e.g. using Win+P on Windows)
4. Open the presentation binary
