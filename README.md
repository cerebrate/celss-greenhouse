celss-greenhouse
================

A life-support system mod for Kerbal Space Program.

Requires
--------

* TAC Life Support, to provide the life support system this plugs into. NOTE: As of 0.3, this is a hard dependency - celss-greenhouse requires the TAC-LS plugin in order to function. Installing this without also installing TAC-LS will crash harder than Jeb with a case of vodka and a barrel of ClF3. You have been warned!
* KSP Interstellar (optionally), which provides the resources ISRU mode uses and means to harvest them. It also provides reactors which may be a more practical way of handling the high power requirements of these greenhouses than extensive solar panel arrays.

WARNING: *If* you are using TAC-LS lower than prelease 0.9, then this release uses the KPSI/TAC-LS compatibility mod that recalibrates TAC-LS in liters. (In 0.9 and above, it uses this natively, so ignore this section.)

* If you don't want this, you'll need to go into both part.cfg files, rename Water to LqdWater in the ISRU mode input sections and edit the quantities for CarbonDioxide, WasteWater, Waste, Oxygen, Water, and Food back to 0.9 and 1.0, appropriately. DON'T do this for Ammonia or LqdWater, however.
* If you DO want this and don't have it, you can grab a copy here: https://gist.github.com/cerebrate/0d6d4366f29684dd39f3

What It Is
----------

Well, it's a greenhouse, to close the life-support circle by transforming CarbonDioxide back into Oxygen, WasteWater
back into Water, and Waste ("flush twice, it's a long way to the galley!") back into Food. (At least as long as your
kerbals don't mind a somewhat monotonous diet consisting primarily of algae and tilapia.) Alternatively, it can process CarbonDioxide, Ammonia and Water collected in-situ into new life support resources.

The basic greenhouse has equivalent efficiency to the TAC-LS mechanical recyclers (90%). The more advanced CELSS greenhouse does its recycling with a nominal 100% efficiency, so long as it's supplied with light. In theory, so long as you have the right number of these, and keep them powered (to run the pumps and lights), you can keep going on the same amount of oxygen, food, and water forever. (In practice... well, try not to lose any to venting, accidents, or kerbonauts lost on EVA, 'kay?)

Regarding the right number - a standard or CELSS greenhouse can process enough CarbonDioxide, WasteWater, and Waste (or alternative ISRU resources) to keep two kerbals fed, watered, breathed, and cleaned up after (with TAC-LS default settings). At the moment, more kerbals means, you guessed it, more greenhouses. An Agricultural Greenhouse can provide enough for six.

How To Use It
-------------

Just install it on your ship or station, and activate it when you want it to start processing waste or harvested resources.

For those playing in career mode, the 90% efficient basic greenhouse unlocks with "Large Electrics", the nominally 100% efficient CELSS greenhouse unlocks with "Advanced Science Tech", and the larger Agricultural Greenhouse unlocks with "Very Heavy Rocketry".

Where To Get It
---------------

You can download 0.4 (beta) from the Releases section of this GitHub repository. Unzip the file and place the included folder inside the folder of your KSP installation.

In Future Versions
------------------

* A spirulina-based mini-greenhouse.
* Power consumption dependent on sunlight availability.
* Diverting output to other uses.

Thanks
------

Many thanks to Zzz for making the greenhouse model available, and to ArcFurnace for much research work on the applicable numbers.
