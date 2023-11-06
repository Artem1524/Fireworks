using System.Collections.Generic;
using UnityEngine;

namespace FXObjects.Fireworks
{

    public class FireworksLauncher : MonoBehaviour
    {
        private readonly string PATH_TO_FIREWORKS_DATA_CONFIGURATIONS = "Configurations/ScriptableObjects/FireworksFXData/";

        [SerializeField]
        private FireworksFX _fireworksPrefab;
        [SerializeField]
        private Transform _fxParent;

        private List<FireworksFXData> _fireworksFXData = new List<FireworksFXData>();

        private List<FireworksFX> _fireworks = new List<FireworksFX>();

        private void OnEnable()
        {
            InitializeFireworksParticleSystems();
            // CreateAndAddFireworks(rocketColor1: Color.blue, rocketColor2: Color.cyan, smokeColor: Color.white);
            // CreateAndAddFireworks(rocketColor1: Color.red, rocketColor2: Color.yellow, smokeColor: Color.white, rocketStartDelay: 0.5f);
            // CreateAndAddFireworks(rocketColor1: Color.green, rocketColor2: Color.cyan, smokeColor: Color.white, rocketStartDelay: 1f);
            // CreateAndAddFireworks(rocketColor1: Color.red, rocketColor2: Color.magenta, smokeColor: Color.white, rocketStartDelay: 1.5f);
            // CreateAndAddFireworks(rocketColor1: Color.white, rocketColor2: Color.cyan, smokeColor: Color.white, rocketStartDelay: 2f);
        }

        private void CreateAndAddFireworks(Color rocketColor1, Color rocketColor2, Color smokeColor, float rocketStartDelay = 0f)
        {
            _fireworks.Add(new FireworksBuilder(_fireworksPrefab, _fxParent)
                .WithExplosionStartColors(rocketColor1, rocketColor2)
                .WithRocketSmokeColor(smokeColor)
                .WithRocketStartDelay(rocketStartDelay)
                .Build());
        }

        private void InitializeFireworksParticleSystems()
        {
            LoadFireworksConfigurations(ref _fireworksFXData, PATH_TO_FIREWORKS_DATA_CONFIGURATIONS);

            foreach (FireworksFXData data in _fireworksFXData)
                CreateAndAddFireworks(data.RocketColor1, data.RocketColor2, data.SmokeColor, data.RocketStartDelay);
        }

        private void LoadFireworksConfigurations(ref List<FireworksFXData> fireworksFXData, string pathToConfigurations)
        {
            FireworksFXDataConfiguration[] configs = Resources.LoadAll<FireworksFXDataConfiguration>(pathToConfigurations);

            foreach (FireworksFXData data in configs[0].FireworksFXData)
                fireworksFXData.Add(data);
        }
    }
}