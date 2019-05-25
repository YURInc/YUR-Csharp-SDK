using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YUR.SDK.Unity.UI {
    public class YURScreenCoordinator : MonoBehaviour
    {

        public static YURScreenCoordinator ScreenCoordinator; 
        public List<YURScreenController> Hierarchy
        {
            get { return _screenControllers; }
            set { _screenControllers = value; }
        }

        public List<YURScreenController> _screenControllers = new List<YURScreenController>();
        public List<YURScreenController> ActiveControllers;

        public YURScreenController MainMenu;
        public YURScreenController InitialMenu;
        public KeyboardCanvas Keyboard;
        public GameObject Background;

        public void Awake()
        {
            ScreenCoordinator = this;
            int children = transform.childCount;
            for(int i = 0; i < children; ++i)
            {
                
                if (transform.GetChild(i).GetComponent<Screens_MainMenu>())
                {
                    MainMenu = transform.GetChild(i).GetComponent<Screens_MainMenu>();
                    ActiveControllers.Add(transform.GetChild(i).GetComponent<Screens_MainMenu>());
                    MainMenu.gameObject.SetActive(false);
                }
                else if (transform.GetChild(i).GetComponent<Screens_InitialLogin>())
                {
                    InitialMenu = transform.GetChild(i).GetComponent<Screens_InitialLogin>();
                    ActiveControllers.Add(transform.GetChild(i).GetComponent<Screens_InitialLogin>());
                    InitialMenu.gameObject.SetActive(false);
                }
                else if (transform.GetChild(i).GetComponent<Image>())
                {

                }
                else
                {
                    ActiveControllers.Add(transform.GetChild(i).GetComponent<YURScreenController>());
                    transform.GetChild(i).gameObject.SetActive(false);
                    
                }
            }

            Keyboard = gameObject.transform.parent.GetComponentInChildren<KeyboardCanvas>();
            Keyboard.SetActive(false);

            gameObject.SetActive(false);
        }

        public void PresentNewScreen<T>(T screenController, bool resetHierarchy = false)
        {
            Keyboard.SetActive(false);
            int index = GetScreenIndex(screenController);
            Hierarchy[Hierarchy.Count - 1].gameObject.SetActive(false);
            if (resetHierarchy)
                Hierarchy.Clear();

            Hierarchy.Add(ActiveControllers[index]);

            if (Hierarchy[Hierarchy.Count - 1].gameObject != null)
            {
                Hierarchy[Hierarchy.Count - 1].gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("Could not find referenced object!");
            }
        }

        public int GetScreenIndex<T>(T yURScreenController)
        {
            int counts = ActiveControllers.Count;
            int index = -1;
            for(int i = 0; i < counts; i++)
            {
                if(ActiveControllers[i] is T)
                {
                    index = i;
                }
            }

            return index;
        }

        public void BackButtonPressed()
        {
            Keyboard.SetActive(false);
            int index = Hierarchy.Count - 1;
            Hierarchy[index].gameObject.SetActive(false);
            Hierarchy.RemoveAt(index);

            if (index - 1 < 0)
            {
                YURScreenSystem.ScreenSystem.DeactivateYURDash();
            }
            else
            {
                Hierarchy[index - 1].gameObject.SetActive(true);
            }
        }

        public void CloseButtonPressed()
        {
            Keyboard.SetActive(false);
            YURScreenSystem.ScreenSystem.DeactivateYURDash();
        }
    }
}