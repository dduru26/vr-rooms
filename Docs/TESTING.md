# Testing

Status of every requirement: what has been checked in the Unity editor, what has been checked on a Meta Quest 2 headset, and what still needs a run.

**Legend:** ✅ passed · ⏳ not tested yet · ⚠️ known issue

## 1. Checked in the editor

| Area | Check | Result |
|---|---|---|
| Door | Play-mode push on the handle: door swings open and closed through the hinge limits without drifting off the hinge | ✅ opened to +103.7°, closed to −100°, pivot drift 0.000 m |
| Sockets | Mug sits on its coaster, flashlight on its mount at start | ✅ |
| Animated hands | Grip and trigger values drive Open / Grip / Point / Fist poses on both hands | ✅ finger bones change with each pose |
| Scene navigation | Start Menu → VR Room → course room → VR Room → Start Menu through the navigator | ✅ one rig, one interaction manager, one event system and one camera after every load |
| Course room rig | Near-Far interactors and poke interactors present on both hands | ✅ 2 near-far, 2 poke |
| Floors | Every build scene has a floor collider under the player start, so stick walking with gravity cannot fall through | ✅ Start Menu, Donald's Room, VR Room |
| VR Room checklist | Scripted play test drives the room's interactions and checks which tasks tick | ✅ 8 of 12 (teleport, menu, grab, flashlight, mount, main room, chess, mug); walk, turn, door and clock need real input |
| Checklist panel | Pinned to the camera; open in the centre of the view; minimize hides the task rows and leaves a "TASKS x / 12" tab in the lower-left corner; hidden while the Game Menu is open | ✅ |
| Flashlight switch | Trigger while held turns the light on, again turns it off | ✅ |
| Menus | Game Menu Turning / Tasks buttons, start screen Turning button and updated controls text render correctly in every scene | ✅ (visual check) |
| Challenge 2 | Paint stroke is collected onto the turntable, rotates with the slider, Print makes a miniature with shape + stroke | ✅ |
| Challenge 3 | Fixes applied and lighting re-baked; console, labels, stand and progress panel render correctly | ✅ (visual check) |
| Lighting | Course room and Challenge 3 baked without errors | ✅ |
| Wall clock | Hands and digital readout placed in both rooms | ✅ |

## 2. Checked on the headset (Meta Quest 2)

- ✅ **Install and launch:** the APK installs over USB and launches into the Start Menu; no crashes and no Unity errors in the headset log.
- ✅ **Performance:** Start Menu holds 72/72 FPS (GPU ~60%, CPU ~20%); one dip during the first load only.
- ✅ **Head tracking:** the view follows the headset. (First build: the view was frozen in front of the face because the XR Interaction Simulator replaced the real headset; it is now stripped from every build, confirmed in the APK.)
- ✅ **Player height:** floor tracking origin; the Start Menu sits at a normal height.
- ✅ **Locomotion:** walking with the left stick, snap turning with the right stick, teleport with X/A + Grip.
- ✅ **Game Menu:** opens with left Y.
- ✅ **Room interactions:** grab, open the glass door by its handle, walk through to the main room, look at the wall clock, chess piece onto the board, mug back onto its coaster (all ticked by the checklist during a play-through).
- ✅ **VR Room checklist:** tasks tick off with a buzz; the panel opens in the centre of the view at a comfortable size and minimizes to the lower-left corner.
- ✅ **Demo video:** full walkthrough recorded in the headset.

## 3. Still to test

### Core interactions
- ⏳ **Flashlight:** find it on the crate in the back-left corner of the storage room, switch it on with the trigger and hang it back on its mount (the only checklist task not yet confirmed on the headset).
- ⏳ **Filtered sockets:** each socket only accepts its own object (try putting the mug on the flashlight mount, a box on a chess square — both should be rejected).
- ⏳ **Turning setting:** switch to smooth turning with the **Turning** button (Game Menu or start screen), change scene, and check it stays smooth; switch back to snap.

### UI with controllers
- ⏳ Start Menu buttons work with the ray **and** by poking with a finger.
- ⏳ Game Menu: every button works (each room, reset, back to start, Turning, Tasks, close).
- ⏳ Course room Welcome, Reset (B) and Settings panels; Settings toggles really switch snap/smooth turning and distance grab.

### Clock and hands
- ⏳ Wall clock matches the headset's clock (check again a few minutes later); digital time ticks every second.
- ⏳ Hands follow grip/trigger smoothly on real controllers (analog half-press shows a half-curled hand); hands hide while holding an object and come back on release.

### Audio, haptics, activation
- ⏳ Controller vibration on hover and select; click sound on select.
- ⏳ Fireplace sound is 3D (louder as you get closer); room reverb.
- ⏳ TV remote: trigger plays/pauses the video ⚠️ in the editor the video prepared but did not report playing — confirm on the headset.

### Consistency across scenes
- ⏳ After each scene change: grabbing, teleport, menu and hand animation all still work (no lost input after loading).

### Build and performance
- ✅ Meta Quest project settings: OpenXR only (Meta Quest feature, Touch + Touch Plus controller profiles), single-pass instanced, Vulkan, IL2CPP ARM64, min API 32, ASTC, URP 4x MSAA / HDR off / shadows 25 m.
- ✅ Android build completes: `Builds/VRRooms.apk` (~70 MB, 3 scenes, package `com.donaldduru.vrrooms`).
- ⏳ Frame rate holds at 72 Hz in Donald's Room and the VR Room (Meta Quest Developer Hub performance overlay, or the FPS overlay under `INTERFACE` in the course room).
- ⏳ Comfort: fade on scene load, no sudden camera moves, text readable at arm's length.
- ⏳ Toxic Outpost UI theme reads well in the headset (contrast, glow, scanlines not shimmering).

### Challenges (optional for the assignment)
- Challenge scenes run in the editor only; they are not part of the headset build. Their editor checks are in section 1.
