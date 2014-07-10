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

#region using

using System;
using System.Linq;
using System.Reflection;

#endregion

namespace ArkaneSystems.KerbalSpaceProgram.Lacuna
{
    internal static class TacSettings
    {
        private static Type globalSettingsType;
        private static object globalSettings;

        static TacSettings ()
        {
            // Create the TacLifeSupport type reference.
            Assembly tac = AppDomain.CurrentDomain.GetAssemblies ().Single (p => p.GetName ().Name == "TacLifeSupport");
            Type tls = tac.GetType ("Tac.TacLifeSupport");

            globalSettingsType = tac.GetType ("Tac.GlobalSettings");

            PropertyInfo instanceProp = tls.GetProperty ("Instance",
                                                         BindingFlags.Static | BindingFlags.FlattenHierarchy |
                                                         BindingFlags.Public);
            object instance = instanceProp.GetValue (null, null);

            PropertyInfo gsProp = tls.GetProperty ("globalSettings",
                                                   BindingFlags.GetProperty | BindingFlags.Instance |
                                                   BindingFlags.Public);
            globalSettings = gsProp.GetValue (instance, null);
        }

        public static int MaxDeltaTime
        {
            get
            {
                PropertyInfo mdtProp = globalSettingsType.GetProperty ("MaxDeltaTime",
                                                                       BindingFlags.Public | BindingFlags.Instance);
                object retval = mdtProp.GetValue (globalSettings, null);

                return (int) retval;
            }
        }

        public static int ElectricityMaxDeltaTime
        {
            get
            {
                PropertyInfo mdtProp = globalSettingsType.GetProperty ("ElectricityMaxDeltaTime",
                                                                       BindingFlags.Public | BindingFlags.Instance);
                object retval = mdtProp.GetValue (globalSettings, null);

                return (int) retval;
            }
        }

        public static int ElectricityId
        {
            get
            {
                PropertyInfo mdtProp = globalSettingsType.GetProperty ("ElectricityId",
                                                                       BindingFlags.Public | BindingFlags.Instance);
                object retval = mdtProp.GetValue (globalSettings, null);

                return (int) retval;
            }
        }
    }
}
