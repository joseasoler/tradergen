using RimWorld;
using Verse;

namespace TG.DefOf
{
	[RimWorld.DefOf]
	public class ThingCategory
	{
		public static ThingCategoryDef BodyParts;
		public static ThingCategoryDef BodyPartsNatural;
		public static ThingCategoryDef Buildings;
		public static ThingCategoryDef BuildingsJoy;
		public static ThingCategoryDef BuildingsFurniture;
		public static ThingCategoryDef BuildingsSecurity;
		public static ThingCategoryDef EggsFertilized;
		public static ThingCategoryDef EggsUnfertilized;
		public static ThingCategoryDef FoodRaw;
		public static ThingCategoryDef Grenades;
		public static ThingCategoryDef InertRelics;

		[MayRequire("VanillaExpanded.VGeneticsE")]
		public static ThingCategoryDef GR_GeneticMaterial;

		static ThingCategory() => DefOfHelper.EnsureInitializedInCtor(typeof(ThingCategory));
	}
}