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
					_dynamicTradeInterface = pack.assemblies.loadedAssemblies[0];
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
				if (!currentTypeName.Contains(typeName))
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