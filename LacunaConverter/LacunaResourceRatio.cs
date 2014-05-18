#region header

// LacunaConverter - LacunaResourceRatio.cs
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
// Created: 2014-05-18 1:31 PM

// The LacunaConverter is based upon the TacGenericConverter from Thunder Aerospace Corporation's
// Life Support, by Taranis Elsu. Code from the aforementioned is used under CC BY-NC-SA 3.0 license.

#endregion

namespace ArkaneSystems.KerbalSpaceProgram.Lacuna
{
    public class LacunaResourceRatio
    {
        public PartResourceDefinition Resource;
        public double Ratio;
        public bool AllowExtra;

        public LacunaResourceRatio(PartResourceDefinition resource, double ratio, bool allowExtra = false)
        {
            this.Resource = resource;
            this.Ratio = ratio;
            this.AllowExtra = allowExtra;
        }
    }
}
