#region header

// LacunaConverter - TacSettings.cs
// 
// Lacuna Space Systems's CELSS Greenhouse for Kerbal Space Program.
// 
// (Note that Lacuna Space Systems is a fictitious corporate entity created for entertainment
//  purposes. It is in no way meant to represent a real corporate or other entity, and any
//  similarities to such are purely coincidental.)
// 
// Alistair J. R. Young
// Arkane Systems
// 
// Copyright Arkane Systems 2014.  All rights reserved. License available under Creative
// Commons; see the LICENSE file for more details.
// 
// Created: 2014-07-10 3:52 AM

#endregion

using Tac;

namespace ArkaneSystems.KerbalSpaceProgram.Lacuna
{
    internal static class TacSettings
    {
        static TacSettings ()
        {
            globalSettings = TacLifeSupport.Instance.globalSettings;
        }

        private static object globalSettings = null;

        public static int MaxDeltaTime
        {
            get { return ((GlobalSettings) globalSettings).MaxDeltaTime; }
        }

        public static int ElectricityMaxDeltaTime
        {
            get { return ((GlobalSettings)globalSettings).ElectricityMaxDeltaTime; }
        }

        public static int ElectricityId
        {
            get { return ((GlobalSettings)globalSettings).ElectricityId; }
        }
    }
}
