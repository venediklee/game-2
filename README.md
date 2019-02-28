# game-2
2D topdown game with skills and rage mechanic that awards players that have high kills by returning them to life after dying.

The game is not done yet. Lists of TO-DO's, features to add, bugs etc. are below:

--------------

*TODO*--do before release

• Reload Sounds

• Kit Use Sounds

• Multiplayer Lobby & Server List & Syncronize safe zone

• Smaller Skill Menu

• Fix Network Tick

• Fix Leap(it might be fine after network tick)

• Slow movement by pressing alt key(also disable melee attack)

• New Game Mode: deathmatch--> small map(perhaps + like wall with hole in the middle etc. would be fine)

~~• Fix dropping items(clients can't see them)~~

• Rage GFX

• Increase bullet speed(?)

• Set all doors Z positions to 0

• Reset player stats, skill points etc.

• RpcApplyDamage gave error on client--test it, fix it.

------------------

*BUG LIST*--current bugs & non implemented essential features // (later) means fix after release and/or internship application.

• Can pick up items from other side of the wall if we are too close to the wall

• Ammo GFX needs to be changed after adding new reload mechanics etc.(later)

• Ammo in mag resets after changing guns(later)

• Instant Reload(later)

• Martial Art doesn't work properly while other skills are in cooldown(later)

• Skills don't get removed from GUI when their points are set to 0(later--maybe just remove skill point decreasing)

------------------

*ADD LATER*--features to add after initial release

• Green pot for speed(sprite @ universal bronze pack)

• Crossbow

• Quirk: Eagle Eye: player sees a larger area (also in sniper guns?)

• Skill: Breacher: Completely break walls by damaging them(?)

• Indicator that we have unspent points etc.(also fix the ugly ammo & skill point UI's)

• Skill: Piercing Shots: Bullets pierce through walls

• Change bullet destroy time & speed of individual guns & rarities

• New Mechanic: Mutations: Changes the player character behaves after getting exposed to certain amount of radiation(like claustrophobia, man of stell etc.)

• Show next safezone area(also move the center of the safezone area)

• Ammo types for different types of weapons

• On screen info like -health, + health, +armor, RAGE etc.

• Current player count and winning the game informations

• Jumping from plane to the ground instead of instant spawn on the ground

• Mobile support: Unity Ads & Mobile Input & Mobile Project Settings(lightweight etc.)

• Skill: Tracer: Traces enemies footsteps gunshots etc.

----------------

*PLACE HOLDERS*--place holder objects like sprites etc.

• Rooftops

• Character Sprite

*NOTES*--feedback from testers

• Want faster speed up & slow down(quite possibly increasing network tick will solve this)

• Skill menu has small leaks on edges

• Fix unspent skill points location

• Limit on max skill points

• Remove decreasing skill&quirk points

• Make the cooldown clocks starting rotation angle 90'(unnecassary)

• Make the bush vibrate when a player is inside(this could be hard / later)

• Make the radiation on edges of the map at the start contain red items and give you more chances to mutate also you may scatter radiation through the map(inside the safe zone) for more high risk high reward situtations(later)

• Laser Swords(later)

------------

Also note that there are some todo's within the scripts
