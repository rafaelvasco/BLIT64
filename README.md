# BLIT64
**C# Retro Style 2D Engine**

**Current Screenshot**

![BLIT64 Screenshot](https://github.com/rafaelvasco/BLIT64/blob/master/blit_64_screenshot.png "Screenshot")

**Mission**: Retro Fantasy Console style game engine.

Inspired by several game engines and fantasy game consoles. The editor interface is especially inspired by [TIC-80](https://github.com/nesbox/TIC-80).

**Status**: WIP

**Tech Stack**

 - C#/.NET Core 3.1+
 
 **External Libs**
 
 - SDL2 (SDL2CS)
 - Binaron.Serializer
 - StbImageSharp

**Features**

 - **Retro style** color indexed based rendering.
 - **KB and Mouse** Input, later **GamePad** too.
 - **Immediate mode** rendering style: Draw index-based bitmaps, text and primitives
 - **Palette Based** Rendering: All pixel values are index values on a palette. Change palette and everything changes.
 - **PAK** based asset handling. When the game is loaded, asset data is loaded from PAK files, which contain assets metadata and image data. 
 - **Game Editor**: In Progress: **Sprite Editor**. Planned: **Map Editor**. Future: **SFX** and **Music** editors
 - Rendering scheme: Everything is directly drawn into ***Pixmaps***. ***Pixmaps*** are arrays of colors. Each color is in fact an index inside a palette. The renderer class is called ***Blitter***. Blitter takes a  ***Pixmap*** and modifies its colors. By default ***Blitter*** draws inside the main window ***Pixmap***. If you want to draw to another pixmap just set it as draw target to ***Blitter***.  ***Pixmaps*** can be drawn on other  ***Pixmaps*** . There are some specialized ***Pixmaps***:  ***SpriteSheet***, **Fonts**. **SpriteSheet** is and indexable **Pixmap** from which you can draw portions of it or the entire thing at once. Each portion is indexed by an integer number. Alternatively names can be assigned to each portion for better referencing. ***Font*** is a kind of **SpriteSheet** in which each portion is additionally index by character. Each character correspond to a portion of the **SpriteSheet**.  Everything that is inside a ***SpriteSheet*** can be referenced and grouped inside a ***Map***. So a ***Map*** is basically a big array of indices of a ***SpriteSheet*** to be later drawn onto the screen or another ***Pixmap***. After everything is drawn to the main game **Pixmap**, the former is then translated to a **DrawSurface**  which is an array of bytes representing rgba color components, four for each color. This then is copied to a final Texture to be rendered on screen.(*Obs.: This scheme is not fully defined yet.*)
