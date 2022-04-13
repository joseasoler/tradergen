using HarmonyLib;
using RimWorld;

namespace TG.Harmony
{
	/// <summary>
	/// Orbital traders use the TraderGen algorithm to generate their stock.
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

			var constructorPostfix = new HarmonyMethod(AccessTools.Method(typeof(TradeShip), nameof(ConstructorPostfix)));
			harmony.Patch(constructor, postfix: constructorPostfix);

			var generateThings = typeof(RimWorld.TradeShip).GetMethod(nameof(RimWorld.TradeShip.GenerateThings));
			var generatePostfix = new HarmonyMethod(AccessTools.Method(typeof(TradeShip), nameof(GenerateThingsPostfix)));
			harmony.Patch(generateThings, postfix: generatePostfix);
		}

		/// <summary>
		/// After the constructor is executed, TraderGen will launch the algorithm if the TraderKindDef uses GenExtension.
		/// TradeShip.GenerateThings() will later use the stock generators gathered by the algorithm.
		/// </summary>
		/// <param name="__instance">Trade ship instance</param>
		/// <param name="def">Procedurally generated TraderKindDef</param>
		/// <param name="faction">Faction to use for the def</param>
		private static void ConstructorPostfix(ref RimWorld.TradeShip __instance, in TraderKindDef def, in Faction faction)
		{
			// ToDo: Adapt to the new code.
			/*
			var extension = __instance.TraderKind.GetModExtension<GenExtension>();
			if (extension == null) return;

			var result = Gen.Gen.Generate(extension.node, -1, __instance.faction);
			// Result may be null if an error happened.
			__instance.TraderKind.stockGenerators = result != null ? result.generators : new List<StockGenerator>();
			*/
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