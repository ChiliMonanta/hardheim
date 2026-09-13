using System;

namespace BepInEx.Harmony
{
    [AttributeUsage(AttributeTargets.Method)]
    [Obsolete("Use HarmonyLib.ParameterByRefAttribute directly", true)]
    public class ParameterByRefAttribute : Attribute
    {
        public int[] ParameterIndices { get; }

        public ParameterByRefAttribute(params int[] parameterIndices)
        {
            ParameterIndices = parameterIndices;
        }
    }
}
