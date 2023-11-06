using UnityEngine;

namespace FXObjects.Fireworks
{

    public class FireworksBuilder
    {
        private FireworksFX _fireworks;

        public FireworksBuilder(FireworksFX fireworksPrefab, Transform fxParent)
        {
            _fireworks = Object.Instantiate(fireworksPrefab, fireworksPrefab.transform.position, fireworksPrefab.transform.rotation, fxParent);
        }

        public FireworksBuilder WithRocketSmokeColor(Color color)
        {
            _fireworks.SetRocketSmokeColor(color);

            return this;
        }
        public FireworksBuilder WithExplosionStartColors(Color color1, Color color2)
        {
            _fireworks.SetExplosionStartColors(color1, color2);

            return this;
        }
        public FireworksBuilder WithRocketStartDelay(float time)
        {
            _fireworks.SetRocketStartDelay(time);

            return this;
        }

        public FireworksFX Build()
        {
            return _fireworks;
        }
    }
}