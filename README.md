# **Silent Steps**

A top-down, real-time stealth-puzzle game for PC, built in Unity 6

Developer: Rohit Khatri

---

## **About the Game**

Sneak through the room, avoid the guards, and reach the green extraction terminal. Guards chase you if they spot you, so use cover and the Flashbang to survive.

The twist is that the usual stealth rule is reversed. A guard that spots you rushes in and keeps hitting you while you stand still. The moment you start running, it loses interest and ignores you for 2.5 seconds. Standing still is dangerous, and moving is how you escape.

## **Objective**

- Reach the green extraction terminal to win.
- Each guard hit takes 10 health from your 100. You lose when health reaches 0.
- Walls and barrels block both movement and the guards' line of sight, so hiding is a real option.

## **Controls**

| Key | Action |
|---|---|
| W / A / S / D | Move |
| E | Flashbang (freezes nearby guards) |

## **Features**

- Guard AI: Each guard is a state machine with four states, shown by colour:
  - Idle (white): waits until you come within 3 units with a clear line of sight.
  - Chasing (red): slides toward you and hits for 10 damage within 1.5 units, repeating every 0.5 seconds.
  - Ignoring (grey): ignores you after you run or hide.
  - Frozen (cyan): stopped by the Flashbang.
- Line of sight: A raycast from the guard to the player checks the obstacle layer, so cover works.
- Flashbang: Press E to freeze every guard within 6 units for 5 seconds. It has a 15 second cooldown shown on the HUD.
- HUD: Health bar, health text, Flashbang cooldown, and Victory and Game Over screens with a Restart button.
- Audio: Looping background music and a hit sound effect.
- Performance: About 120 FPS in the Windows build (lowest reading 118 FPS) on an i5 9400F, GTX 1660 Ti and 16 GB RAM.

## **Project Structure**

The game uses a single scene. It starts immediately and ends in either a Victory or a Game Over screen. The Restart button reloads the scene.

Key scripts:

- GameManager: game state (Playing, GameOver, Victory), player health, health UI, end screens and hit sound.
- PlayerMovement: WASD movement, wall collision and the Flashbang.
- EnemyGuard: guard AI (detection, chasing, attacking, ignoring, frozen).
- ExtractionTerminal: trigger that calls the Victory screen when the player touches it.

## **Tuning**

Guard behaviour can be adjusted in the Inspector on each Enemy Guard (detection range, hit rate, ignore time) to make the game easier or harder.

## **Tools and Assets**

- Engine: Unity 6, C#
- Input: Unity's new Input System
- UI: Unity UI and TextMeshPro
- Environment: Hyper-Casual Obstacle pack (Unity Asset Store)
- Character: Player model from Mixamo
- Audio: Background music and collision sound effect from Pixabay
- Code editor: VS Code

## **How to Run**

Play the build: Open the `Build` folder and run the `.exe`. Keep all the files in that folder together.

Open the project: Add the project folder in Unity Hub and open it with Unity 6. The first load takes a few minutes while Unity regenerates the `Library` folder. Open the scene and press Play.
