using UnityEngine.UI;
using UnityEngine;

namespace YUR.SDK.Unity.UI
{
    public class Screens_Stats : YURScreenController
    {
        public static Screens_Stats inst;
        [Header("Stats Viewer")]
        public StatsViewer CurrentGame;
        public StatsViewer AllGames;

        
        public YUR_CurrentUser User { get; set; }

        public void Awake()
        {
            inst = this;
            Finished += Screens_MainMenu_Finished;
        }

        private void Screens_MainMenu_Finished()
        {
            //CurrentGame.GetValues(ref UserManagement.YUR_UserManager.YUR_Users.CurrentUser.Data_Current_Game);
            //AllGames.GetValues(ref UserManagement.YUR_UserManager.YUR_Users.CurrentUser.Data_General_Calories);
        }

        public void OnEnable()
        {
            CurrentGame.GetValues(ref UserManagement.YUR_UserManager.YUR_Users.CurrentUser.Data_Current_Game);
            AllGames.GetValues(ref UserManagement.YUR_UserManager.YUR_Users.CurrentUser.Data_General_Calories);
        }
    }

}
