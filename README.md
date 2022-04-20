# TraderGen

[![RimWorld](https://img.shields.io/badge/RimWorld-1.3-informational)](https://rimworldgame.com/) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.1-4baaaa.svg)](CODE_OF_CONDUCT.md)

TraderGen is a mod for the [RimWorld](https://rimworldgame.com/) game. It modifies the random generation of stock for traders, increasing stock variety and customization while improving mod compatibility.

Currently most of the features of the mod are only implemented for orbital traders, but expansions are planned for the future.

## Features


Orbital traders now have an specialization in addition to their type. The specialization adds a limited amount of stock but it may be particularly rare or useful. Each trader type has its own set of available specializations, with some of them being less common than the others.

All vanilla orbital traders (and several modded ones) have some changes to their base stock aiming for increasing their variety and improving compatibility with mods.

## Give me the details!

### Specializations

**Bionics:** Adds prostheses between the industrial and ultratech technology levels. May rarely have a single archotech bionic in stock.

### Orbital trader changes

**Bulk goods:** May have any alcohol type in stock instead of only beer. Increased variety and quantity of textiles in stock. Never has human leather in stock by default.

**Combat:** Guaranteed to have a few pieces of power armor in stock. May rarely have a shield belt or smokepop belt in stock.

**Exotic:** Very cheap and very expensive bionics are less common. Ultratech bionics are unfrequent. Increased variety of joy buildings in stock.

**Slaver:** May have any alcohol type in stock instead of only beer. Has more variety of gluttonous food instead of just chocolate. Very cheap and very expensive bionics are less common. Ultratech bionics are unfrequent.

**Imperial:** Increased variety of joy buildings in stock. Has some gluttonous food in stock.

Some orbital trader types added by mods have received changes as well. In the cases of traders with limited stock or having way less total wealth than vanilla traders, an effort has been made to add stock which makes sense to them. Check the Mod support section for details.  

#### Mod support

TraderGen should support wares from most mods out of the box; please submit a [bug report](CONTRIBUTING.md) if you find any issues.

TraderGen also provides extra features when certain mods are enabled. TraderGen includes extra content for the following mods:  

[Alpha Animals](https://steamcommunity.com/sharedfiles/filedetails/?id=1541721856):

* ...

[Expanded Prosthetics and Organ Engineering](https://steamcommunity.com/sharedfiles/filedetails/?id=725956940)

* ...
  
[Expanded Prosthetics and Organ Engineering - Forked](https://steamcommunity.com/sharedfiles/filedetails/?id=1949064302):

* Traders with the bionic specialization may rarely have MA-AI chips in stock.

[Jewelry](https://steamcommunity.com/workshop/filedetails/?id=2020964421):

* ...

[RimBees](https://steamcommunity.com/sharedfiles/filedetails/?id=1558161673):

* ...

[Trader Ships](https://steamcommunity.com/sharedfiles/filedetails/?id=2046222331):

* Trader ship stock is generated using the TraderGen rules for orbital traders.

[Vanilla Apparel Expanded — Accessories](https://steamcommunity.com/sharedfiles/filedetails/?id=2521176396):

* Bulk good orbital traders can have tool belts in stock.
* Combat orbital traders may rarely have a ranged shield belt in stock.

[Vanilla Armour Expanded](https://steamcommunity.com/workshop/filedetails/?id=1814988282):

* Combat orbital traders may rarely have the new power armor types in stock.

[Vanilla Brewing Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=2186560858):

* TraderGen distinguishes between alcoholic beverages, non-alcoholic beverages and other drugs when appropriate.

[Vanilla Brewing Expanded - Coffees and Teas](https://steamcommunity.com/sharedfiles/filedetails/?id=2275449762):

* Orbital traders no longer can have cannibal coffee on stock by default. Morally flexible traders will not have such qualms.

[Vanilla Books Expanded](https://steamcommunity.com/workshop/filedetails/?id=2193152410):

* ...

[Vanilla Cooking Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=2134308519):

* Bulk goods orbital traders will sell canned food instead of raw food when this mod is enabled.

[Vanilla Factions Expanded - Insectoids](https://steamcommunity.com/sharedfiles/filedetails/?id=2149755445):

* ... 

[Vanilla Factions Expanded - Mechanoids](https://steamcommunity.com/sharedfiles/filedetails/?id=2329011599):

* ...

[Vanilla Factions Expanded - Pirates](https://steamcommunity.com/sharedfiles/filedetails/?id=2723801948):

* Combat orbital traders may rarely have privateer armor in stock.

[Vanilla Factions Expanded - Settlers](https://steamcommunity.com/sharedfiles/filedetails/?id=2052918119):

* ...

[Vanilla Furniture Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=1718190143):

* Orbital traders carrying joy buildings will include items from this mod in their stock.

[Vanilla Furniture Expanded - Security](https://steamcommunity.com/workshop/filedetails/?id=1845154007):

* ...

[Vanilla Weapons Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=1814383360):

* Bulk good orbital traders can have tools in stock.
* Orbital traders will never generate tools when they are meant to generate weapons.

[Vanilla Weapons Expanded - Grenades](https://steamcommunity.com/sharedfiles/filedetails/?id=2194472657):

* ...

[Vanilla Weapons Expanded - Non-Lethal](https://steamcommunity.com/sharedfiles/filedetails/?id=2454918354):

* Slaver orbital traders may have some non-lethal weapons in stock.

## Development

To compile this mod on Windows, you will need to install the [.NET Framework 4.7.2 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472). On Linux the packages you need vary depending on your distribution of choice.

Dependencies are managed using NuGet. Compiling this project does not require any [external dependencies or extra setup steps](https://ludeon.com/forums/index.php?topic=49914.0).

## Contributions

This project encourages community involvement and contributions. Check the [CONTRIBUTING](CONTRIBUTING.md) file for details. Existing contributors can be checked in the [contributors list](https://gitlab.com/joseasoler/tradergen/-/graphs/main).

## License

This project is licensed under the MIT license. Check the [LICENSE](LICENSE) file for details.

## Acknowledgements

Read the [ACKNOWLEDGEMENTS.md](ACKNOWLEDGEMENTS.md) file for details.
