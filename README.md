# GDIM33 Vertical Slice
## Milestone 1 Devlog
1. I made a Visual Scripting Graph for my PlayerController class. The logic-heavy code (such as ground checks, jumping, etc) gets separated into functions in my PlayerController class for easier testing, and then my Update event in the graph simply strings movement functions together for a relatively easy "overall look" at the logic.
2. <img width="1771" height="859" alt="breakdown4" src="https://github.com/user-attachments/assets/22533b85-fd5d-499d-be20-88be0f1ccd7d" /> I added the state machine I made for the player's animations. The state machine is still relatively simple with only three states - idle, walking, and falling. The PlayerController triggers animation state transitions by setting bools related to the player's own movement (if moving, set Moving to true, if not touching ground, set InAir to true, etc). In the future, this state machine will likely also include animation states for taking damage and dying.

## Milestone 2 Devlog
Milestone 2 Devlog goes here.
## Milestone 3 Devlog
Milestone 3 Devlog goes here.
## Milestone 4 Devlog
Milestone 4 Devlog goes here.
## Final Devlog
Final Devlog goes here.
## Open-source assets
- Cite any external assets used here!
