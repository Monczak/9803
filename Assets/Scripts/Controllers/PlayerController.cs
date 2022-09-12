using NineEightOhThree.Math;
using NineEightOhThree.Objects;
using UnityEngine;

namespace NineEightOhThree.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        private GridTransform gridTransform;

        private PlayerControls controls;

        private void Awake()
        {
            controls = new PlayerControls();

            gridTransform = GetComponent<GridTransform>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Vector2Byte pos = gridTransform.pixelPos.GetValue<Vector2Byte>();
            gridTransform.pixelPos.SetValue(new Vector2Byte(pos.x + 1, pos.y));
        }
    }
}

