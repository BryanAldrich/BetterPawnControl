﻿using HugsLib;
using HugsLib.Utils;
using Verse;
using RimWorld;
using System.Collections.Generic;
using RimWorld.Planet;

namespace BetterPawnControl
{
	public class DataStorage : ModBase
	{
		public override string ModIdentifier
		{
			get { return "BetterPawnControl"; }
		}

		public override void WorldLoaded()
		{
			var obj =
				UtilityWorldObjectManager.GetUtilityWorldObject<WorldDataStore>();
		}

		private class WorldDataStore : UtilityWorldObject
		{

			//public class DataStorage : WorldComponent
			//{
			//	public DataStorage(World world) : base(world)
			//	{
			//	}

			public override void ExposeData()
			{
				base.ExposeData();

                if (Scribe.mode == LoadSaveMode.LoadingVars)
                {
                    //Let's make sure all static variables are cleaned.
                    //This shows there's a fundamental problem with this code
                    //A code refractor is require to remove the Static Managers 
                    //and replace it with GameComponents
                    AssignManager.Prisoners = null;
                    AssignManager.links = null;                    
                    AssignManager.policies = null;
                    AssignManager.activePolicies = null;
                    AssignManager.DefaultDrugPolicy = null;
                    AssignManager.DefaultFoodPolicy = null;
                    AssignManager.DefaultOutfit = null;
                    AssignManager.DefaultPrisonerFoodPolicy = null;
                    AnimalManager.links = null;
                    AnimalManager.policies = null;
                    AnimalManager.activePolicies = null;
                    WorkManager.links = null;
                    WorkManager.policies = null;
                    WorkManager.activePolicies = null;
                    RestrictManager.links = null;
                    RestrictManager.policies = null;
                    RestrictManager.activePolicies = null;
					AlertManager.alertLevelsList = null;
					System.GC.Collect();
                }

                if (Scribe.mode == LoadSaveMode.LoadingVars ||
					Scribe.mode == LoadSaveMode.Saving)
				{
					Scribe_References.Look<Outfit>(
						ref AssignManager._defaultOutfit,
						"DefaultOutfit");

					Scribe_References.Look<DrugPolicy>(
						ref AssignManager._defaultDrugPolicy,
						"DefaultDrugPolicy");

					Scribe_References.Look<FoodRestriction>(
						ref AssignManager._defaultFoodPolicy,
						"DefaultFoodPolicy");

					Scribe_References.Look<FoodRestriction>(
						ref AssignManager._defaultPrisonerFoodPolicy,
						"DefaultPrisonerFoodPolicy");

					Scribe_Collections.Look<Policy>(
						ref AssignManager.policies,
						"AssignPolicies", LookMode.Deep);

					Scribe_Collections.Look<AssignLink>(
						ref AssignManager.links,
						"AssignLinks", LookMode.Deep);

					Scribe_Collections.Look<string>(
						ref AssignManager.Prisoners,
						"Prisoners", LookMode.Value);

					if (AssignManager.Prisoners == null)
					{
						//this is only required if the save file contains
						//empty prisoners
						AssignManager.InstantiatePrisoners();
					}

					Scribe_Collections.Look<MapActivePolicy>(
						ref AssignManager.activePolicies,
						"AssignActivePolicies", LookMode.Deep);

					Scribe_Collections.Look<Policy>(
						ref AnimalManager.policies,
						"AnimalPolicies", LookMode.Deep);

					Scribe_Collections.Look<AnimalLink>(
						ref AnimalManager.links,
						"AnimalLinks", LookMode.Deep);

					Scribe_Collections.Look<MapActivePolicy>(
						ref AnimalManager.activePolicies,
						"AnimalActivePolicies", LookMode.Deep);

					Scribe_Collections.Look<Policy>(
						ref RestrictManager.policies,
						"RestrictPolicies", LookMode.Deep);

					Scribe_Collections.Look<RestrictLink>(
						ref RestrictManager.links,
						"RestrictLinks", LookMode.Deep);

                    Scribe_Collections.Look<MapActivePolicy>(
                        ref RestrictManager.activePolicies,
                        "RestrictActivePolicies", LookMode.Deep);

                    Scribe_Collections.Look<Policy>(
						ref WorkManager.policies,
						"WorkPolicies", LookMode.Deep);

					Scribe_Collections.Look<WorkLink>(
						ref WorkManager.links,
						"WorkLinks", LookMode.Deep);

					Scribe_Collections.Look<MapActivePolicy>(
						ref WorkManager.activePolicies,
						"WorkActivePolicies", LookMode.Deep);

					Scribe_Values.Look<int>(
						ref AlertManager._alertLevel,"ActiveLevel", 0, true);

					Scribe_Values.Look<bool>(
						ref AlertManager._automaticPawnsInterrupt, "AutomaticPawnsInterrupt", true, true);

					Scribe_Collections.Look<AlertLevel>(
						ref AlertManager.alertLevelsList,
						"AlertLevelsList", LookMode.Deep);


					if (Scribe.mode == LoadSaveMode.LoadingVars &&
						WorkManager.activePolicies == null)
					{
						//this only happens with existing saves. Existing saves 
						//have no WorkPolicy data so let's initialize!
						WorkManager.ForceInit();
					}

                    if (Scribe.mode == LoadSaveMode.LoadingVars &&
                        RestrictManager.activePolicies == null)
                    {
                        //temporary code to be removed on the next version. To fix saves games without activePolicies
                        RestrictManager.FixActivePolicies();
                    }
				}

				if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
				{
					Scribe_References.Look<Outfit>(
						ref AssignManager._defaultOutfit,
						"DefaultOutfit");

					Scribe_References.Look<DrugPolicy>(
						ref AssignManager._defaultDrugPolicy,
						"DefaultDrugPolicy");

					Scribe_References.Look<FoodRestriction>(
						ref AssignManager._defaultFoodPolicy,
						"DefaultFoodPolicy");

					Scribe_References.Look<FoodRestriction>(
						ref AssignManager._defaultPrisonerFoodPolicy,
						"DefaultPrisonerFoodPolicy");
				}
			}
        }
    }
}
