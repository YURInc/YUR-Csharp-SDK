using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Networking;
using TMPro;


namespace YUR.SDK.Unity.UI
{
    public class Screens_CreateAccount : YURScreenController
    {
        public static Screens_CreateAccount instance;
        [Header("Legal")]
        public GameObject Legal;
        public TextMeshProUGUI MainContent;
        public Button Accept;
        public Button Decline;
        public Scrollbar scrollbar;

    // TODO
    // Scrollbar end and reset


        UnityEngine.Events.UnityAction unityAction;


        string TermsOfService = string.Empty;
        string PrivacyPolicy = string.Empty;
        string EndUserLicenseAgreement = string.Empty;

        public void Awake()
        {
            instance = this;
            Finished += Screens_CreateAccount_Finished;
        }

        private void Screens_CreateAccount_Finished()
        {
            SetBackButtonVisible(false);
            Setup();
        }
        

        private void Setup()
        {
            if(string.IsNullOrEmpty(PrivacyPolicy))
            {
                StartCoroutine(GET_FILE(SERVER_FILE.PP));
            }

            Accept.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
            Decline.GetComponentInChildren<TextMeshProUGUI>().text = "Back";
            Decline.onClick.AddListener(delegate ()
            {
                Setup();
                BackButton.GetComponent<Button>().onClick.Invoke();

            });

            MainContent.text = "\n\n\nThe YUR Mobile App requires the creation of a YUR account." +
                        "\n In order to do that, you will have to read, understand and agree to the following" +
                        "\n Privacy Policy, Terms of Service, and End User License Agreement." +
                        "\n" +
                        "\n" +
                        "Press Continue";

            Accept.interactable = true;
            unityAction = new UnityEngine.Events.UnityAction(delegate ()
            {
                Display_PrivacyPolicy();
            });

            Accept.onClick.AddListener(unityAction);

            Legal.SetActive(true);
            SignUp.SetActive(false);
        }

        public void Display_PrivacyPolicy()
        {
            scrollbar.value = 1;
            Accept.onClick.RemoveListener(unityAction);
            MainContent.text = "In order to continue, you must read and accept our Privacy Policy. \n\n" + PrivacyPolicy;
            Accept.GetComponentInChildren<TextMeshProUGUI>().text = "Accept";
            unityAction = new UnityAction(delegate ()
            {
                Display_TermsofService();
            });
            Accept.onClick.AddListener(unityAction);

            Decline.GetComponentInChildren<TextMeshProUGUI>().text = "Decline";
        }

        public void Display_TermsofService()
        {
            scrollbar.value = 1;
            Accept.onClick.RemoveListener(unityAction);
            MainContent.text = "In order to continue, you must read and accept our Terms of Service. \n\n" + TermsOfService;
            unityAction = new UnityAction(delegate ()
            {
                Display_EndUserLicenseAgreement();
            });
            Accept.onClick.AddListener(unityAction);
        }

        public void Display_EndUserLicenseAgreement()
        {
            scrollbar.value = 1;
            Accept.onClick.RemoveListener(unityAction);
            MainContent.text = "In order to continue, you must read and accept our End User License Agreement. \n\n" + EndUserLicenseAgreement;
            unityAction = new UnityAction(delegate ()
            {

                PresentSignUpScreen();
            });
            Accept.onClick.AddListener(unityAction);
        }

        [Header("Sign Up")]
        public GameObject SignUp;
        public GameObject LoginIssues;
        public GameObject EmailInput;
        public GameObject PasswordInput;
        public GameObject NameInput;
        public GameObject Submit;

