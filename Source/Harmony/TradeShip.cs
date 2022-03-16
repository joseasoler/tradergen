using System.Linq;
using HarmonyLib;
using RimWorld;
using TG.Gen;
using TG.Trader;
using Verse;

namespace TG.Harmony
{
	/// <summary>
	/// Patches the TradeShip constructor to inject procedurally generated TraderKindDefs.
	/// Although this approach for patching procedural generation into orbital traders is not very clean, it has been
	/// chosen due to its simplicity, and its compatibility with mods which also modify orbital trader ship creation such
	/// as Trader Ships.
	/// One of the issues of this approach is that the TraderKindDef received by parameter is being ignored. This may
	/// become a compatibility issue with other mods and will need to be kept in mind in the future.
	/// It also patches the Depart method to remove the Def when it stops being necessary.
	/// </summary>
	public static class TradeShip
	{

		/// <summary>
		/// Applies Harmony patches required for this functionality.
		/// </summary>
		/// <param name="harmony">Harmony instance.</param>
		public static void Patch(HarmonyLib.Harmony harmony)
		{
			var constructor = typeof(RimWorld.TradeShip).GetConstructor(new[]
			{
				typeof(TraderKindDef), typeof(Faction)
			});

			var constructorPrefix = new HarmonyMethod(AccessTools.Method(typeof(TradeShip), nameof(ConstructorPrefix)));
			var constructorPostfix = new HarmonyMethod(AccessTools.Method(typeof(TradeShip), nameof(ConstructorPostfix)));
			harmony.Patch(constructor, constructorPrefix, constructorPostfix);

			// Patching RimWorld.TradeShip.Depart is incompatible with Trader Ships. See TG.Harmony.Mod.TraderShips.
			if (HarmonyInitialization.TraderShipsModEnabled)
			{
				return;
			}

			var depart = typeof(RimWorld.TradeShip).GetMethod(nameof(RimWorld.TradeShip.Depart));
			var departPostfix = new HarmonyMethod(AccessTools.Method(typeof(TradeShip), nameof(DepartPostfix)));
			harmony.Patch(depart, postfix: departPostfix);

			var generateThings = typeof(RimWorld.TradeShip).GetMethod(nameof(RimWorld.TradeShip.GenerateThings));
			var generatePostfix = new HarmonyMethod(AccessTools.Method(typeof(TradeShip), nameof(GenerateThingsPostfix)));
			harmony.Patch(generateThings, postfix: generatePostfix);
		}

		/// <summary>
		/// Inject a procedurally generated TraderKindDef before the TradeShip is created.
		/// </summary>
		/// <param name="def">Definition originally intended for this ship. It will be discarded.</param>
		/// <param name="faction">Faction originally intended for this ship. It will be discarded.</param>
		/// <returns>True, allowing the original method to run.</returns>
		private static bool ConstructorPrefix(ref TraderKindDef def, ref Faction faction)
		{
			if (DefDatabase<TraderGenDef>.AllDefs.Where(traderGenDef => traderGenDef.orbital)
			    .TryRandomElementByWeight(traderGenDef => traderGenDef.commonality, out var genDef))
			{
				def = Find.World.GetComponent<TraderKind>().Generate(genDef);
				faction = OrbitalTraderArrival.GetFaction(def);
			}
			else
			{
				Logger.ErrorOnce("No TraderGenDef definitions could be found.");
			}

			return true;
		}

		/// <summary>
		/// Procedurally generate any other attributes required.
		/// </summary>
		/// <param name="__instance">Trade ship instance</param>
		/// <param name="def">Procedurally generated TraderKindDef</param>
		/// <param name="faction">Faction to use for the def</param>
		private static void ConstructorPostfix(ref RimWorld.TradeShip __instance, in TraderKindDef def, in Faction faction)
		{
			// ToDo procedural generation of ship names.
		}

		/// <summary>
		/// Clean up the generated TraderKindDef after the ship departs.
		/// </summary>
		/// <param name="__instance">Departing TradeShip instance.</param>
		private static void DepartPostfix(in RimWorld.TradeShip __instance)
		{
			Find.World.GetComponent<TraderKind>().Remove(__instance.def);
		}

		/// <summary>
		/// Log a generated things report if necessary.
		/// </summary>
		/// <param name="__instance">TradeShip instance.</param>
		private static void GenerateThingsPostfix(in RimWorld.TradeShip __instance)
		{
			Logger.GeneratedThingsReport(__instance, __instance.things);
		}
	}
}