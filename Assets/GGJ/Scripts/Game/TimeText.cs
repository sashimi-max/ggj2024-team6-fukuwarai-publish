using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GGJ.Game
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TimeText : MonoBehaviour
    {
        TextMeshProUGUI text;
        // Start is called before the first frame update
        void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            text.text = $"{Mathf.RoundToInt(TimeManager.Instance.remainingTime)}";
        }
    }
}
