# MicroCosmos
Fun little Unity evolution simulator.

Preview
---
Macro view sub-colonies:
![Macro view sub-colonies](Assets/Captures/Macro%20view%20sub-colonies.gif "Macro view sub-colonies")
Manual controls:
![Manual controls](Assets/Captures/Manual%20controls.gif "Manual controls")
Cell eats and bursts:
![Cell eats and bursts](Assets/Captures/Cell%20eats%20and%20bursts.gif "Cell eats and bursts")
Basic plotting:
![Basic plotting](Assets/Captures/Basic%20plotting.gif "Basic plotting")
Detailed plotting:
![Detailed plotting](Assets/Captures/Detailed%20plotting.gif "Detailed plotting")

TODO
---
 - ~~Divine intervention to recycle fat~~
 - ~~Include chemical blobs in save~~
 - Flagella:
   - ~~Force scales with mass~~
   - ~~Display flagella as ring (color and spin speed indicates relative power)~~
 - Membrane:
   - ~~Display membrane as ring around cell (thickness indicates relative mass)~~
   - As a jumpstart, have cell thickness controlled by a static gene.
 - Orifice:
   - ~~Ability for cells to ingest substances from the environment using it's orifice~~
   - Flow rate is governed by mass of chemical pump substance and indicated by width of pump
   - Absolute membrane thickness limits pump speed
   - Maintaining relative mass of pump is an involuntary reaction
 - Ability for cells to sense nearby obstacles
   - Separate logit for each target type (inert obstacle, cell, chemical blob)
 - Ability for cells to sense other cells
   - ~~Mass by substance~~
   - ~~Distance~~
   - Incoming velocity
   - Degree of separation (genealogically speaking)
   - Cost of sensor is proportional to range
   - Inter-cell communication signals (3 channels visualized a RGB)
 - Ability for cells to sense nearby substances
   - ~~Mass by substance~~
   - Cost of sensor is proportional to range
   - Cost of sensor is proportional to number of substances
 - Ability for cells to sense obstacles like walls
 - Bigger brains
 - Egg laying
 - Saprotrophs
 - Murder
   - Stab
   - Drop poison
 - Plot colony statistics in grapher
   - ~~Cell count~~
   - ~~Chemical blob count~~
   - ~~Total mass~~
   - Global events system: Number of births, deaths and miscarriages and deaths by child birth.
 - ~~Color chemical blobs by contents~~ 
 - Ability to toggle Grapher plots.
 - Save
   - ~~Stenographer streaming file compression~~