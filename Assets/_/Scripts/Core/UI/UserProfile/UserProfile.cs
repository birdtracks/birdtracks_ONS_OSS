
using System;

namespace BirdTracks.Game.Core
{
    [Serializable]
    public sealed class UserProfile
    {
        public string Id;
        public string IdNumber;
        public string Location;
        public string DateOfBirth;
        public string YearOfBirth;
        public string MonthOfBirth;
        public string DayOfBirth;
        public string FirstName;
        public string LastName;
        public string Gender;
        public string Language;
        public string UserName;
        public bool IsDefaultProfile;
        public string Password;
    }
}