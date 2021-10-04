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
		public const string pluginVersion = "0.0.0.2";

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
					float[] gravityValues = { 1.0f, 0.075F, 0F, -0.075F,-0.2f,-1.0f };

					Potion potion = potionItem.inventoryItem as Potion;

					if (potion != null) //It maybe possible to acquire empty potions at somepoint.
					{

						var id = 0;
						foreach (PotionEffect effect in potion.effects)
						{
							if (effect.name == "Fly")
							{
								id += 1;
							}
						}

						//On the off chance a potion has more than 5 effects.
						if (id < gravityValues.Length)
						{
							potionItem.SetGravityScale(gravityValues[id]);
						}

					}
				}
			}

		}

	}
}
