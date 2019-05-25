using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRUiKits.Utils;
using UnityEngine.UI;
using UnityEngine.Events;

namespace YUR.SDK.Unity.UI
{
    public class YUROptionsManager : MonoBehaviour
    {
        [Header("Template")]
        public GameObject optionTemplate;
        public delegate void ChangedValue(string value);
        public event ChangedValue NextPressed;
        public event ChangedValue PrevPressed;
        // The index of the selected <option> element in the options list (starts at 0)
        public Button Previous;
        public Button Next;
        public string selectedValue { get { return optionTemplate.GetComponent<TMPro.TextMeshProUGUI>().text; } set { optionTemplate.GetComponent<TMPro.TextMeshProUGUI>().text = value; } }

        public void Awake()
        {
            Previous.onClick.AddListener(new UnityAction(PreviousClicked));
            Next.onClick.AddListener(new UnityAction(NextClicked));
        }

        public void PreviousClicked()
        {
            if (PrevPressed != null)
                PrevPressed.Invoke(selectedValue);
        }

        public void NextClicked()
        {
            if(NextPressed != null)
                NextPressed.Invoke(selectedValue);
        }
    }
}