# MusicVR

Prototyping an idea for a 360 music sequencer, for VR headsets

Inspired by TiltBrush, I wanted to make a VR music experience where you can see and edit music around you.

For music synthesis, I have utilised and extended [Unity CSharpSynth ](https://forum.unity3d.com/threads/unitysynth-full-xplatform-midi-synth.130104/)

https://vimeo.com/223441066  pw: vr360demo

## Things to do

### Bugs 
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
- Allow portamento synth instruments; allow user to draw a continuous line around themselves
