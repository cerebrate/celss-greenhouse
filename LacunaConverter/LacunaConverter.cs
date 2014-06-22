#region header

// LacunaConverter - LacunaConverter.cs
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
// Created: 2014-05-18 12:51 PM

#endregion

// The LacunaConverter is based upon the TacGenericConverter from Thunder Aerospace Corporation's
// Life Support, by Taranis Elsu. Code from the aforementioned is used under CC BY-NC-SA 3.0 license.

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tac;

using UnityEngine;

#endregion

namespace ArkaneSystems.KerbalSpaceProgram.Lacuna
{
    public class LacunaGreenhouseConverter : PartModule
    {
        /*
         * Example config file:
         *    MODULE
         *    {
         *       name = LacunaGreenhouseConverter
         *
         *       // Number of units to convert per day.
         *       ConversionRate = 2
         *
         *       // A comma separated list of resources to use as inputs in each mode.
         *       // For each resource, list the resource name and the amount (which
         *       // is multiplied by the conversionRate)
         *       CelssInputResources = CarbonDioxide, 261.78, WasteWater, 1.98, Waste, 0.56, ElectricCharge, 1900800
         *       IsruInputResources = CarbonDioxide, 261.78, Water, 1.84, Ammonia, 0.72, ElectricCharge, 1900800
         *
         *       // A comma separated list of resources to output in each mode. Same as above
         *       // but also specify whether it should keep converting if the
         *       // resource is full (generating excess that will be thrown away).
         *       CelssOutputResources = Oxygen, 304.27, true, Water, 1.798, true, Food, 0.32, true
         *       IsruOutputResources = Oxygen, 304.27, true, Water, 1.798, true, Food, 0.32, true
         *    }
         */

        private const int SecondsPerDay = 24 * 60 * 60;
        private static readonly char[] Delimiters = {' ', ',', '\t', ';'};
        private List<LacunaResourceRatio> CelssInputResourceList;

        [KSPField]
        public string CelssInputResources = "";

        private List<LacunaResourceRatio> CelssOutputResourceList;

        [KSPField]
        public string CelssOutputResources = "";

        [KSPField]
        public float ConversionRate = 1.0f;

        private List<LacunaResourceRatio> IsruInputResourceList;

        [KSPField]
        public string IsruInputResources = "";

        private List<LacunaResourceRatio> IsruOutputResourceList;

        [KSPField]
        public string IsruOutputResources = "";

        [KSPField]
        public string ShutterAnimationName = "";

        public Animation ShutterAnimation = null;

        [KSPField (isPersistant = true)]
        public int _underlyingMode;

        public LacunaConverterMode Mode
        {
            get { return (LacunaConverterMode) this._underlyingMode; }
            set { this._underlyingMode = (int) value;  }
        }

        protected readonly string[] Modes = {"Inactive", "Operating in CELSS mode", "Operating in ISRU mode"};

        [KSPField (isPersistant = false, guiActive = true, guiName = "Status")]
        public string StatusDisplay;

        private double lastUpdateTime;

        [KSPEvent (guiActive = true, guiName = "Activate in CELSS mode", active = true)]
        public void ActivateInCelssMode ()
        {
            this.Mode = LacunaConverterMode.Celss;

            // open shutters
            if (this.ShutterAnimation != null)
            {
                this.ShutterAnimation[this.ShutterAnimationName].speed = 1.0f;
                this.ShutterAnimation.Play (this.ShutterAnimationName);
            }
        }

        [KSPEvent (guiActive = true, guiName = "Activate in ISRU mode", active = true)]
        public void ActivateInIsruMode ()
        {
            this.Mode = LacunaConverterMode.Isru;

            // open shutters
            if (this.ShutterAnimation != null)
            {
                this.ShutterAnimation[this.ShutterAnimationName].speed = 1.0f;
                this.ShutterAnimation.Play (this.ShutterAnimationName);
            }
        }

        [KSPEvent (guiActive = true, guiName = "Shut Down", active = false)]
        public void ShutdownGreenhouse ()
        {
            this.Mode = LacunaConverterMode.Disabled;

            // close shutters
            if (this.ShutterAnimation != null)
            {
                this.ShutterAnimation[this.ShutterAnimationName].speed = -1.0f;
                this.ShutterAnimation.Play (this.ShutterAnimationName);
            }
        }

        public override void OnAwake ()
        {
            base.OnAwake ();
            this.UpdateResourceLists ();

            if (!string.IsNullOrEmpty (this.ShutterAnimationName))
            {
                this.ShutterAnimation = this.part.FindModelAnimators (this.ShutterAnimationName)[0];
            }
        }

        public override void OnStart (StartState state)
        {
            base.OnStart (state);

            if (state != StartState.Editor)
                this.part.force_activate ();

            if (this.Mode == LacunaConverterMode.Disabled)
            {
                this.ShutterAnimation[this.ShutterAnimationName].speed = -1.0f;
                this.ShutterAnimation.Play (this.ShutterAnimationName);
            }
        }

