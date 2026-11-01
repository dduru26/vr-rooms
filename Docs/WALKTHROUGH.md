# Demo video and code walkthrough plan

## Demo video (5–7 min, recorded in the headset or with the XR Interaction Simulator)

| # | Shot | Show |
|---|---|---|
| 1 | Start Menu | Lobby, start screen; press a button with the ray and one by poking |
| 2 | Course room | Welcome panel, teleport on the rug and mats, snap/smooth turn from Settings |
| 3 | Course room interactions | Grab the ball and racket, hats on the hooks and your head (socket), TV remote turns the TV on (trigger), fireplace audio |
| 4 | Hands | Close-up of both hands: half grip, full grip, point, fist |
| 5 | Wall clock | Clock next to the headset's own time; digital readout ticking |
| 6 | Game Menu | Left Menu/Y opens it; travel to the VR Room |
| 7 | VR Room | Look around storage room; grab boxes, crane, flashlight |
| 8 | Sockets | Flashlight onto its mount, mug onto the coaster, chess pieces onto their squares; show a wrong object being rejected |
| 9 | Door | Grab the handle, swing the glass door open, walk through, close it |
| 10 | Navigation back | Game Menu → course room → Start Menu, showing everything still works after loading |

## Code walkthrough (5–8 min, screen recording of the editor/IDE)

1. **Project tour** — `Assets/_Project` folder layout, the three scenes in Build Profiles, the shared `XR Setup` rig prefab.
2. **`WallClock.cs`** — `DateTime.Now`; why each hand adds the smaller unit (minutes include seconds/60, hours include minutes/60) so they move smoothly; base rotation × `AngleAxis` around the clock face axis; digital `HH:mm:ss` and date strings.
3. **`HandAnimator.cs`** — `InputActionProperty` for grip and trigger (XRI Select Value / Activate Value); exponential smoothing `1 - e^(-k·dt)`; Animator `Grip`/`Trigger` floats and the 2D blend tree (Open, Grip, Point, Fist).
4. **`SceneNavigator.cs`** — scene name constants, fade overlay built in code, `LoadSceneAsync` in Single mode so each scene brings its own rig and input actions (why interactions don't break after a transition).
5. **`InGameMenu.cs`** — input action created in code with controller and keyboard bindings; placing the menu in front of the camera using a flattened forward vector.
6. **Door** — `HingeJoint` limits, velocity-tracked grab on the handle only, and `IgnoreCollisionsWith.cs` to stop the door catching on the frame and floor tiles.
7. **Sockets and interaction layers** — Flashlight / Chess Piece / Mug layers on the sockets and their objects; Raycasts layer reserved for the teleport ray.
8. **Challenge bonus scripts (optional)** — `PaintTurntable.cs`, `TrainingProgress.cs`, `MagnetSpeedControl.cs`.
