// Guids.cs
// MUST match guids.h
using System;

namespace KevinAenmey.MoveLine
{
    static class GuidList
    {
        public const string guidMoveLinePkgString = "593d8a1f-8719-4f45-888e-944177958a8e";
        public const string guidMoveLineCmdSetString = "880fdee3-b114-46bd-8ab6-99aba4f718d0";

        public static readonly Guid guidMoveLineCmdSet = new Guid(guidMoveLineCmdSetString);
    };
}