# `alch` Registry Generator

https://github.com/radj307/caco-alch-potion-builder

This is the utility used to generate the ingredients registry for my CLI potion builder / general alchemy utility project.  
Uses the [Mutagen](https://github.com/Mutagen-Modding/Mutagen) library.

# Usage
## With a Mod Manager
_The process for Vortex is probably similar, but I've never used it so I don't know._
1. Extract the latest release to a directory of your choice.
2. Add `alch-registry-generator.exe` as an external executable. The following table explains what each field does:  

  | Field | Description |  
  |:-----:|:------------|  
  | Title | Your choice, this is the name that appears in the executable list. |  
  | Binary | The path to `alch-registry-generator.exe` |  
  | Start In | Your desired output directory. |  
  | Arguments | Space-separated list of arguments. See [here](https://github.com/radj307/caco-alch-registry-builder/wiki/_new#commandline-options) for a list of valid arguments. |

  Here is an example: _(Note: Executable name is `alch-registry-generator.exe` as of v2.0.1)_  
  ![](https://i.imgur.com/xj0Ty4Z.png)  


3. Run the executable through your mod manager.  

## Manual
_Note: This method will only include manually-installed mods, if you use a mod manager, see [above](https://github.com/radj307/caco-alch-registry-builder/wiki/_new#with-a-mod-manager)._

1. Extract the latest release to a directory of your choice.
2. Run `alch-registry-generator.exe` by opening a terminal in the same directory, or by double-clicking it.

### Specifying the Output Directory
There are 2 ways to accomplish this without using a mod manager:

#### Using the Terminal
1. Open a terminal session
2. `cd` to your desired output directory.
3. Call the program by using its absolute path.

#### Using a Shortcut
1. R+Click on `alch-registry-generator.exe` and select ___Create Shortcut___.  
2. R+Click on the shortcut, select ___Properties___.  
3. Put your desired output directory in the box next to ___Start In___.

## Commandline Options
| Option | Description |
|:------:|:------------|
| `-n`  `--no-pause` | Allows the program to exit without prompting the user for input. |
| `-o <name>`  `--output <name>` | Specify the name of the output file. |
