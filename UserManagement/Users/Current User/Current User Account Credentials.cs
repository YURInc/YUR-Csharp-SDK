using UnityEngine;

namespace YUR.SDK.Unity
{
    public class LoginCredentials
    {
        [SerializeField]
        private string idToken;
        [SerializeField]
        private string refreshToken;
        [SerializeField]
        private string localId;
        [SerializeField]
        private string displayName;

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        public string IDtoken
        {
            get
            {
                return idToken;
            }
            set
            {
                idToken = value;
            }
        }


        public string RefreshToken
        {
            get { return refreshToken; }
            set
            {

                refreshToken = value;

            }
        }


        public string LocalId
        {
            get { return localId; }
            set
            {
                localId = value;
            }
        }
    }
    public partial class YUR_CurrentUser : MonoBehaviour
    {
        public YUR_Profile Profile = new YUR_Profile();
        public LoginCredentials loginCredentials = new LoginCredentials();
        
    }
}
