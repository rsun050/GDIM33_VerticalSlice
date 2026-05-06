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

## Milestone 3 Devlog
Milestone 3 Devlog goes here.
## Milestone 4 Devlog
Milestone 4 Devlog goes here.
## Final Devlog
Final Devlog goes here.
## Open-source assets
- Cite any external assets used here!