        public override void OnFixedUpdate ()
        {
            base.OnFixedUpdate ();

            if (Time.timeSinceLevelLoad < 1.0f || !FlightGlobals.ready)
                return;

            if (this.lastUpdateTime == 0.0f)
            {
                // Just started running
                this.lastUpdateTime = Planetarium.GetUniversalTime ();
                return;
            }

            this.Events["ActivateInCelssMode"].active = (this.Mode == LacunaConverterMode.Disabled);
            this.Events["ActivateInIsruMode"].active = (this.Mode == LacunaConverterMode.Disabled);
            this.Events["ShutdownGreenhouse"].active = (this.Mode != LacunaConverterMode.Disabled);

            GlobalSettings globalSettings = TacLifeSupport.Instance.globalSettings;

            double deltaTime = Math.Min (Planetarium.GetUniversalTime () - this.lastUpdateTime,
                                         globalSettings.MaxDeltaTime);
            this.lastUpdateTime += deltaTime;

            List<LacunaResourceRatio> inputResourceList;
            List<LacunaResourceRatio> outputResourceList;

            switch (this.Mode)
            {
                case LacunaConverterMode.Celss:
                {
                    inputResourceList = this.CelssInputResourceList;
                    outputResourceList = this.CelssOutputResourceList;
                    break;
                }

                case LacunaConverterMode.Isru:
                {
                    inputResourceList = this.IsruInputResourceList;
                    outputResourceList = this.IsruOutputResourceList;
                    break;
                }

                case LacunaConverterMode.Disabled:
                default:
                {
                    this.StatusDisplay = this.Modes[(int) this.Mode];

                    // Nothing more to do here.
                    return;
                }
            }

            // Do the processing.
            double desiredAmount = this.ConversionRate / SecondsPerDay * deltaTime;
            double maxElectricityDesired = Math.Min (desiredAmount,
                                                     this.ConversionRate / SecondsPerDay *
                                                     Math.Max (globalSettings.ElectricityMaxDeltaTime,
                                                               Time.fixedDeltaTime));
                // Limit the max electricity consumed when reloading a vessel

            // Limit the resource amounts so that we do not produce more than we have room for, nor consume more than is available
            foreach (LacunaResourceRatio output in outputResourceList)
            {
                if (!output.AllowExtra)
                {
                    if (output.Resource.id == globalSettings.ElectricityId && desiredAmount > maxElectricityDesired)
                    {
                        // Special handling for electricity
                        double desiredElectricity = maxElectricityDesired * output.Ratio;
                        double availableSpace = -this.part.IsResourceAvailable (output.Resource, -desiredElectricity);
                        desiredAmount = desiredAmount * (availableSpace / desiredElectricity);
                    }
                    else
                    {
                        double availableSpace =
                            -this.part.IsResourceAvailable (output.Resource, -desiredAmount * output.Ratio);
                        desiredAmount = availableSpace / output.Ratio;
                    }

                    if (desiredAmount <= 0.000000001)
                    {
                        // Out of space, so no need to run
                        this.StatusDisplay = "No space for more " + output.Resource.name;
                        return;
                    }
                }
            }

            foreach (LacunaResourceRatio input in inputResourceList)
            {
                if (input.Resource.id == globalSettings.ElectricityId && desiredAmount > maxElectricityDesired)
                {
                    // Special handling for electricity
                    double desiredElectricity = maxElectricityDesired * input.Ratio;
                    double amountAvailable = this.part.IsResourceAvailable (input.Resource, desiredElectricity);
                    desiredAmount = desiredAmount * (amountAvailable / desiredElectricity);
                }
                else
                {
                    double amountAvailable = this.part.IsResourceAvailable (input.Resource, desiredAmount * input.Ratio);
                    desiredAmount = amountAvailable / input.Ratio;
                }

                if (desiredAmount <= 0.000000001)
                {
                    // Not enough input resources
                    this.StatusDisplay = "Not enough " + input.Resource.name;
                    return;
                }
            }

            foreach (LacunaResourceRatio input in inputResourceList)
            {
                double desired;

                if (input.Resource.id == globalSettings.ElectricityId)
                    desired = Math.Min (desiredAmount, maxElectricityDesired) * input.Ratio;
                else
                    desired = desiredAmount * input.Ratio;

                double actual = this.part.TakeResource (input.Resource, desired);

                if (actual < (desired * 0.999))
                {
                    this.LogWarning ("OnFixedUpdate: obtained less " + input.Resource.name + " than expected: " +
                                     desired.ToString ("0.000000000") + "/" + actual.ToString ("0.000000000"));
                }
            }

            foreach (LacunaResourceRatio output in outputResourceList)
            {
                double desired;

                if (output.Resource.id == globalSettings.ElectricityId)
                    desired = Math.Min (desiredAmount, maxElectricityDesired) * output.Ratio;
                else
                    desired = desiredAmount * output.Ratio;

                double actual = -this.part.TakeResource (output.Resource.id, -desired);

                if (actual < (desired * 0.999) && !output.AllowExtra)
                {
                    this.LogWarning ("OnFixedUpdate: put less " + output.Resource.name + " than expected: " +
                                     desired.ToString ("0.000000000") + "/" + actual.ToString ("0.000000000"));
                }
            }

            this.StatusDisplay = this.Modes[(int) this.Mode];
        }

