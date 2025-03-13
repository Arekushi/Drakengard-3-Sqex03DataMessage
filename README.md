<h1 align="center">
    Drakengard 3 Sqex03DataMessage
</h1>

<p align="center">
  <a href="#" target="blank">
    <img src="https://i.imgur.com/GFTrIdW.png" width="200" alt="Zero" />
  </a>
</p>

<p align="center">
    Drakengard 3 Translation Tool is a command-line interface (CLI) application that enables seamless text translation within the game, without any size limitations.
</p>

## About the project
This project was not originally developed by me but by Vietnamese developers. Thanks to this tool, I was able to create a **Brazilian Portuguese** translation project for Drakengard 3, a language not natively supported by the game. You can check out my project [here][pt-br]. 😉

My motivation for forking this repository was to adapt the original solution into a CLI, making it easier to automate my translation workflow. You can download the latest release [here][last_release].

I am deeply grateful to the creators of the original repository—without their work, my project wouldn't have been possible. You can find the original repository [here][source_repo]. 💻

## Usage
### Preparations
To get started, first you need to have the following files on hand:
1. **`ALLMESSAGE_SF.XXX`**
2. **`MISSIONMESSAGE_SF.XXX`**
    * The above files can be found in the following directories (either one is valid):
        1. `drakengard_3_folder\PS3_GAME\USRDIR\SQEX03GAME\COOKEDPS3`
        2. `rpcs3_folder\dev_hdd0\game\BLUS31197\USRDIR\PATCH\SQEX03GAME\COOKEDPS3`
3. **`PS3TOC.TXT`**
    * This file can be found in the following directory:
        1. `drakengard_3_folder\PS3_GAME\USRDIR\SQEX03GAME`

Once you have these files, place them in any folder and let’s proceed with the exporting and repacking. 🎉

### 🚀 Exporting the Texts
For the export, we use a command called `export`
```
  -s, --source         Required. Path to the folder containing .XXX and TOC files (ALLMESSAGE_SF.XXX,
                       MISSIONMESSAGE_SF.XXX, and PS3TOC.TXT).

  -d, --destination    Required. Directory where the exported files will be saved.

  -o, --one-file       (Default: false) Determines whether all texts will be saved in a single .txt file (true) or
                       multiple .txt files (false).
```

To extract, just run the following command:
```bash
& '.\D3 Sqex03DataMessage.exe' export -s "source_folder_path" -d "destination_folder_path"
```

At the end of the export, a file called `_export.json` will be generated — this file will be important for the repacking stage, so **DO NOT DELETE IT**.

### 🗃️ Repacking the Texts
For repacking, we use a command called `repack`
```
  -s, --source      Required. Path to the folder containing the .XXX and TOC files (ALLMESSAGE_SF.XXX,
                    MISSIONMESSAGE_SF.XXX, and PS3TOC.TXT).

  -p, --patch       Required. Path where the export.json file is located, along with either a single .txt file or
                    multiple separate .txt files.

  -o, --one-file    (Default: false) Determines whether all texts will be read from a single .txt file or if they are
                    separated.
```

To properly repack the files back into .XXX format, you need to provide the folder containing the files `(ALLMESSAGE_SF.XXX, MISSIONMESSAGE_SF.XXX, e PS3TOC.TXT)` and the folder where the modified `.txt` files are located.

```bash
& '.\D3 Sqex03DataMessage.exe' repack -s "source_folder_path" -p "patch_folder_path"
```

## Original Creators
| [<div><img width=115 src="https://avatars.githubusercontent.com/u/51288927?v=4"><br><sub>Lê Hiếu</sub></div>][lehieugch68] <div title="Code">💻</div> | [<div><img width=115 src="https://viethoagame.com/data/avatars/o/0/4.jpg?1504006389"><br><sub>Oblivion</sub></div>][oblivion] <div title="Code">💻</div> |
| :---: | :---: |

## Contributors
| [<div><img width=115 src="https://avatars.githubusercontent.com/u/54884313?v=4"><br><sub>Alexandre Ferreira de Lima</sub></div>][arekushi] <div title="Code">💻</div> |
| :---: |

<!-- [Constributors] -->
[arekushi]: https://github.com/Arekushi
[lehieugch68]: https://github.com/lehieugch68
[oblivion]: https://viethoagame.com/members/oblivion.4/

<!-- [Some links] -->
[source_repo]: https://github.com/lehieugch68/Drakengard-3-Sqex03DataMessage
[pt-br]: https://github.com/Arekushi/drakengard-3-pt-br-translation
[last_release]: https://github.com/Arekushi/Drakengard-3-Sqex03DataMessage/releases/latest
