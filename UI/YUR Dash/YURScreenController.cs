﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace YUR.SDK.Unity.UI
{
    public class YURScreenController : MonoBehaviour
    {
        public delegate void ScreenControllerSetup();
        public event ScreenControllerSetup Finished;

        public delegate void Buttons();
        public event Buttons BackButtonPressed;
        public event Buttons CloseButtonPressed;

        public List<VRUiKits.Utils.UIKitInputField> InputOrder = new List<VRUiKits.Utils.UIKitInputField>();

        [HideInInspector]
        public GameObject BackButton;
        [HideInInspector]
        public GameObject CloseButton;

        public void SetInputFieldFocus(int index)
        {
            if(this.InputOrder.Count > 0)
                VRUiKits.Utils.TMP_KeyboardManager.Target = InputOrder[index];
        }

        public void Start()
        {
            StartCoroutine(LoadSetupCloseBackButtons());
            SetInputFieldFocus(0);
        }

        public IEnumerator LoadSetupCloseBackButtons()
        {
            if (YUR.Yur.YURAssetBundle == null)
            {
                string URI = "file:///" + Application.dataPath + "/AssetBundles/" + YUR.Yur.assetBundleName;
                UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(URI, 0);
                yield return request.SendWebRequest();
                if (request.error != null)
                {
                    
                    yield break;
                }
                else
                {
                    YUR_Log.Log("Local asset bundle found!");
                    yield return YUR.Yur.YURAssetBundle = DownloadHandlerAssetBundle.GetContent(request);

                }
            }

            var temppp = YUR.Yur.YURAssetBundle.LoadAsset<GameObject>("YUR Back Button");
            yield return BackButton = Instantiate(temppp, gameObject.transform);
            // BackButton = (GameObject)Instantiate(Resources.Load("YUR Back Button"), gameObject.transform);
            BackButton.GetComponent<Button>().onClick.AddListener(delegate
            {
                if (BackButtonPressed != null)
                    BackButtonPressed.Invoke();
                YURScreenCoordinator.ScreenCoordinator.BackButtonPressed();
            });


            temppp = YUR.Yur.YURAssetBundle.LoadAsset<GameObject>("YUR Close Button");
            yield return CloseButton = Instantiate(temppp, gameObject.transform);

            // CloseButton = (GameObject)Instantiate(Resources.Load("YUR Close Button"), gameObject.transform);

            CloseButton.GetComponent<Button>().onClick.AddListener(delegate
            {
                if (CloseButtonPressed != null)
                    CloseButtonPressed.Invoke();
                YURScreenCoordinator.ScreenCoordinator.CloseButtonPressed();
            });
            if (Finished != null)
                Finished.Invoke();

            
        }

        public void SetBackButtonVisible(bool isEnabled)
        {
            BackButton.SetActive(isEnabled);
        }

        public void SetCloseButtonVisible(bool isEnabled)
        {
            CloseButton.SetActive(isEnabled);
        }

    }
}