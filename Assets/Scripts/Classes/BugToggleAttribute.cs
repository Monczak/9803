using System;

namespace NineEightOhThree.Classes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BugToggleAttribute : Attribute
    {
        public string BugName { get; init; }

        public BugToggleAttribute(string bugName)
        {
            BugName = bugName;
        }
    }
}