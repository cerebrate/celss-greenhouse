#region header

// LacunaConverter - Logging.cs
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
// Created: 2014-07-10 4:10 AM

#endregion

#region using

using UnityEngine;

#endregion

namespace ArkaneSystems.KerbalSpaceProgram.Lacuna
{
    public static class Logging
    {
        public static void Log (this UnityEngine.Object obj, string message)
        {
            Debug.Log (obj.GetType ().FullName + "[" + obj.GetInstanceID ().ToString ("X") + "][" +
                       Time.time.ToString ("0.00") + "]: " + message);
        }

        public static void LogWarning (this UnityEngine.Object obj, string message)
        {
            Debug.LogWarning (obj.GetType ().FullName + "[" + obj.GetInstanceID ().ToString ("X") + "][" +
                              Time.time.ToString ("0.00") + "]: " + message);
        }

        public static void LogError (this UnityEngine.Object obj, string message)
        {
            Debug.LogError (obj.GetType ().FullName + "[" + obj.GetInstanceID ().ToString ("X") + "][" +
                            Time.time.ToString ("0.00") + "]: " + message);
        }

        public static void Log (this System.Object obj, string message)
        {
            Debug.Log (obj.GetType ().FullName + "[" + obj.GetHashCode ().ToString ("X") + "][" +
                       Time.time.ToString ("0.00") + "]: " + message);
        }

        public static void LogWarning (this System.Object obj, string message)
        {
            Debug.LogWarning (obj.GetType ().FullName + "[" + obj.GetHashCode ().ToString ("X") + "][" +
                              Time.time.ToString ("0.00") + "]: " + message);
        }

        public static void LogError (this System.Object obj, string message)
        {
            Debug.LogError (obj.GetType ().FullName + "[" + obj.GetHashCode ().ToString ("X") + "][" +
                            Time.time.ToString ("0.00") + "]: " + message);
        }

        public static void Log (string context, string message)
        {
            Debug.Log (context + "[][" + Time.time.ToString ("0.00") + "]: " + message);
        }

        public static void LogWarning (string context, string message)
        {
            Debug.LogWarning (context + "[][" + Time.time.ToString ("0.00") + "]: " + message);
        }

        public static void LogError (string context, string message)
        {
            Debug.LogError (context + "[][" + Time.time.ToString ("0.00") + "]: " + message);
        }
    }
}