        public override void OnLoad (ConfigNode node)
        {
            this.Log ("OnLoad: " + node);
            base.OnLoad (node);
            this.lastUpdateTime = Utilities.GetValue (node, "lastUpdateTime", this.lastUpdateTime);

            this.UpdateResourceLists ();
        }

        public override void OnSave (ConfigNode node)
        {
            node.AddValue ("lastUpdateTime", this.lastUpdateTime);
            this.Log ("OnSave: " + node);
        }

        public override string GetInfo ()
        {
            var sb = new StringBuilder ();
            sb.Append (base.GetInfo ());

            sb.Append ("\nContains the LacunaGreenhouseConverter module\n  CELSS mode inputs: ");
            sb.Append (String.Join (", ",
                                    this.CelssInputResourceList.Select (
                                                                        value =>
                                                                        value.Resource.name + ", " + value.Ratio)
                                        .ToArray ()));
            sb.Append ("\n  CELSS mode outputs: ");
            sb.Append (String.Join (", ",
                                    this.CelssOutputResourceList.Select (
                                                                         value =>
                                                                         value.Resource.name + ", " + value.Ratio)
                                        .ToArray ()));

            sb.Append ("\n  ISRU mode inputs: ");
            sb.Append (String.Join (", ",
                                    this.IsruInputResourceList.Select (value => value.Resource.name + ", " + value.Ratio)
                                        .ToArray ()));
            sb.Append ("\n  ISRU mode outputs: ");
            sb.Append (String.Join (", ",
                                    this.IsruOutputResourceList.Select (
                                                                        value =>
                                                                        value.Resource.name + ", " + value.Ratio)
                                        .ToArray ()));

            sb.Append ("\n  Conversion Rate: ");
            sb.Append (this.ConversionRate);
            sb.Append ("\n");

            return sb.ToString ();
        }

        private void UpdateResourceLists ()
        {
            if (this.CelssInputResourceList == null)
                this.CelssInputResourceList = new List<LacunaResourceRatio> ();

            if (this.CelssOutputResourceList == null)
                this.CelssOutputResourceList = new List<LacunaResourceRatio> ();

            if (this.IsruInputResourceList == null)
                this.IsruInputResourceList = new List<LacunaResourceRatio> ();

            if (this.IsruOutputResourceList == null)
                this.IsruOutputResourceList = new List<LacunaResourceRatio> ();

            this.ParseInputResourceString (this.CelssInputResources, this.CelssInputResourceList);
            this.ParseOutputResourceString (this.CelssOutputResources, this.CelssOutputResourceList);

            this.ParseInputResourceString (this.IsruInputResources, this.IsruInputResourceList);
            this.ParseOutputResourceString (this.IsruOutputResources, this.IsruOutputResourceList);
        }

        private void ParseInputResourceString (string resourceString, List<LacunaResourceRatio> resources)
        {
            resources.Clear ();

            string[] tokens = resourceString.Split (Delimiters, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < (tokens.Length - 1); i += 2)
            {
                PartResourceDefinition resource = PartResourceLibrary.Instance.GetDefinition (tokens[i]);
                double ratio;
                if (resource != null && double.TryParse (tokens[i + 1], out ratio))
                    resources.Add (new LacunaResourceRatio (resource, ratio));
                else
                    this.Log ("Cannot parse \"" + resourceString + "\", something went wrong.");
            }

            string ratios = resources.Aggregate ("",
                                                 (result, value) =>
                                                 result + value.Resource.name + ", " + value.Ratio + ", ");
            this.Log ("Input resources parsed: " + ratios + "\nfrom " + resourceString);
        }

        private void ParseOutputResourceString (string resourceString, List<LacunaResourceRatio> resources)
        {
            resources.Clear ();

            string[] tokens = resourceString.Split (Delimiters, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < (tokens.Length - 2); i += 3)
            {
                PartResourceDefinition resource = PartResourceLibrary.Instance.GetDefinition (tokens[i]);
                double ratio;
                bool allowExtra;
                if (resource != null && double.TryParse (tokens[i + 1], out ratio) &&
                    bool.TryParse (tokens[i + 2], out allowExtra))
                    resources.Add (new LacunaResourceRatio (resource, ratio, allowExtra));
                else
                    this.Log ("Cannot parse \"" + resourceString + "\", something went wrong.");
            }

            string ratios = resources.Aggregate ("",
                                                 (result, value) =>
                                                 result + value.Resource.name + ", " + value.Ratio + ", ");
            this.Log ("Output resources parsed: " + ratios + "\nfrom " + resourceString);
        }
    }
}
