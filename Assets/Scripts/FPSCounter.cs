using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    private Text _fpsCounterField;

    private float _timer = 0f;
    private int _framesCount = 0;

    private void Update()
    {
        CheckTimer();
    }

    private void CheckTimer()
    {
        _framesCount++;
        _timer += Time.deltaTime;

        if (_timer >= 1f)
        {
            _timer -= 1f;
            _fpsCounterField.text = _framesCount.ToString();

            _framesCount = 0;
        }
    }
}
