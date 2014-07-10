#region header

// LacunaConverter - PartExtensions.cs
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
// Created: 2014-07-10 4:06 AM

#endregion

#region using

using System;
using System.Collections.Generic;

using UnityEngine;

#endregion

/* This was taken directly from TAC Life Support; do not alter. */

namespace ArkaneSystems.KerbalSpaceProgram.Lacuna
{
    public static class PartExtensions
    {
        public static double TakeResource (this Part part, string resourceName, double demand)
        {
            PartResourceDefinition resource = PartResourceLibrary.Instance.GetDefinition (resourceName);
            return TakeResource (part, resource, demand);
        }

        public static double TakeResource (this Part part, int resourceId, double demand)
        {
            PartResourceDefinition resource = PartResourceLibrary.Instance.GetDefinition (resourceId);
            return TakeResource (part, resource, demand);
        }

        public static double TakeResource (this Part part, PartResourceDefinition resource, double demand)
        {
            if (resource == null)
            {
                Debug.LogError ("ArkaneSystems.KerbalSpaceProgram.Lacuna.PartExtensions.TakeResource: resource is null");
                return 0.0;
            }

            switch (resource.resourceFlowMode)
            {
                case ResourceFlowMode.NO_FLOW:
                    return TakeResource_NoFlow (part, resource, demand);
                case ResourceFlowMode.ALL_VESSEL:
                    return TakeResource_AllVessel (part, resource, demand);
                case ResourceFlowMode.STACK_PRIORITY_SEARCH:
                    return TakeResource_StackPriority (part, resource, demand);
                case ResourceFlowMode.STAGE_PRIORITY_FLOW:
                    Debug.LogWarning (
                                      "ArkaneSystems.KerbalSpaceProgram.Lacuna.PartExtensions.TakeResource: ResourceFlowMode.STAGE_PRIORITY_FLOW is not supported yet.");
                    return part.RequestResource (resource.id, demand);
                default:
                    Debug.LogWarning (
                                      "ArkaneSystems.KerbalSpaceProgram.Lacuna.PartExtensions.TakeResource: Unknown ResourceFlowMode = " +
                                      resource.resourceFlowMode);
                    return part.RequestResource (resource.id, demand);
            }
        }

        public static double IsResourceAvailable (this Part part, string resourceName, double demand)
        {
            PartResourceDefinition resource = PartResourceLibrary.Instance.GetDefinition (resourceName);
            return IsResourceAvailable (part, resource, demand);
        }

        public static double IsResourceAvailable (this Part part, int resourceId, double demand)
        {
            PartResourceDefinition resource = PartResourceLibrary.Instance.GetDefinition (resourceId);
            return IsResourceAvailable (part, resource, demand);
        }

        public static double IsResourceAvailable (this Part part, PartResourceDefinition resource, double demand)
        {
            if (resource == null)
            {
                Debug.LogError (
                                "ArkaneSystems.KerbalSpaceProgram.Lacuna.PartExtensions.IsResourceAvailable: resource is null");
                return 0.0;
            }

            switch (resource.resourceFlowMode)
            {
                case ResourceFlowMode.NO_FLOW:
                    return IsResourceAvailable_NoFlow (part, resource, demand);
                case ResourceFlowMode.ALL_VESSEL:
                    return IsResourceAvailable_AllVessel (part, resource, demand);
                case ResourceFlowMode.STACK_PRIORITY_SEARCH:
                    return IsResourceAvailable_StackPriority (part, resource, demand);
                case ResourceFlowMode.STAGE_PRIORITY_FLOW:
                    Debug.LogWarning (
                                      "ArkaneSystems.KerbalSpaceProgram.Lacuna.PartExtensions.IsResourceAvailable: ResourceFlowMode.STAGE_PRIORITY_FLOW is not supported yet.");
                    return IsResourceAvailable_AllVessel (part, resource, demand);
                default:
                    Debug.LogWarning (
                                      "ArkaneSystems.KerbalSpaceProgram.Lacuna.PartExtensions.IsResourceAvailable: Unknown ResourceFlowMode = " +
                                      resource.resourceFlowMode);
                    return IsResourceAvailable_AllVessel (part, resource, demand);
            }
        }

        private static double TakeResource_NoFlow (Part part, PartResourceDefinition resource, double demand)
        {
            // ignoring PartResourceDefinition.ResourceTransferMode

            PartResource partResource = part.Resources.Get (resource.id);
            if (partResource != null)
            {
                if (partResource.flowMode == PartResource.FlowMode.None)
                {
                    Debug.LogWarning (
                                      "ArkaneSystems.KerbalSpaceProgram.Lacuna.PartExtensions.TakeResource_NoFlow: cannot take resource from a part where FlowMode is None.");
                    return 0.0;
                }
                if (!partResource.flowState)
                {
                    // Resource flow was shut off -- no warning needed
                    return 0.0;
                }
                if (demand >= 0.0)
                {
                    if (partResource.flowMode == PartResource.FlowMode.In)
                    {
                        Debug.LogWarning (
                                          "ArkaneSystems.KerbalSpaceProgram.Lacuna.PartExtensions.TakeResource_NoFlow: cannot take resource from a part where FlowMode is In.");
                        return 0.0;
                    }

                    double taken = Math.Min (partResource.amount, demand);
                    partResource.amount -= taken;
                    return taken;
                }
                if (partResource.flowMode == PartResource.FlowMode.Out)
                {
                    Debug.LogWarning (
                                      "ArkaneSystems.KerbalSpaceProgram.Lacuna.PartExtensions.TakeResource_NoFlow: cannot give resource to a part where FlowMode is Out.");
                    return 0.0;
                }

                double given = Math.Min (partResource.maxAmount - partResource.amount, -demand);
                partResource.amount += given;
                return -given;
            }
            return 0.0;
        }

