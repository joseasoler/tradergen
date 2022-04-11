# TraderGen

[![RimWorld](https://img.shields.io/badge/RimWorld-1.3-informational)](https://rimworldgame.com/) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.1-4baaaa.svg)](CODE_OF_CONDUCT.md)

TraderGen is a mod for the [RimWorld](https://rimworldgame.com/) game. It overhauls the random generation of stock in orbital traders to increase their stock variety while generating logical combinations.

Orbital traders from mods not supported by TraderGen will appear normally. Orbital traders coming from vanilla or mods with explicit support (see Mod support section below) will no longer appear. Their wares have been added to the orbital traders created by TraderGen.

## Features

* Random generation of orbital trader stock using ship specializations and ship size.
* Customizable amount of silver for orbital traders.
* Appropriate traders may have psylink neuroformers in stock (must be enabled using mod settings, requires Royalty).

## How it works

Stock is grouped in loosely defined categories. For example, an orbital trader with the Medicine category will always have different types of medicine, neutroamine and related goods in stock.  Each category may in turn have different specializations.

An orbital trader can have one or more categories, depending on their size. Small orbital traders have just one, medium traders have two, and large traders have three. Uncommon orbital traders do not follow category rules and have their stock specifically defined instead.

Certain categories become less common as ship size increases, as larger ships tend to be less specialized.

## Mod support

TraderGen should support wares from most mods out of the box; please submit a [bug report](CONTRIBUTING.md) if you find any issues.

TraderGen also provides extra features when certain mods are enabled, including specific trader types, stock combinations and other tweaks to improve the experience. TraderGen includes extra content for the following mods:  

[Alpha Animals](https://steamcommunity.com/sharedfiles/filedetails/?id=1541721856):

* Medicine orbital traders sometimes carry a limited stock of cactipine quills.
* Traders with insectoid genomes from Vanilla Factions Expanded - Insectoids have Black Hive genome in stock when appropriate.
* Traders purchasing insectoid animal products or gluttonous foods will also purchase Black Hive insect jelly.
* Uncommon biomaterials orbital trader.

[Expanded Prosthetics and Organ Engineering](https://steamcommunity.com/sharedfiles/filedetails/?id=725956940)

* The prosthesis orbital trader no longer appears, but the role it fulfilled is now split between different orbital trader categories and specializations.
* The textiles trader is now a rare type of uncommon orbital trader. Other orbital trader categories or specialities carry synthread and hyperweave more frequently.
  
[Expanded Prosthetics and Organ Engineering - Forked](https://steamcommunity.com/sharedfiles/filedetails/?id=1949064302):

* The prosthesis orbital trader no longer appears, but the role it fulfilled is now split between different orbital trader categories and specializations.
* The textiles trader is now a rare type of uncommon orbital trader. Other orbital trader categories or specialities carry synthread and hyperweave more frequently.
* Tech and bionic orbital traders may have MA-AI chips in stock.
* Combat and body part orbital traders may have optimizing nanobots and instinct optimizing nanobots in stock.

[Jewelry](https://steamcommunity.com/workshop/filedetails/?id=2020964421):

* Orbital traders always purchase jewelry.
* Orbital traders may sometimes be specialized on items created from gemstones.
* Transhumanist orbital traders may have eltex jewelry in stock.

[RimBees](https://steamcommunity.com/sharedfiles/filedetails/?id=1558161673):

* Food orbital traders always have temperate and mild bees in stock.
* Orbital traders may sometimes be specialized on craft created from different wax types. These traders will also have some temperate and mild bees in stock.

[Trader Ships](https://steamcommunity.com/sharedfiles/filedetails/?id=2046222331):

* Trader ship stock is generated using the TraderGen rules for orbital traders.

[Vanilla Apparel Expanded — Accessories](https://steamcommunity.com/sharedfiles/filedetails/?id=2521176396):

* Ranged shield belts now appear along with regular shield belts in orbital trader stock.
* Hunting supplies orbital traders may have quivers in stock.
* Medicine orbital traders can have medic bags in stock.
* Tech orbital traders can have tool belts in stock.

[Vanilla Armour Expanded](https://steamcommunity.com/workshop/filedetails/?id=1814988282):

* Hunting supplies orbital traders may have ghillie gear in stock.
* Combat orbital traders can have the new power armor types in stock.

[Vanilla Brewing Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=2186560858):

* TraderGen distinguishes between alcoholic beverages, non-alcoholic beverages and other drugs when appropriate.

[Vanilla Brewing Expanded - Coffees and Teas](https://steamcommunity.com/sharedfiles/filedetails/?id=2275449762):

* Orbital traders no longer can have cannibal coffee on stock by default. Morally flexible traders will not have such qualms.

[Vanilla Books Expanded](https://steamcommunity.com/workshop/filedetails/?id=2193152410):

* All orbital traders may have a couple of books in stock.
* Culture orbital traders will have books, blueprints and maps in stock. You can purchase the newspaper of their ship as well.
* Tech orbital traders will have blueprints in stock.

[Vanilla Cooking Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=2134308519):

* All orbital traders purchase long-shelf life food such as canned food and cheese.
* Food orbital traders have canned food in stock, and purchase condiments. They will also have different food items from this mod depending on their specialization.
* Food and medicine orbital traders may have digestible resurrector nanites in stock.
* Traders purchasing insectoid animal products always purchase insect jelly preserves.

[Vanilla Factions Expanded - Insectoids](https://steamcommunity.com/sharedfiles/filedetails/?id=2149755445):

* Orbital traders will sometimes be specialized in insectoid animal products, chitin or combat insectoids.
* Orbital traders specialized in insectoid wares will have relevant insectoid genomes and, sometimes bio-engineering incubators in stock.
* Uncommon trader specialized in all kinds of insectoid wares, vat-grown insectoids and insectoid bio-engineering.
* The bio-engineering supplier no longer appears, but as mentioned above all of the wares it had still appear in other orbital traders. 

[Vanilla Factions Expanded - Mechanoids](https://steamcommunity.com/sharedfiles/filedetails/?id=2329011599):

* Bulk and Tech orbital traders have mechanoid components in stock.
* High tech uncommon orbital traders have mechanoid components in stock.

[Vanilla Factions Expanded - Pirates](https://steamcommunity.com/sharedfiles/filedetails/?id=2723801948):

* Ordnance orbital traders sometimes have field guns in stock.

[Vanilla Factions Expanded - Settlers](https://steamcommunity.com/sharedfiles/filedetails/?id=2052918119):

* Hunting supplies orbital traders may have hunting rifles in stock.

[Vanilla Furniture Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=1718190143):

* Orbital traders carrying joy furniture will include items from this mod in their stock.

[Vanilla Furniture Expanded - Security](https://steamcommunity.com/workshop/filedetails/?id=1845154007):

* Ordnance orbital traders sometimes have artillery in stock.

[Vanilla Weapons Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=1814383360):

* Tools will not be generated in place of regular weapons, but orbital traders may have tools relevant to their specializations in stock.
* Hunting supplies orbital traders may have compound bows in stock.

[Vanilla Weapons Expanded - Grenades](https://steamcommunity.com/sharedfiles/filedetails/?id=2194472657):

* Ordnance orbital traders will have some grenades in stock.

[Vanilla Weapons Expanded - Non-Lethal](https://steamcommunity.com/sharedfiles/filedetails/?id=2454918354):

* Hunting supplies orbital traders may have dart guns in stock.

## Development

To compile this mod on Windows, you will need to install the [.NET Framework 4.7.2 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472). On Linux the packages you need vary depending on your distribution of choice.

Dependencies are managed using NuGet. Compiling this project does not require any [external dependencies or extra setup steps](https://ludeon.com/forums/index.php?topic=49914.0).

## Contributions

This project encourages community involvement and contributions. Check the [CONTRIBUTING](CONTRIBUTING.md) file for details. Existing contributors can be checked in the [contributors list](https://gitlab.com/joseasoler/tradergen/-/graphs/main).

## License

This project is licensed under the MIT license. Check the [LICENSE](LICENSE) file for details.

## Acknowledgements

Read the [ACKNOWLEDGEMENTS.md](ACKNOWLEDGEMENTS.md) file for details.
