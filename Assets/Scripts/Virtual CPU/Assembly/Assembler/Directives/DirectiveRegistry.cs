using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ReflectionAssembly = System.Reflection.Assembly;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler.Directives
{
    public static class DirectiveRegistry
    {
        private static Dictionary<string, Directive> directives;

        public static void RegisterDirectives()
        {
            directives = new Dictionary<string, Directive>();
            
            int directiveCount = 0;
            foreach (Type type in ReflectionAssembly.GetAssembly(typeof(Directive)).GetTypes()
                         .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Directive))))
            {
                Directive directive = (Directive)Activator.CreateInstance(type, new object[] {null});

                if (directives.ContainsKey(directive.Name))
                {
                    Debug.LogError($"Conflicting directive name {directive.Name} for {type.Name}, a directive of type {directives[directive.Name].GetType().Name} exists with this name");
                    continue;
                }
                else
                {
                    directives.Add(directive.Name, directive);
                }

                directiveCount++;
            }
            
            Debug.Log($"Loaded {directiveCount} directives");
        }

        public static bool TryGetDirective(string name, out Directive directive)
        {
            if (!directives.ContainsKey(name))
            {
                directive = null;
                return false;
            }

            directive = directives[name];
            return true;
        }
    }
}