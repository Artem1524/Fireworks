using System.Collections.Generic;
using UnityEngine;

namespace FXObjects.Fireworks
{

    [CreateAssetMenu(fileName = "NewFireworksDataConfiguration", menuName = "Configs/Fireworks Data Configuration")]
    public class FireworksFXDataConfiguration : ScriptableObject
    {
        [SerializeField]
        private List<FireworksFXData> _fireworksFXData;

        public List<FireworksFXData> FireworksFXData => _fireworksFXData;
    }
}