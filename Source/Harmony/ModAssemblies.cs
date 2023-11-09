using System.Reflection;
using Verse;

namespace TraderGen.Harmony
{
	public static class ModAssemblies
	{
		private static Assembly _dynamicTradeInterface;

		public static void Initialize()
		{
			foreach (var pack in LoadedModManager.RunningMods)
			{
				var packageId = pack.PackageId;
				if (packageId == "zeracronius.dynamictradeinterface")
				{
					if (pack.assemblies == null || pack.assemblies.loadedAssemblies.NullOrEmpty())
					{
						Logger.Error($"Could not find assemblies for Dynamic Trade Interface. Patch will not be applied.");
					}
					else
					{
						_dynamicTradeInterface = pack.assemblies.loadedAssemblies[0];
					}
				}
			}
		}

		public static Assembly DynamicTradeInterface()
		{
			return _dynamicTradeInterface;
		}

		public static MethodBase GetMethod(Assembly assembly, string typeName, string methodName)
		{
			foreach (var type in assembly.GetTypes())
			{
				var currentTypeName = type.FullName;
				if (currentTypeName != null && !currentTypeName.Contains(typeName))
				{
					continue;
				}

				foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
					         BindingFlags.Instance | BindingFlags.Static))
				{
					if (method.Name.Contains(methodName))
					{
						return method;
					}
				}
			}

			return null;
		}
	}
}