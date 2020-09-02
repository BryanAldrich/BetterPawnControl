﻿using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    internal static class AlertManager
    {
        internal static List<AlertLevel> alertLevelsList = new List<AlertLevel>();
        internal static int _alertLevel = 0;
        internal static bool _automaticPawnsInterrupt = true;
        private static bool _initialized = false;

        //Only two levels supported for now (ON and OFF)
        internal static bool OnAlert
        {
            get
            {
                return _alertLevel > 0 ? true : false;
            }
            set
            {
                _alertLevel = value == true ?  1 :  0;
            }
        }

        internal static bool AutomaticPawnsInterrupt 
        { 
            get => _automaticPawnsInterrupt; 
            set => _automaticPawnsInterrupt = value; 
        }

        internal static void ForceInit ()
        {
            if (!_initialized)
            {
                alertLevelsList = new List<AlertLevel>();
                Dictionary<Resources.Type, Policy> noAlert = new Dictionary<Resources.Type, Policy>();
                noAlert.Add(Resources.Type.work, WorkManager.GetActivePolicy());
                noAlert.Add(Resources.Type.restrict, RestrictManager.GetActivePolicy());
                noAlert.Add(Resources.Type.assign, AssignManager.GetActivePolicy());
                noAlert.Add(Resources.Type.animal, AnimalManager.GetActivePolicy());

                Dictionary<Resources.Type, Policy> alertOn = new Dictionary<Resources.Type, Policy>(noAlert);

                alertLevelsList.Add(new AlertLevel(0, noAlert));
                alertLevelsList.Add(new AlertLevel(1, alertOn));
                _initialized = true;
            }
        }

        internal static void PawnsInterruptForced()
        {
            List<Pawn> Pawns = Find.CurrentMap.mapPawns.FreeColonists;
            foreach (Pawn pawn in Pawns)
            {
                pawn.mindState.priorityWork.ClearPrioritizedWorkAndJobQueue();
                if (pawn.Spawned && !pawn.Downed && !pawn.InMentalState && !pawn.Drafted)
                {
                    pawn.Map.pawnDestinationReservationManager.ReleaseAllClaimedBy(pawn);
                }
                pawn.jobs.ClearQueuedJobs();
                if (pawn.jobs.curJob != null && pawn.jobs.IsCurrentJobPlayerInterruptible() && 
                    !pawn.Downed && !pawn.InMentalState && !pawn.Drafted)
                {
                    pawn.jobs.EndCurrentJob(JobCondition.InterruptForced);
                }
            }
        }

        internal static Policy GetAlertPolicy(int level, Resources.Type type)
        {
            if (alertLevelsList.NullOrEmpty())
            {
                ForceInit();
            }
            return alertLevelsList.Find(x => x.level == level).settings.TryGetValue(type);
        }

        internal static void SaveState(int level, Resources.Type type, Policy policy)
        {
            alertLevelsList.Find(x => x.level == level).settings.SetOrAdd(type, policy);
        }

        internal static void SaveState(int level)
        {
            alertLevelsList.Find(x => x.level == level).settings.SetOrAdd(Resources.Type.work, WorkManager.GetActivePolicy());
            alertLevelsList.Find(x => x.level == level).settings.SetOrAdd(Resources.Type.restrict, RestrictManager.GetActivePolicy());
            alertLevelsList.Find(x => x.level == level).settings.SetOrAdd(Resources.Type.assign, AssignManager.GetActivePolicy());
            alertLevelsList.Find(x => x.level == level).settings.SetOrAdd(Resources.Type.animal, AnimalManager.GetActivePolicy());
        }

        internal static void LoadState(int level)
        {   
            List<AlertLevel> alertList = alertLevelsList.FindAll(x => x.level == level);
            foreach (AlertLevel alert in alertList)
            {
                foreach (KeyValuePair<Resources.Type, Policy> entry in alert.settings)
                {
                    switch(entry.Key)
                    {
                        case Resources.Type.work:
                            //WorkManager.SetActivePolicy(entry.Value);
                            WorkManager.LoadState(entry.Value);
                            break;
                        case Resources.Type.restrict:
                            //RestrictManager.SetActivePolicy(entry.Value);
                            RestrictManager.LoadState(entry.Value);
                            break;
                        case Resources.Type.assign:
                            //AssignManager.SetActivePolicy(entry.Value);
                            AssignManager.LoadState(entry.Value);
                            break;
                        case Resources.Type.animal:
                            //AnimalManager.SetActivePolicy(entry.Value);
                            AnimalManager.LoadState(entry.Value);
                            break;
                    }
                }
            }
        }
    }
}