        public void PresentSignUpScreen()
        {
            scrollbar.value = 1;
            Legal.SetActive(false);
            SignUp.SetActive(true);

            SetBackButtonVisible(true);


            YURScreenCoordinator.ScreenCoordinator.Keyboard.SetActive(true, KeyboardCanvas.KeyboardStyle.KeyboardNumPad);
            //LoginIssues = (GameObject)Instantiate(Resources.Load("YUR Error Report"), gameObject.transform);
            //Submit = (GameObject)Instantiate(Resources.Load("YUR Selection Button"), SignUp.transform);
            //EmailInput = (GameObject)Instantiate(Resources.Load("YUR Input"), SignUp.transform);
            //PasswordInput = (GameObject)Instantiate(Resources.Load("YUR Input"), SignUp.transform);
            //NameInput = (GameObject)Instantiate(Resources.Load("YUR Input"), SignUp.transform);

            //EmailInput.GetComponent<RectTransform>().localPosition = new Vector3(0, 115f, EmailInput.GetComponent<RectTransform>().localPosition.z);
            EmailInput.GetComponent<YURInputSetup>().Label.text = "Email";
            EmailInput.GetComponent<YURInputSetup>().PlaceHolder.text = "i.e. \"username@gmail.com\"";

            //PasswordInput.GetComponent<RectTransform>().localPosition = new Vector3(0, 55, PasswordInput.GetComponent<RectTransform>().localPosition.z);
            PasswordInput.GetComponent<YURInputSetup>().Label.text = "Password";
            PasswordInput.GetComponent<YURInputSetup>().PlaceHolder.text = "6 Characters or more";


            //NameInput.GetComponent<RectTransform>().localPosition = new Vector3(0, -5, NameInput.GetComponent<RectTransform>().localPosition.z);
            NameInput.GetComponent<YURInputSetup>().Label.text = "Name";
            NameInput.GetComponent<YURInputSetup>().PlaceHolder.text = "i.e John Smith";
            //NameInput.GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.Name;


            //Submit.GetComponent<RectTransform>().localPosition = new Vector3(0, -60, PasswordInput.GetComponent<RectTransform>().localPosition.z);
            Submit.GetComponentInChildren<TextMeshProUGUI>().text = "Create Account";

            Submit.GetComponent<Button>().onClick.AddListener(delegate
            {
                UserManagement.YUR_UserManager.Successful_Login += YUR_UserManager_Successful_Login;
                UserManagement.YUR_UserManager.Bad_Login += YUR_UserManager_Bad_Login;
                string Login_Issues;
                if(Create.Email_Password(EmailInput.GetComponent<YURInputSetup>().Input.text, PasswordInput.GetComponent<YURInputSetup>().Input.text, NameInput.GetComponent<YURInputSetup>().Input.text, out Login_Issues))
                { 
                    YURScreenCoordinator.ScreenCoordinator.Keyboard.SetActive(false);
                    SignUp.SetActive(false);
                    Legal.SetActive(true);
                    Accept.gameObject.SetActive(false);
                    Decline.gameObject.SetActive(false);
                    MainContent.text = "\n\n\n Please wait while YUR account is created. \n\nYou will be automatically signed in once setup has finished";
                }
                LoginIssues.GetComponentInChildren<TextMeshProUGUI>().text = Login_Issues;
            });


        }

        private string YUR_UserManager_Bad_Login(string response)
        {
            YURScreenCoordinator.ScreenCoordinator.Keyboard.SetActive(true, KeyboardCanvas.KeyboardStyle.KeyboardNumPad);
            LoginIssues.GetComponentInChildren<TextMeshProUGUI>().text = response;
            UserManagement.YUR_UserManager.Successful_Login -= YUR_UserManager_Successful_Login;
            UserManagement.YUR_UserManager.Bad_Login -= YUR_UserManager_Bad_Login;
            Legal.SetActive(false);
            SignUp.SetActive(true);
            return response;
        }

        private string YUR_UserManager_Successful_Login(string response)
        {
            UserManagement.YUR_UserManager.Successful_Login -= YUR_UserManager_Successful_Login;
            UserManagement.YUR_UserManager.Bad_Login -= YUR_UserManager_Bad_Login;
            YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_MainMenu.inst, true);
            YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_EditProfile.inst);
            return response;
        }


        enum SERVER_FILE
        {
            TOS,
            PP,
            EULA
        }

        IEnumerator GET_FILE(SERVER_FILE file)
        {
            string url;
            if (file == SERVER_FILE.TOS)
                url = "https://yurapp-502de.firebaseapp.com/Doc/termsofservice.txt";
            else if (file == SERVER_FILE.PP)
                url = "https://yurapp-502de.firebaseapp.com/Doc/privacypolicy.txt";
            else if (file == SERVER_FILE.EULA)
                url = "https://yurapp-502de.firebaseapp.com/Doc/enduserlicenseagreement.txt";
            else
                url = "https://yurapp-502de.firebaseapp.com/api/v1/hidden/running";
            var uwr = new UnityWebRequest(url, "GET");
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError)
            {
                YUR_Log.Server_Log("Network Error occured while retrieving documents");
            }
            else
            {
                switch (file)
                {
                    case SERVER_FILE.PP:
                        PrivacyPolicy = uwr.downloadHandler.text;
                        StartCoroutine(GET_FILE(SERVER_FILE.TOS));
                        break;
                    case SERVER_FILE.TOS:
                        TermsOfService = uwr.downloadHandler.text;
                        StartCoroutine(GET_FILE(SERVER_FILE.EULA));
                        break;
                    case SERVER_FILE.EULA:
                        EndUserLicenseAgreement = uwr.downloadHandler.text;
                        break;
                }

            }
        }
    }
}