using UnityEngine;

namespace FXObjects.Fireworks
{

    public class FireworksFX : MonoBehaviour
    {
        public FireworksRocketFX FireworksRocketFX { get; private set; }
        public FireworksRocketSmokeFX FireworksRocketSmokeFX { get; private set; }
        public FireworksExplosionFX FireworksExplosionFX { get; private set; }

        private void Awake()
        {
            FireworksRocketFX = GetComponentInChildren<FireworksRocketFX>();
            FireworksRocketSmokeFX = GetComponentInChildren<FireworksRocketSmokeFX>();
            FireworksExplosionFX = GetComponentInChildren<FireworksExplosionFX>();
        }

        public void SetRocketSmokeColor(Color color)
        {
            FireworksRocketSmokeFX.SetColorOverLifetimeColor(color);
        }

        public void SetExplosionStartColors(Color color1, Color color2)
        {
            FireworksExplosionFX.SetStartColors(color1, color2);
        }

        public void SetRocketStartDelay(float time)
        {
            FireworksRocketFX.SetStartDelay(time);
        }
    }
}