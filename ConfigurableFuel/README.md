# Configurable Fire
This mod adds configurability to Fires.

## Specifications
* Fires supported - FirePit, Bonfire Hearth, Standing Torches (Wood, Iron and Green), Sconce and Brazier.
* Configure Max and Start Count, Type and Burn Duration of Fuel for Fires.
* Disable use the need for Fuels.
* Extinguish and re-light Fires.
* Modifier Key (Default: G) for toggling Fires.

## Installation
1. Install BepInEx
2. Unzip `ConfigurableFire.zip`
3. Copy `ConfigurableFire.dll` to `Valheim\BepInEx\plugins`

## Configuration
* Config file will auto generate after Valheim is run for the first time after adding the Mod.
* Or additionally download add the .cfg to `.\BepInEx\config\goonlou.ConfigurableFire.cfg`.  

~~~ini
## Settings file was created by plugin Configurable Fire v0.1.0
## Plugin GUID: goonlou.ConfigurableFire

[00_General]

## Allow all fires to burn without fuel
# Setting type: Boolean
# Default value: false
all_Nofuel = false

## Allow all fires to be extinguishable
# Setting type: Boolean
# Default value: true
ExtinguishableFires = true

## Modifier key to toggle fires on and off. Use https://docs.unity3d.com/Manual/ConventionalGameInput.html
# Setting type: String
# Default value: G
toggleFireKey = G

## Enable this mod
# Setting type: Boolean
# Default value: true
Enabled = true

[01_fire_pit]

## Allow Fire Pit to burn without fuel
# Setting type: Boolean
# Default value: false
fire_pit_NoFuel = false

## Fuel type for Fire Pit. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table
# Setting type: String
# Default value: Wood
fire_pit_FuelType = Wood

## Maximum fuel level for Fire Pit
# Setting type: Single
# Default value: 10
fire_pit_MaxFuel = 10

## Start fuel level for Fire Pit
# Setting type: Single
# Default value: 1
fire_pit_StartFuel = 1

## Time for Fire Pit to burn 1 fuel (sec)
# Setting type: Single
# Default value: 5000
fire_pit_FuelTimeToBurn = 5000

[02_bonfire]

## Allow Bonfire to burn without fuel
# Setting type: Boolean
# Default value: false
bonfire_NoFuel = false

## Fuel type for Bonfire. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table
# Setting type: String
# Default value: Wood
bonfire_FuelType = Wood

## Maximum fuel level for Bonfire
# Setting type: Single
# Default value: 10
bonfire_MaxFuel = 10

## Start fuel level for Bonfire
# Setting type: Single
# Default value: 0
bonfire_StartFuel = 0

## Time for Bonfire to burn 1 fuel (sec)
# Setting type: Single
# Default value: 5000
bonfire_FuelTimeToBurn = 5000

[03_hearth]

## Allow Hearth to burn without fuel
# Setting type: Boolean
# Default value: false
hearth_NoFuel = false

## Fuel type for Hearth. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table
# Setting type: String
# Default value: Wood
hearth_FuelType = Wood

## Maximum fuel level for Hearth
# Setting type: Single
# Default value: 20
hearth_MaxFuel = 20

## Start fuel level for Hearth
# Setting type: Single
# Default value: 0
hearth_StartFuel = 0

## Time for Hearth to burn 1 fuel (sec)
# Setting type: Single
# Default value: 5000
hearth_FuelTimeToBurn = 5000

[04_wood_torch]

## Allow Standing Wood Torch to burn without fuel
# Setting type: Boolean
# Default value: false
wood_torch_NoFuel = false

## Fuel type for Standing Wood Torch. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table
# Setting type: String
# Default value: Resin
wood_torch_FuelType = Resin

## Maximum fuel level for Standing Wood Torch
# Setting type: Single
# Default value: 4
wood_torch_MaxFuel = 4

## Start fuel level for Standing Wood Torch
# Setting type: Single
# Default value: 2
wood_torch_StartFuel = 2

## Time for Standing Wood Torch to burn 1 fuel (sec)
# Setting type: Single
# Default value: 10000
wood_torch_FuelTimeToBurn = 10000

[05_iron_torch]

## Allow Standing Iron Torch to burn without fuel
# Setting type: Boolean
# Default value: false
iron_torch_NoFuel = false

## Fuel type for Standing Iron Torch pit. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table
# Setting type: String
# Default value: Resin
iron_torch_FuelType = Resin

## Maximum fuel level for Standing Iron Torch pit
# Setting type: Single
# Default value: 6
iron_torch_MaxFuel = 6

## Start fuel level for Standing Iron Torch pit
# Setting type: Single
# Default value: 2
iron_torch_StartFuel = 2

## Time for Standing Iron Torch pit to burn 1 fuel (sec)
# Setting type: Single
# Default value: 20000
iron_torch_FuelTimeToBurn = 20000

[06_green_torch]

## Allow Standing Green Torch to burn without fuel
# Setting type: Boolean
# Default value: false
green_torch_NoFuel = false

## Fuel type for Standing Green Torch. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table
# Setting type: String
# Default value: Guck
green_torch_FuelType = Guck

## Maximum fuel level for Standing Green Torch
# Setting type: Single
# Default value: 6
green_torch_MaxFuel = 6

## Start fuel level for Standing Green Torch
# Setting type: Single
# Default value: 2
green_torch_StartFuel = 2

## Time for Standing Green Torch to burn 1 fuel (sec)
# Setting type: Single
# Default value: 20000
green_torch_FuelTimeToBurn = 20000

[07_sconce]

## Allow Sconce to burn without fuel
# Setting type: Boolean
# Default value: false
sconce_NoFuel = false

## Fuel type for Sconce. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table
# Setting type: String
# Default value: Resin
sconce_FuelType = Resin

## Maximum fuel level for Sconce
# Setting type: Single
# Default value: 6
sconce_MaxFuel = 6

## Start fuel level for Sconce
# Setting type: Single
# Default value: 2
sconce_StartFuel = 2

## Time for Sconce to burn 1 fuel (sec)
# Setting type: Single
# Default value: 20000
sconce_FuelTimeToBurn = 20000

[08_brazier]

## Allow Brazier to burn without fuel
# Setting type: Boolean
# Default value: false
brazier_NoFuel = false

## Fuel type for Brazier. Swap with any Prefab Name https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table
# Setting type: String
# Default value: Coal
brazier_FuelType = Coal

## Maximum fuel level for Brazier
# Setting type: Single
# Default value: 5
brazier_MaxFuel = 5

## Start fuel level for Brazier
# Setting type: Single
# Default value: 1
brazier_StartFuel = 1

## Time for Brazier to burn 1 fuel (sec)
# Setting type: Single
# Default value: 20000
brazier_FuelTimeToBurn = 20000
~~~

## Changelog
* v0.1.1: Added support for additional fires, optimise toggle click
* v0.1.0: Initial Release
* v0.0.1: Rename Mod from ConfigurableFuel to ConfigurableFire

## Planned Future Updates
* Wet check for torches.
* Support for any DLC Fires.
* Bug fixes.
