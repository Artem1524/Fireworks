using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace FXObjects.Fireworks
{

    [RequireComponent(typeof(ParticleSystem))]
    public class FXObject : MonoBehaviour
    {
        protected ParticleSystem _particleSystem;

        private void Awake()
        {
            TryGetComponent(out _particleSystem);
        }

        public void SetColorOverLifetimeColor(Color color)
        {
            GradientColorKey[] gradientColorKeys = _particleSystem.colorOverLifetime.color.gradient.colorKeys;

            //if (gradientColorKeys.Length != 2)
            //    Debug.LogError($"Количество цветов градиента (colorOverLifetime) объекта {name} должно быть равно 2!");

            for (int i = 0; i < gradientColorKeys.Length; i++)
                gradientColorKeys[i].color = color;

            _particleSystem.colorOverLifetime.color.gradient.colorKeys = gradientColorKeys;
        }

        public void SetStartColors(Color color1, Color color2)
        {
            MainModule main = _particleSystem.main;

            MinMaxGradient minMaxGradient = main.startColor;
            minMaxGradient.colorMin = color1;
            minMaxGradient.colorMax = color2;

            main.startColor = minMaxGradient;   // При обращении напрямую к _particleSystem.main.startColor ошибка
                                                // "Не удалось изменить возвращаемое значение 'ParticleSystem.main', т. к. оно не является переменной
        }

        public void SetStartDelay(float time)
        {
            MainModule main = _particleSystem.main;

            MinMaxCurve minMaxCurve = main.startDelay;
            minMaxCurve.constant = time;

            main.startDelay = minMaxCurve;
        }
    }
}