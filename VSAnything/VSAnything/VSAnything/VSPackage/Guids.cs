// Guids.cs
// MUST match guids.h
using System;

namespace Company.VSAnything
{
    static class GuidList
    {
        public const string guidVSAnythingPkgString = "f0242d42-7c10-472e-b103-aebd2d3940ba";
        public const string guidVSAnythingCmdSetString = "aa570726-018c-468e-8508-18f7b2c9a7ce";

        public static readonly Guid guidVSAnythingCmdSet = new Guid(guidVSAnythingCmdSetString);
    };
}