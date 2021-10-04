using BepInEx;
using HarmonyLib;
using System;
using UnityEngine;

namespace FlyingPotions
{

	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	public class FlyingPotions : BaseUnityPlugin
	{
		public const string pluginGuid = "potioncraft.hxp.flyingPotions";
		public const string pluginName = "Levitation Sensation";
		public const string pluginVersion = "0.0.0.1";

		public void Awake()
		{		
			Debug.Log("Loading " + pluginName+", Version: " + pluginVersion);
			DoPatching();
			Debug.Log("Finished Loading " + pluginName + ", Version: " + pluginVersion);
		}

		private void DoPatching()
		{
			var harmony = new HarmonyLib.Harmony("hxp.flyingPotions");
			harmony.PatchAll();
		}

		//Patching start

		[HarmonyPatch(typeof(ItemFromInventory))]
		[HarmonyPatch("OnReleasePrimary")]
		class FlightPotionsFlyPatch
		{
			static void Postfix(ItemFromInventory __instance)
			{
				PotionItem potionItem = __instance as PotionItem;

				if (potionItem != null)
				{
					//Access this array according to the amount of flying effects in the released potion.
					//0 flying effects = 1.0f = default gravity.
					float[] gravityValues = { 1.0f, 0.075F, 0F, -0.075F };

					Potion potion = potionItem.inventoryItem as Potion;
					var id = 0;
					foreach (PotionEffect effect in potion.effects)
					{
						if (effect.name == "Fly")
						{
							id += 1;
						}
					}

					potionItem.SetGravityScale(gravityValues[id]);
				}
			}

		}

	}
}
