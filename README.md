# CS50G FINAL PROJECT DOCUMENTATION

Hello and welcome to the documentation of my final project on CS50's Introduction to Game Development. In this document I will try to explain my journey of creating a game from scratch. *(For more information, "almost" every script I created has extensive use of comments, explaning what it does.)*

## Coming Up With the Idea For This Game

In the first place I wanted to make a 3D Hack & Slash style game that is like Bayonetta. I had thought of a concept for that game and even a simple storyline, but I quickly came to a realization; I didn't have enough time to create assets, animations, scripts etc. for that kind of game. With the time limitation I am under I decided to go with a much simpler game. I wondered what kind of game I can make which will take less time, be fun to play and still be complex enough for it to be accepted. The first thing that popped up in my mind was Minesweeper, I am a little addicted to that game. But then I thought if it's even complex enough, I needed a twist. And BAM! How about it's Minesweeper, but from a first person perspective? Genius!

## Birth of First Person Sweeper

Coming up with a name was easy, it's just a wordplay on FPS, First Person Shooter. As in the name's sake I needed an FPS player controller so I quickly cobbled together a simple FPS rig. It did work, but the movement had small jitters so decided to scratch that and went to Unity Asset Store. I picked [Mini First Person Controller](https://assetstore.unity.com/packages/tools/input-management/mini-first-person-controller-174710) by Simon Pasi as it was the simplest looking one. It had a couple of features I didn't need so I slightly modified the asset to better suit my needs. Now that I had my FPS controller in place, I needed a play area to roam around.

### Minefield Generation

I created a basic tile prefab out of a quad. I used quad instead of a plane because quads have only 4 vertices, meanwhile plane has a far more geometry that I didn't need. I created [`Mine Generator Script`](Assets/Scripts/MineGeneratorScript.cs) and started coding away. I assigned 2 public integers called *mapHeight* and *mapWidth* that will control the size of the minefield. Once I was able to generate a field of tiles, there was nothing except them. So I added another quad named "Terrain", slightly lower that tile's Y position, so they won't cause visual glitches. Script scales it according to minefield's size, also a little more according to *_spaceBeyond* as a buffer, plus it adjust texture scale of the terrain so it's texture aligngs with tile's texture. After the addition of a terrain, player was able to move outside the minefield, so I decided to add semi-invisible walls. 4 pieces of walls, scaled to fit any size of minefield perfectly (I'm not very good at math, so that almost broke me) and encapculate it completely. Then the script picks random coordinates within the minefield and adds a mine (simply makes *IsMine* bool on that tile true), then it checks adjacent tiles and increases their *NearbyMines*, this process occurs until we have enough mines specified in *mines* variable.

### Tile Prefab
To improve tile prefab from a simple quad I created [`Tile Script`](Assets/Scripts/TileScript.cs). Currently it contains a lot of fields, so I gave them all a tooltip to make it easy to digest. *IsCovered* bool checks if tile is covered or not. If it's uncovered, tile's texture changes to display it. All tiles have a *NearbyMines* int which holds the number of mines adjacent to it. To display that number in an FPS environment I used TMP text meshes. Text automatically rotates towards player to make them more visible. Also if they are away certain distance from the player, they fade away completely to gain performance. I created different materials for every situation (Covered, uncovered or exploded) a tile will go through, tile's material changes according to what happened to it.

### Implementing a Player Script
After tile's has been revamped, I needed some extra functionalities for my player. [`Player Script`](Assets/Scripts/PlayerScript.cs) has some of these functionalities, such as *Health* of the player, references to the sub scripts of player and raycasting ability that will be used to get which tile player clicked on. I check all the input through [`Player Input Script`](Assets/Scripts/PlayerInputScript.cs), which can be turned off on demand through main [`Player Script`](Assets/Scripts/PlayerScript.cs). Also a simple damage calculation method that will be used later.

### Mine Controller
Now is the time to control the game. [`Mine Controller Script`](Assets/Scripts/MineControllerScript.cs) takes care of everything happens inside the game. Setting up the minefield, Uncovering tiles and flagging tiles. Speaking of flagging tiles, I modelled a basic flag prefab in [Blender](https://www.blender.org/), imported into the game. While the pole has a basic material, flag itself has a cloth component and uses [Free Double Sided Shaders](https://assetstore.unity.com/packages/vfx/shaders/free-double-sided-shaders-23087) by Ciconia Studio, otherwise it's only visible on one side.

### UI Elements
Next up I created a basic UI, a [`Healthbar Script`](Assets/Scripts/HealthbarScript.cs) that scales the healthbar according to player's health, a [`Counter Script`](Assets/Scripts/CounterScript.cs) that displays remaining mines and a timer, also a [`Minimap Script`](Assets/Scripts/MinimapScript.cs) that both displays a minimap on the corner and when player holds down "Tab" key shows the entire map.

### Mine Goes Boom!
The game's an FPS Minesweeper, you're literally walking around in a minefield, so I wanted to convey the feeling of "One wrong step, and you're done". In that regard I added a collider trigger to the tile prefab. [`Mine Trigger Script`](Assets/Scripts/MineTriggerScript.cs) activates when the player steps on a mine and tells the [`Tile Script`](Assets/Scripts/TileScript.cs) to explode that tile. I used "Big Explosion" in the [Unity Particle Pack 5.x](https://assetstore.unity.com/packages/essentials/asset-packs/unity-particle-pack-5-x-73777) asset by Unity Technologies. Other than explosion visual, both [Bomb Countdown Beeps](https://pixabay.com/sound-effects/bomb-countdown-beeps-6868/) by snakebarney and [Explosion 01](https://pixabay.com/sound-effects/explosion-01-6225/) by tommccann sound effects play in that order. Mine takes 1 second to explode, I wanted to give the player some time to run away, because explosion has a damage fall-off, meaning farther away the player is from the epicenter of the explosion, less damage they take.

### Adding a Main Menu Screen
Now the gameplay part is done, I needed a way to get there, so I started working on the [`Main Menu Script`](Assets/Scripts/MainMenuScript.cs). Script controls every button, setting, sub menus etc. and let's you set-up the game from predefined levels or fully customize it.

### A Game Controller to Manage Flow
To manage scene transitions and managing the flow of both the game and data, I created a [`Game Manager`](Assets/Scripts/GameManager.cs). It's a simpleton, which means it moves through all the scenes without unloading and carries essential data. It also has a loading screen (Which you can't see in the editor because scenes load pretty fast), a pop-up window that has yes/no functions and a fade to black ability that will be used at the end of the game.

### Checking Game's Status

Update of [`Mine Controller Script`](Assets/Scripts/MineControllerScript.cs) constantly checks if all of the mines are either exploded or has been flagged. If that's the case within 5 seconds player moves to "Good" End Scene. However, if player has took enough damage I unlock all of their rotation constraits, so they spin away with the explosive force, their screen fades away slowly - again, within 5 seconds - and they move to "Bad" End Scene.

### Game Over Screen

I just put together a really basic game over screen. On the top of the screen game states either "MINEFIELD CLEAR!" or "YOU ARE DEAD!" with corresponding green or red background. I inform player with their total time and current settings of the minefield they just played. On the bottom 2 buttons move the player either back to main menu or let them restart the game with same settings.

### Adding Audio-Visual Flair
After the game is fully functional, I started working on textures. For tiles I got [Desert Soil Dry Dirt Land Earth](https://pixabay.com/photos/desert-soil-dry-dirt-land-earth-5697545/) by Ramsey Media for the covered material and [Gravel Stones Dirt Road Fixed](https://pixabay.com/photos/gravel-stones-dirt-road-fixed-3388388/) by anaterate for the uncovered material. Both images have been converted through [IMGonline's Seamless Texture Converter](https://www.imgonline.com.ua/eng/make-seamless-texture.php) to make them tilable. I edited them and created height maps in [GIMP](https://www.gimp.org/), and got normal maps from [Smart-Page's Normal Map Generator](https://www.smart-page.net/smartnormal/). Exploded material also shares similar textures with uncovered material, but with dark markings at the center of it, suggesting the explosion that occured. Also created a transparent texture for the aforementioned "semi-invisible walls" in [GIMP](https://www.gimp.org/) as well and used [`Wall Texture Scroll Script`](Assets/Scripts/WallTextureScrollScript.cs) to move them, so they give a hologram-ish vibes.

### First Person Sweeper v0.4.2 Alpha (As of December 31, 2022)
After a month of work on the game, it's finally "finished" enough for me to call it an alpha version (During late stages of the development it was pre-alpha). Thank you for reading through this documentation. Have a wonderful 2023!

### Credits
##### Assets
- [Mini First Person Controller](https://assetstore.unity.com/packages/tools/input-management/mini-first-person-controller-174710) by Simon Pasi.
- [Free Double Sided Shaders](https://assetstore.unity.com/packages/vfx/shaders/free-double-sided-shaders-23087) by Ciconia Studio.
- [Unity Particle Pack 5.x](https://assetstore.unity.com/packages/essentials/asset-packs/unity-particle-pack-5-x-73777) by Unity Technologies.
##### Fonts
- [Monofonto](https://www.dafont.com/monofonto.font?text=01h%3A23m%3A45s%3A67m+%7C+Mines%3A+012) by Typodermic Fonts.
##### Sound Effects
- [Bomb Countdown Beeps](https://pixabay.com/sound-effects/bomb-countdown-beeps-6868/) by snakebarney.
- [Explosion 01](https://pixabay.com/sound-effects/explosion-01-6225/) by tommccann.
- [Man Dying](https://pixabay.com/sound-effects/man-dying-89565/) by Under7dude.
- [068232 Success.wav](https://pixabay.com/sound-effects/068232-successwav-82815/) by pixabay.
- [Badlands, wind, slow, open barren landscape, near stream, ALBERTA, 100315](https://pixabay.com/sound-effects/badlands-wind-slow-open-barren-landscape-near-stream-alberta-100315-18027/) by TRP
##### Images
- [Desert Soil Dry Dirt Land Earth](https://pixabay.com/photos/desert-soil-dry-dirt-land-earth-5697545/) by Ramsey Media.
- [Gravel Stones Dirt Road Fixed](https://pixabay.com/photos/gravel-stones-dirt-road-fixed-3388388/) by anaterate.
##### Programs Used
- [Unity](https://unity.com/) by Unity Technologies.
- [Visual Studio Code](https://code.visualstudio.com/) by Microsoft.
- [Blender](https://www.blender.org/) by Blender Foundation.
- [GIMP](https://www.gimp.org/) by The GIMP Team
- [Audacity](https://www.audacityteam.org/) by Audacity