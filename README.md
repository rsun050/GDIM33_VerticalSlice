# GDIM33 Vertical Slice
## Milestone 1 Devlog
1. I made a Visual Scripting Graph for my PlayerController class. The logic-heavy code (such as ground checks, jumping, etc) gets separated into functions in my PlayerController class for easier testing, and then my Update event in the graph simply strings movement functions together for a relatively easy "overall look" at the logic.
2. <img width="1771" height="859" alt="breakdown4" src="https://github.com/user-attachments/assets/22533b85-fd5d-499d-be20-88be0f1ccd7d" /> I added the state machine I made for the player's animations. The state machine is still relatively simple with only three states - idle, walking, and falling. The PlayerController triggers animation state transitions by setting bools related to the player's own movement (if moving, set Moving to true, if not touching ground, set InAir to true, etc). In the future, this state machine will likely also include animation states for taking damage and dying.

## Milestone 2 Devlog
1. Complicating gameplay feature: the player's sticky hand not only looks silly and goofy, but it also allows for interaction with items at a distance (picking things up, throwing them).
  - Step 1: the sticky hand follows the mouse up to a certain distance from the player (cannot travel infinitely far from the player)
    - 1-1: The sticky hand follows the mouse
    - 1-2: The sticky hand moves with and turns with the player
    - 1-3: The sticky hand has an arm that connects it to its anchor on the player's body
  - Step 2: the sticky hand can pick up and drop items
    - 2-1: The sticky hand can pick up an item - the item must follow the hand's position and rotation
    - 2-2: The item's physics must be changed (eg: not fall, have different collisions convenient for gameplay, etc)
    - 2-3: The sticky hand can drop the item. The item must return to its original physics pre-pickup.
  - Step 3: the sticky hand can throw items
    - 3-1: The sticky hand can be locked in place and 'aimed'
    - 3-2: Items can be thrown rather than simply dropped, and are thrown in the aimed direction
    - 3-3: Some thrown items act as projectiles (eg: thrown rocks, nerf bullets), and can trigger items like targets
2. Honestly I forgot I wrote them down here, but it stayed subconsciously in my mind a little bit? Which is better than not thinking it through at all, I guess. I don't think there's much I can improve about my breakdown other than remember to actually look at them.
3. I'm not a fan of complex graphs since they're much harder for me to follow. So I broke down some of my movement logic for the player into smaller subfunctions, then punched my subfunctions into a graph. It's actually a bit helpful for me to have the subfunctions as nodes in a scripting graph because I can dynamically modify the edges at runtime (during testing) to see how it affects the player movement, which I had some trouble with during Milestone 1. If I had done this with C# code, I would have had to recompile between changes, slowly things down drastically.
<img width="1210" height="445" alt="image" src="https://github.com/user-attachments/assets/a22364f9-0c56-4db4-ab40-32f65eaa6f8a" />
4. Animator, it's on the player and the enemy (kill the enemy for a secret super special sprite!!!)

## Milestone 3 Devlog
1. My shader gives a pinkish pulse to interactable items that the player is close enough to interact with. The color part is not that complicated, just a "multiply texture using this color over time". The annoying part was the Sample Texture 2D thinking my very transparent sprite was very un-transparent for some reason, so I had to use the Alpha channel to make sure transparent pixels didn't show up as black pixels.
<img width="1920" height="1080" alt="2026-05-28 22-58-14" src="https://github.com/user-attachments/assets/5fc56ffb-5b9c-4558-90f5-f29e9800cc71" />

2. I improved the gun - it should no longer fire backwards at weird angles. I also received feedback that putting the keyhole not on the door makes it appear that the keyhole must be interacted with rather than the door itself, so I moved the keyhole onto the door.

3. I added level transitions - rather than the game being one big level, the player can now move between levels, which is the most important part to add to a 2D platformer, 'closing the gameplay loop' by allowing a player to continuously beat levels and move on. In other news, an enemy now exists and is a tangible threat, attacking the player. It can also be killed. I also added a DOOM-style crusher, because those are cool. It also kills the player.

## Final Devlog
1. Core gameplay loop: beat a level and move to the next. Levels get more complex as the player proceeds to them, starting from simple movement to puzzle mechanics with the picking up and using of items, and enemies and hazards appear in later levels. This illustrates the various mechanics of the game and how the ramping up of difficulty might look in the final game with more content.

2. My rendering effect is activated when the player's sticky hand is close enough to (touches the Collider2D trigger of) an item that they can pick up or interact with. It highlights the item of interest with a glowing pinkish flash that repeats itself until the player's hand moves away again. The flash is triggered by moving the item onto a layer called ItemOutline. This was actually an interesting problem for me since I also needed the layer for tracking item collisions with the player's sticky hand, so there is a layer called Item (no flash) and another called ItemOutline (with flash), both of which interact with the player's hand.
<img width="1920" height="1080" alt="2026-05-28 22-58-14" src="https://github.com/user-attachments/assets/5fc56ffb-5b9c-4558-90f5-f29e9800cc71" />

3. When breaking a project down, I think of the important parts - for this project, it the big parts were the player's sticky hand, the items, and the interactable systems (levers, targets, and the things that they activate). I plan out what they do, what they need to do what they do, and how they interact with each other.

  - I think I do plan to use the task breakdown system in future projects, as it's like making a plan before you act, and it will probably serve as a good checklist during development, and a reference afterwards to review what went well, what went poorly.
  - Breaking down a project into smaller steps often makes me get a better picture of the scope, since I can see all (or at least more of) the work that will need to go into the project. This usually makes me realize if I'm overscoping or not.
  - I put thought into the systems, but my view was too broad, and some of the programming details fell into the cracks, which led to some hardcoding of the systems, which kind of sucked. In the future I'd want to go into more detail on the design of my classes, variables, and functions, although I also worry about then getting too lost in the details before even starting development.

## Open-source assets
Sounds and music: scratch.mit.edu, bfxr, Kevin Macleod
