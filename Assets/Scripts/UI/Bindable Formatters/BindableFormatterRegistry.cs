using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NineEightOhThree.VirtualCPU;
using NineEightOhThree.VirtualCPU.Interfacing;

using ReflectionAssembly = System.Reflection.Assembly;

namespace NineEightOhThree.UI.BindableFormatters
{
    public static class BindableFormatterRegistry
    {
        private static Dictionary<string, IBindableDataFormatter> formattersByType;

        public static void RegisterFormatters()
        {
            formattersByType = new Dictionary<string, IBindableDataFormatter>();

            foreach (Type type in ReflectionAssembly.GetAssembly(typeof(IBindableDataFormatter)).GetTypes()
                         .Where(t => t.IsClass && !t.IsAbstract && typeof(IBindableDataFormatter).IsAssignableFrom(t)))
            {
                string formatterType = type.GetCustomAttribute<FormatterAttribute>().formatterType;
                formattersByType[formatterType] = (IBindableDataFormatter)Activator.CreateInstance(type);
            }
            
            Logger.Log($"Registered {formattersByType.Count} bindable formatters");
        }

        public static string[] BindableTypes => formattersByType.Keys.ToArray();
        public static IBindableDataFormatter GetFormatter(string type) => formattersByType[type];
        public static IBindableDataFormatter GetFormatter(int index) => formattersByType[BindableTypes[index]];
    }
}