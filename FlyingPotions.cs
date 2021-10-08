using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ObjectBased.Shelf;
using System;
using System.Linq;
using UnityEngine;

namespace FlyingPotions
{

	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	public class FlyingPotions : BaseUnityPlugin
	{
		public const string pluginGuid = "potioncraft.hxp.flyingPotions";
		public const string pluginName = "Levitation Sensation";
		public const string pluginVersion = "0.0.0.2";

		//Mod config
		public static ConfigEntry<bool> potionsFly;
		public static ConfigEntry<float> flightGravity0;
		public static ConfigEntry<float> flightGravity1;
		public static ConfigEntry<float> flightGravity2;
		public static ConfigEntry<float> flightGravity3;
		public static ConfigEntry<float> flightGravity4;
		public static ConfigEntry<float> flightGravity5;

		public static ConfigEntry<bool> addShelf;

		public void Awake()
		{		
			Debug.Log("Loading " + pluginName+", Version: " + pluginVersion);
			DoPatching();
			BindConfigs();
			Debug.Log("Finished Loading " + pluginName + ", Version: " + pluginVersion);
		}


		private void BindConfigs()
        {
			potionsFly = Config.Bind("Levitation Station", "Potions Fly", true, "If your potions should fly or not.");
			flightGravity0 = Config.Bind("Levitation Station", "Normal Gravity Amount", 1.0f, "Set gravity scale for all non-flight potions (only if above is currently active");
			flightGravity1 = Config.Bind("Levitation Station", "Flight 1 Amount", 0.075f, "Set gravity scale for potions with Flight 1");
			flightGravity2 = Config.Bind("Levitation Station", "Flight 2 Amount", 0f);
			flightGravity3 = Config.Bind("Levitation Station", "Flight 3 Amount", -0.075f);
			flightGravity4 = Config.Bind("Levitation Station", "Flight 4 Amount", -0.2f);
			flightGravity5 = Config.Bind("Levitation Station", "Flight 5 Amount", -1.0f);
			addShelf = Config.Bind("Levitation Station", "Solid bedside table", true, "Lets you place potions and ingredients on the beside table.");
		}

		private void DoPatching()
		{
			var harmony = new HarmonyLib.Harmony("hxp.flyingPotions");
			harmony.PatchAll();
		}


		public static void AddMagicShelf()
        {
			//var shelf = Shelf.shelves.FirstOrDefault();
			//Debug.Log(shelf);
			//var shelf = GameObject.Find("Room Lab");

			//get shelf
			Shelf shelf = Resources.FindObjectsOfTypeAll<Shelf>().First();

			//Copy shelf at new position
			shelf = Instantiate(shelf, new Vector3(4.5f,12.2f,0),new Quaternion());

			//Get sprite and turn it off
			SpriteRenderer sr = shelf.gameObject.GetComponentInChildren<SpriteRenderer>();
			sr.enabled = false;
		}


	}

	[HarmonyPatch(typeof(ItemFromInventory))]
	[HarmonyPatch("OnReleasePrimary")]
	class FlightPotionsFlyPatch
	{
		static void Postfix(ItemFromInventory __instance)
		{


			if(FlyingPotions.potionsFly.Value)
            {

			PotionItem potionItem = __instance as PotionItem;

				if (potionItem != null)
				{
					//Access this array according to the amount of flying effects in the released potion.
					//0 flying effects = 1.0f = default gravity.
					float[] gravityValues = { FlyingPotions.flightGravity0.Value,
										FlyingPotions.flightGravity1.Value,
										FlyingPotions.flightGravity2.Value,
										FlyingPotions.flightGravity3.Value,
										FlyingPotions.flightGravity4.Value,
										FlyingPotions.flightGravity5.Value};

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

	[HarmonyPatch(typeof(RoomManager))]
	[HarmonyPatch("OrderedStart")]
	class AddObjectsPatch
    {
		static void Postfix()
        {
			if (FlyingPotions.addShelf.Value) {
				Debug.Log("Attempting to add Shelf");
				FlyingPotions.AddMagicShelf();
			}
        }
    }



}