        private static double TakeResource_AllVessel (Part part, PartResourceDefinition resource, double demand)
        {
            if (demand >= 0.0)
            {
                double leftOver = demand;

                // Takes an equal percentage from each part (rather than an equal amount from each part)
                List<PartResource> partResources = GetAllPartResources (part.vessel, resource, true);
                double totalAmount = 0.0;
                foreach (PartResource partResource in partResources)
                    totalAmount += partResource.amount;

                if (totalAmount > 0.0)
                {
                    double percentage = Math.Min (leftOver / totalAmount, 1.0);

                    foreach (PartResource partResource in partResources)
                    {
                        double taken = partResource.amount * percentage;
                        partResource.amount -= taken;
                        leftOver -= taken;
                    }
                }

                return demand - leftOver;
            }
            else
            {
                double leftOver = -demand;

                List<PartResource> partResources = GetAllPartResources (part.vessel, resource, false);
                double totalSpace = 0.0;
                foreach (PartResource partResource in partResources)
                    totalSpace += partResource.maxAmount - partResource.amount;

                if (totalSpace > 0.0)
                {
                    double percentage = Math.Min (leftOver / totalSpace, 1.0);

                    foreach (PartResource partResource in partResources)
                    {
                        double space = partResource.maxAmount - partResource.amount;
                        double given = space * percentage;
                        partResource.amount += given;
                        leftOver -= given;
                    }
                }

                return demand + leftOver;
            }
        }

        private static double TakeResource_StackPriority (Part part, PartResourceDefinition resource, double demand)
        {
            // FIXME finish implementing
            return part.RequestResource (resource.id, demand);
        }

        private static double IsResourceAvailable_NoFlow (Part part, PartResourceDefinition resource, double demand)
        {
            PartResource partResource = part.Resources.Get (resource.id);
            if (partResource != null)
            {
                if (partResource.flowMode == PartResource.FlowMode.None || partResource.flowState == false)
                    return 0.0;
                if (demand > 0.0)
                {
                    if (partResource.flowMode != PartResource.FlowMode.In)
                        return Math.Min (partResource.amount, demand);
                }
                else
                {
                    if (partResource.flowMode != PartResource.FlowMode.Out)
                        return -Math.Min ((partResource.maxAmount - partResource.amount), -demand);
                }
            }

            return 0.0;
        }

        private static double IsResourceAvailable_AllVessel (Part part, PartResourceDefinition resource, double demand)
        {
            if (demand >= 0.0)
            {
                double amountAvailable = 0.0;

                foreach (Part p in part.vessel.parts)
                {
                    PartResource partResource = p.Resources.Get (resource.id);
                    if (partResource != null)
                    {
                        if (partResource.flowState && partResource.flowMode != PartResource.FlowMode.None &&
                            partResource.flowMode != PartResource.FlowMode.In)
                        {
                            amountAvailable += partResource.amount;

                            if (amountAvailable >= demand)
                                return demand;
                        }
                    }
                }

                return amountAvailable;
            }
            double availableSpace = 0.0;
            double demandedSpace = -demand;

            foreach (Part p in part.vessel.parts)
            {
                PartResource partResource = p.Resources.Get (resource.id);
                if (partResource != null)
                {
                    if (partResource.flowState && partResource.flowMode != PartResource.FlowMode.None &&
                        partResource.flowMode != PartResource.FlowMode.Out)
                    {
                        availableSpace += (partResource.maxAmount - partResource.amount);

                        if (availableSpace >= demandedSpace)
                            return demand;
                    }
                }
            }

            return -availableSpace;
        }

        private static double IsResourceAvailable_StackPriority (Part part,
                                                                 PartResourceDefinition resource,
                                                                 double demand)
        {
            // FIXME finish implementing
            return IsResourceAvailable_AllVessel (part, resource, demand);
        }

        private static List<PartResource> GetAllPartResources (Vessel vessel,
                                                               PartResourceDefinition resource,
                                                               bool consuming)
        {
            // ignoring PartResourceDefinition.ResourceTransferMode
            var resources = new List<PartResource> ();

            foreach (Part p in vessel.parts)
            {
                PartResource partResource = p.Resources.Get (resource.id);
                if (partResource != null)
                {
                    if (partResource.flowState && partResource.flowMode != PartResource.FlowMode.None)
                    {
                        if (consuming)
                        {
                            if (partResource.flowMode != PartResource.FlowMode.In && partResource.amount > 0.0)
                                resources.Add (partResource);
                        }
                        else
                        {
                            if (partResource.flowMode != PartResource.FlowMode.Out &&
                                partResource.amount < partResource.maxAmount)
                                resources.Add (partResource);
                        }
                    }
                }
            }

            return resources;
        }
    }
}
