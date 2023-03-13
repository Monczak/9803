using System.IO;
using UnityEngine;

namespace NineEightOhThree.Utilities
{
    public static class PathUtils
    {
        public static string GetAbsoluteResourceLocation<T>(string targetResourceLocation, string currentResourceLocation) where T : Object
        {
            string combinedPath =
                $"{currentResourceLocation[..currentResourceLocation.LastIndexOf('/')]}/{targetResourceLocation}";
            return Resources.Load<T>(combinedPath) is not null ? combinedPath : targetResourceLocation;
        }
    }
}