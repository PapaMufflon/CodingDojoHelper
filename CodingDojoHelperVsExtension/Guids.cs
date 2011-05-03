// Guids.cs
// MUST match guids.h
using System;

namespace MufflonoSoft.CodingDojoHelperVsExtension
{
    static class GuidList
    {
        public const string guidCodingDojoHelperVsExtensionPkgString = "7ba8957b-1cc7-42a3-9180-4e8cd27d66a5";
        public const string guidCodingDojoHelperVsExtensionCmdSetString = "c7267408-b709-428a-ba41-a5e02479792d";
        public const string guidToolWindowPersistanceString = "486687b3-3286-49a5-a11c-74d829df845d";

        public static readonly Guid guidCodingDojoHelperVsExtensionCmdSet = new Guid(guidCodingDojoHelperVsExtensionCmdSetString);
    };
}