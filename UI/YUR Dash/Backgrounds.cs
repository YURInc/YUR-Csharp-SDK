using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YUR.SDK.Unity.UI;

namespace YUR.SDK.Unity.UI.Background
{
    [Serializable]
    public class Background : MonoBehaviour
    {
        public GameObject Background_GO;
        public System.Type RelevantScreensType;

        public Background(GameObject Background_Object, YURScreenController RelevantScreen)
        {
            Background_GO = Background_Object;
            RelevantScreensType = RelevantScreen.GetType();
        }
        public Background(System.Type tahype)
        {
            RelevantScreensType = tahype;
        }
    }

   [Serializable]
    public class Backgrounds : MonoBehaviour
    {
        // Start is called before the first frame update
        public static Backgrounds Instance;

        public Image BackgroundBorder;
        public int InitialScreen;
        public int MainMenu;
        public int ActiveBackground;
        public bool Animate;

       
        public List<Background> BackgroundList;

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }

            Instance = this;
        }


        public void Start()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                Animate = true;
            }

            BackgroundList = new List<Background>();
            var Settings = Resources.Load("YURGeneralSettings") as YURSettingsScriptableObject;
            BackgroundList = JsonUtility.FromJson<List<Background>>(Settings.BackgroundsList);

            

            //var blistGobject = gameObject.transform.GetComponentInChildren<Mask>().gameObject;

            //BackgroundList = new List<Background>(blistGobject.transform.childCount);
            //for (int i = 0; i < BackgroundList.Count; i++)
            //{
            //    BackgroundList.Add(new Background(blistGobject.transform.GetChild(i).gameObject, );

            //    if (blistGobject.transform.GetChild(i).GetComponent<InitialBackground>())
            //    {
            //        InitialScreen = ActiveBackground = i;
            //    }

            //    if (blistGobject.transform.GetChild(i).GetComponent<MainMenuBackground>())
            //        MainMenu = i;

            //    blistGobject.transform.GetChild(i).gameObject.SetActive(false);
            //}
        }

        public void SetBackground<T>(T Screen)
        {
            //// Script priority low, bugs need to be worked. Return for now.
            return;
            int count = 0;
            foreach(Background background in BackgroundList)
            {
                if(background.RelevantScreensType == Screen.GetType())
                {
                    BackgroundList[ActiveBackground].Background_GO.SetActive(false);
                    BackgroundList[count].Background_GO.SetActive(true);
                    break;
                }
                else
                {
                    BackgroundList[count].Background_GO.SetActive(false);
                }
                count++;
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (!Animate)
                return;
        }
    }
}