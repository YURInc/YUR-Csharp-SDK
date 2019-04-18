using UnityEngine;

namespace YUR.SDK.Unity
{
    public class YUR_Profile
    {
        [SerializeField]
        private string email;
        [SerializeField]
        private string photoURL;
        [SerializeField]
        private string phoneNumber;
        [SerializeField]
        private string creationTime;

        
        public string Email { get { return email; } set { email = value; } }
        
        public string PhotoURL { get { return photoURL; } set { photoURL = value; } }
        
        public string PhoneNumber { get { return phoneNumber; } set { phoneNumber = value; } }
        
        public string CreationTime { get { return creationTime; } set { creationTime = value; } }
    }
}