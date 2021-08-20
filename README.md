# vrisian
## Open-Source Asset Editor for Games
Vrisian is an asset editor targeted at games like Minecraft. It allows for super-simple editing of assets such as images and text files.

Vrisian is written as a WPF project in C#.

The word `vrisian` is completely made-up, by the way.

## TODO:

### Bugs
 - Zooming in the Image Editor does not go to the middle / mouse like it should

### General
 - Redo the FileTree
   - It does not update live
   - Add renaming files
   - Add file addition
   - Add file deletion
   - Add file moving

 - Export menu
   - Options
     - Scaled up (Gif of animations )
     - Before / After
     - Tiled
     - All pngs stitched (animations: gif)
   - Copy to clipboard
 - Generators
   - Chime Format new item (gui)
     - Item to override
     - texture
     - override
   - New animation (gui)
     - [Wiki](https://minecraft.fandom.com/wiki/Resource_Pack#Animation)
   - New resource pack (gui: version)
 - Style 

### Technical
 - Make AnimationViewer Calculations be on a different thread

### Text Editor
 - Find and replace (regex)
 - Show Tabs
 - Syntax Highlighting

### Image Editor
 - Image saving
 - Undo / Redo
 - Editor Tools (cursor)
   - Pencil (default)
   - Selection
   - Move
   - Flood
   - Erase
 - Tool options (size)
 - Pallette
 - Image Copy/Paste
