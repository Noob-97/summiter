<img width="1278" height="228" alt="SumitterBANNER" src="https://github.com/user-attachments/assets/a9da6c20-75d4-4e11-b79c-91e2d86dc4f2" />

*Source Code for my submission "summiter" made for the Stop Killing Games game jam.*

# About

Edric has a rare, chronic disease, where each second he doesn't stay in contact with water, his health deteriorates, but he wants to climb up this mountain where his mother lives to see her one last time.

Summiter is a platformer game with 4 levels (but more to be added!) made for the Stop Killing Games community Game Jam, available on itch.io (https://noob-97.itch.io/summiter). This game is still in development for making more levels and fixing bugs people may find and write in the comments section or by sending me a DM on Discord at @notnoob97xd.

Good luck to everyone else in the jam!! The community of this jam was super nice and I'll definitely join the next one if it gets made :D.

# End of Life Plan

OK, let's be honest. This is jam game and I will probably abandon it when the jam ends. This means that Sumitter is going to end its support on Saturday, August 2 for bug fixing, updates and new content.

The Stop Killing Games movement proposes 2 solutions for this. I'll take the second: becoming the game open-source under the MIT license. This allows people to make mods of the game or more content if they really want to by making a fork of the project and publishing it freely (though I doubt someone would really do that lol).

The game will always be downloadable from itch.io. If itch.io goes away, someone else that has downloaded the game (or even me) could distribute it through other source. If necessary, they could even make a new build with the source files.

# Transparency Notes [since v1.0]

All the code for this project has been human-written EXCEPT FOR THIS SINGLE LINE OF CODE because I got too lazy to search it on Google and instead asked it to ChatGPT (I shouldn't have done that ;-;)

rb.AddForce(UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(5f, 15f), ForceMode2D.Impulse);

(https://github.com/Noob-97/summiter/blob/main/Assets/Scripts/CharacterController2D.cs#L241)

The date of creation of this project was 3 days after the game jam started, and therefore, all of the code was made during the jam, except for reused parts of code from other projects of mine linked here (all links refer to github):

- Follower Mode of CharacterController2D.cs is an adapted version of NPCController.cs from the repository "Unityrune" made by XariusExcl.

- Simulated Mode, the GetMovement() and UpdateAnim() functions, and Debug Flying feature from CharacterController2D.cs are based of PlayerMovement.cs and NPCBrain.cs of my repository "tkrpg-files".

- The scripts WaterCollectable.cs and Grab.cs are a shortened and modified version of Bomb.cs and (also) Grab.cs of my repository "multigun_game".

- All the ChromaKey related materials and shader graphs were originally made for a previous game called "Black-Friday", also in my repositories.

- Finally, the Error.png file has been borrowed also from my "tkrpg-files" repo.

There were also entire assets I didn't made at all, but were licensed under the Creative Commons 0, Attribution 4 and other licenses.

// SCRIPTS & ASSETS

- The SpringPref.prefab, WaterSpring.cs, and WaterShapeController.cs are files from the repository "Water2D-Unity" by MemoryLeakHub

- The asset "DOTween" by Demigiant from the Unity Asset Store has been used in this project.

- The asset "PlayerPrefsEditor" by Sabresaurus Ltd from the Unity Asset Store has been used in this project.

// SOUND EFFECTS

- Car motor by enric592 -- https://freesound.org/s/185743/ -- License: Attribution 4.0

- Car Door open and close 3.wav by NachtmahrTV -- https://freesound.org/s/556690/ -- License: Creative Commons 0

- "Video Game Text": Sound Effect by CreatorsHome from Pixabay

- Water Splash in Lake 05.wav by vero.marengere -- https://freesound.org/s/524524/ -- License: Creative Commons 0

- Knock on Door 2 by NachtmahrTV -- https://freesound.org/s/571671/ -- License: Creative Commons 0

- storm wind in trees 1640 2 160221_0857.wav by klankbeeld -- https://freesound.org/s/677646/ -- License: Attribution 4.0

(that's it finally)



