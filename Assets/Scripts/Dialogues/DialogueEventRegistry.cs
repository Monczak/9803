using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NineEightOhThree.Dialogues
{
    public static class DialogueEventRegistry
    {
        public static List<DialogueEvent> Events { get; private set; }
        public static Dictionary<DialogueEvent, Type> EventTypes { get; private set; }

        public static bool IsNull() => Events is null;

        public static void RegisterEvents()
        {
            Events = Assembly.GetAssembly(typeof(DialogueEventRegistry))
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(DialogueEvent)))
                .Select(t => (DialogueEvent)Activator.CreateInstance(t))
                .ToList();

            EventTypes = new Dictionary<DialogueEvent, Type>();
            foreach (DialogueEvent theEvent in Events)
                EventTypes[theEvent] = theEvent.GetType();
            
            Logger.Log($"Registered {Events.Count} dialogue events");
        }
    }
}