using UnityEngine.UI;
using UnityEngine;
using TMPro;
namespace YUR.SDK.Unity.UI
{
    public class Screens_MainMenu : YURScreenController
    {
        public static Screens_MainMenu inst;

        public GameObject EditProfile;
        public GameObject Stats_Screen;

        public GameObject SwitchUsers;
        public GameObject SignOut;

        public void Awake()
        {
            inst = this;
            Finished += Screens_MainMenu_Finished;
        }

        private void Screens_MainMenu_Finished()
        {
            //EditProfile = (GameObject)Instantiate(Resources.Load("YUR Selection Button"), gameObject.transform);
            //EditProfile.GetComponent<RectTransform>().localPosition = new Vector3(0, 53f, EditProfile.GetComponent<RectTransform>().localPosition.z);
            EditProfile.GetComponentInChildren<TextMeshProUGUI>().text = "Edit Profile";
            EditProfile.GetComponent<Button>().onClick.AddListener(delegate
            {
                YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_EditProfile.inst);
            });

            //Stats_Screen = (GameObject)Instantiate(Resources.Load("YUR Selection Button"), gameObject.transform);
            //Stats_Screen.GetComponent<RectTransform>().localPosition = new Vector3(0f, 11f, Stats_Screen.GetComponent<RectTransform>().localPosition.z);
            Stats_Screen.GetComponentInChildren<TextMeshProUGUI>().text = "View Stats";
            Stats_Screen.GetComponent<Button>().onClick.AddListener(delegate
            {
                YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_Stats.inst);
            });

            //SwitchUsers = (GameObject)Instantiate(Resources.Load("YUR Selection Button"), gameObject.transform);
            //SwitchUsers.GetComponent<RectTransform>().localPosition = new Vector3(0f, -69, SwitchUsers.GetComponent<RectTransform>().localPosition.z);
            SwitchUsers.GetComponentInChildren<TextMeshProUGUI>().text = "Switch Users";
            SwitchUsers.GetComponent<Button>().onClick.AddListener(delegate
            {
                YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_UserSelection.instance);
            });

            //SignOut = (GameObject)Instantiate(Resources.Load("YUR Selection Button"), gameObject.transform);
            //SignOut.GetComponent<RectTransform>().localPosition = new Vector3(0f, -111, SignOut.GetComponent<RectTransform>().localPosition.z);
            SignOut.GetComponentInChildren<TextMeshProUGUI>().text = "Sign Out";
            SignOut.GetComponent<Button>().onClick.AddListener(delegate
            {
                Logout.ActiveUser("Logging out from Main Menu");
                YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_InitialLogin.inst, true);
            });


        }
    }
}