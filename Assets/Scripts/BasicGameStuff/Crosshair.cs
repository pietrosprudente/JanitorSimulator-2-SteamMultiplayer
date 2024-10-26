using UnityEngine;
using UnityEngine.UI;

namespace BasicGameStuff
{
    public class Crosshair : MonoBehaviour
    {
        public static Crosshair Instance { get; private set; }

        public static Color CrosshairColor
        {
            set
            {
                Instance.GetComponent<Image>().color = value;
            }
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}

