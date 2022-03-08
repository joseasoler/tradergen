using System;
using System.Collections.Generic;
using Verse;

namespace TG
{
	internal class WeightedElement<T> : IComparable
	{
		public T Element;
		public float Weight;

		public WeightedElement(T e, float w)
		{
			Element = e;
			Weight = w;
		}

		public int CompareTo(object rhsObj)
		{
			if (!(rhsObj is WeightedElement<T> rhs))
			{
				throw new ArgumentException("Object is not a WeightedElement");
			}

			return rhs.Weight.CompareTo(Weight);
		}
	}

	public class Algorithm
	{
		/// <summary>
		/// Choose n elements randomly from the provided list without repetition, weighted with the provided function.
		/// </summary>
		/// <param name="elements">List of available elements to choose.</param>
		/// <param name="getWeight">Weight of an element when randomly choosing one of them.</param>
		/// <param name="n">Number of elements to choose.</param>
		/// <typeparam name="T">Type of the elements.</typeparam>
		/// <returns></returns>
		public static List<T> ChooseNWeightedRandomly<T>(List<T> elements, Func<T, float> getWeight, int n)
			where T : class
		{
			if (elements.Count <= n)
			{
				return elements;
			}

			// Obtain a sorted weighted list.
			var weightedElements = new List<WeightedElement<T>>();
			var totalWeight = 0.0f;
			foreach (var element in elements)
			{
				if (element == null)
				{
					continue;
				}

				var weight = getWeight(element);
				if (weight > 0.0f)
				{
					totalWeight += weight;
					weightedElements.Add(new WeightedElement<T>(element, weight));
				}
				else
				{
					Logger.ErrorOnce(
						$"ChooseNWeightedRandomly: Element {element} of type {typeof(T).Name} has an invalid weight {weight}");
				}
			}

			weightedElements.Sort();

			var result = new List<T>();
			while (result.Count < n)
			{
				var randomCumulativeWeight = Rand.Range(0.0f, totalWeight);
				var cumulativeWeight = 0.0f;
				T chosenElement = null;
				foreach (var weightedElement in weightedElements)
				{
					cumulativeWeight += weightedElement.Weight;
					if (randomCumulativeWeight > cumulativeWeight) continue;
					chosenElement = weightedElement.Element;
					// Elements are chosen without repetition. Weight is set to zero to ignore the element from now on.
					totalWeight -= weightedElement.Weight;
					weightedElement.Weight = 0.0f;
					break;
				}

				result.Add(chosenElement);
			}

			return result;
		}
	}
}