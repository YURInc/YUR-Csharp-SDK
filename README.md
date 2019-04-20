<!-- [![Image caption](/project.logo.jpg)](#) -->

# YUR C# SDK
[d]: #project
**[GETTING STARTED][gt] | [AUTHOR][auth] | [SUPPORT][ps] | [YUR Discord](https://yur.chat)**

This project produces the YUR.SDK.UNITY.dll found within [YUR Asset Package](https://github.com/YURInc/YUR-Unity-Package/wiki)

## MOTIVATION
This project allows access to YUR's API using YUR Native Plugin.

## GETTING STARTED
[gt]: #getting-started 'Getting started guide'

This project requires Visual Studio 2015 or newer.
Most Unity Versions should be compatible.

### Example Use Code
```c#
using YUR.SDK.Unity;

public class Login2Yur : Monobehaviour 
{
    public GameObject Submit;
    public GameObject LoginIssues;
    
    void Awake(){
      Submit.GetComponent<Button>().onClick.AddListener(delegate
      {
          UserManagement.YUR_UserManager.Successful_Login += YUR_UserManager_Successful_Login;
          UserManagement.YUR_UserManager.Bad_Login += YUR_UserManager_Bad_Login;
          string Login_Issues;
          if (Login.Email_Password(EmailInput.GetComponent<YURInputSetup>().Input.text, PasswordInput.GetComponent<YURInputSetup>().Input.text, out Login_Issues))
          {
              YURScreenCoordinator.ScreenCoordinator.Keyboard.SetActive(false);
          }
          LoginIssues.GetComponent<Text>().text = Login_Issues;
      });
    }
}
```

## AUTHOR
[auth]: #author 'Credits & author\'s contacts info '
YUR is a company dedicated to conjoining fitness and immersive gaming.
To find out more and contact the creators directly
[**Visit our discord** | **https://yur.chat**](https://yur.chat)

## PRODUCTION STATUS & SUPPORT
[ps]: #production-status--support 'Support info'

This project is in its early stages and will rapidly be improving.


<hr>

Go back to the **[project description][d]**

Copyright © 2019 YUR Inc
