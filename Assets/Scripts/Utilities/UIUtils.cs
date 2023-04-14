using UnityEngine;

namespace NineEightOhThree.Utilities
{
    public class UIUtils
    {
        public static Canvas GetRootCanvas(Component component)
        {
            Canvas[] parentCanvases = component.GetComponentsInParent<Canvas>();
            if (parentCanvases is not null && parentCanvases.Length > 0) {
                return parentCanvases[^1];
            }
            return null;
        }
    }
}