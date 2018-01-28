# MusicVR

Prototyping VR music sequencer; create and watch music in VR.

![demo](https://github.com/paulmriordan/MusicVR/raw/master/musicvrdemo.gif "demo")

[Demo video](https://vimeo.com/223441066)

## Requirements
- Oculus Rift
- Unity 2017.1.3f 

## Libraries used

- For music synthesis, I have used and modified [Unity CSharpSynth ](https://forum.unity3d.com/threads/unitysynth-full-xplatform-midi-synth.130104/)
- For VR interaction,  I have used and extended [VRTK](https://assetstore.unity.com/packages/tools/vrtk-virtual-reality-toolkit-vr-toolkit-64131/)

## Things to do

### Bugs 
- Reloading can cause zombie button to be left in scene
- HDR effect not working on Oculus
- Instrument GUI buttons (scale, instrument select) should be pushed back
- Pointer raycast should visually hit left hand UI buttons
- Wall drag colliders must be EXACTLY at cylinder radius to work, this should be made more robust
- Drag extents must change dynamically; configurable joint limits must be set when height changes
- Icons for saving/loading are not clear

### Future goals
- Review UX
	- instrument selection, scale selection, tempo, num rows/cols should, saving, loading
- Art pass (UI, skybox, sequencer buttons... everything)
- Configurable rythm swing level 
- Improve visualization; better buttons assets, skybox, add other visual effects 
- Loading custom SFZ instruments
- Importing/exporting midi projects; allow users to edit composition externally, then experience them in VR
- Sharing of compositions between users
- Allow portamento synth instruments; allow user to draw a continuous line
