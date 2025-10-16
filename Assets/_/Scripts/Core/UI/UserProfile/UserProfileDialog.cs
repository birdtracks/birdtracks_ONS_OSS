
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BirdTracks.Game.Core
{

    public sealed class UserProfileDialog : MonoBehaviour
    {
        public static readonly List<Dropdown.OptionData> _DefaultGender = new List<Dropdown.OptionData>
        {
            new Dropdown.OptionData("Other"),
            new Dropdown.OptionData("Male"),
            new Dropdown.OptionData("Female"),
        };
        public static readonly List<Dropdown.OptionData> _DefaultLocations = new List<Dropdown.OptionData>
        {
            new Dropdown.OptionData("Other"),
            new Dropdown.OptionData("Nceduluntu"),
            new Dropdown.OptionData("Kenwyn"),
            new Dropdown.OptionData("Kildare"),
            new Dropdown.OptionData("Little Ones"),
            new Dropdown.OptionData("UCT Educare"),
        };
        public static readonly List<Dropdown.OptionData> _DefaultLanguages = new List<Dropdown.OptionData>
        {
            new Dropdown.OptionData("Other"),
            new Dropdown.OptionData("IsiXhosa"),
            new Dropdown.OptionData("Afrikaans"),
            new Dropdown.OptionData("English"),
            new Dropdown.OptionData("IsiNdebeles"),
            new Dropdown.OptionData("IsiZulu"),
            new Dropdown.OptionData("Sepedi"),
            new Dropdown.OptionData("Sesotho"),
            new Dropdown.OptionData("Setswana"),
            new Dropdown.OptionData("SiSwati"),
            new Dropdown.OptionData("Tshivenda"),
            new Dropdown.OptionData("Xitsonga"),
        };
        public static readonly List<Dropdown.OptionData> _MonthOfBirth = new List<Dropdown.OptionData>
        {
            new Dropdown.OptionData("1"),
            new Dropdown.OptionData("2"),
            new Dropdown.OptionData("3"),
            new Dropdown.OptionData("4"),
            new Dropdown.OptionData("5"),
            new Dropdown.OptionData("6"),
            new Dropdown.OptionData("7"),
            new Dropdown.OptionData("8"),
            new Dropdown.OptionData("9"),
            new Dropdown.OptionData("10"),
            new Dropdown.OptionData("11"),
            new Dropdown.OptionData("12"),
        };
        public static readonly int _START_YEAR = 2000;
        public static readonly List<Dropdown.OptionData> _years;
        [SerializeField] private Dropdown m_ProfilesDropdown = default;
        [SerializeField] private Dropdown m_GenderDropdown = default;
        [SerializeField] private Dropdown m_LocationDropdown = default;
        [SerializeField] private Dropdown m_LanguagesDropdown = default;
        [SerializeField] private Dropdown m_YearDropdown = default;
        [SerializeField] private Dropdown m_MonthDropdown = default;
        [SerializeField] private Dropdown m_Daydropdown = default;
        [SerializeField] private InputField m_NameInput = default;
        [SerializeField] private InputField m_SurnameInput = default;
        [SerializeField] private GameObject m_DeleteProfileButton = default;
        [SerializeField] private LocalizeAudio m_LocalizeAudio = default;
        private UserProfileService _profileService;


        static UserProfileDialog()
        {
             _years = new List<Dropdown.OptionData>();
            for (int i = _START_YEAR; i < DateTime.Now.Year; i++)
            {
                _years.Add(new Dropdown.OptionData { text = i.ToString() });
            }
        }

        public void DeleteCurrentProfile()
        {
            DeleteCurrentProfileAsync();
        }

        private async Task DeleteCurrentProfileAsync()
        {
            throw new NotImplementedException();
            //await FindObjectOfType<UserProfileService>().DeleteCurrentProfile();
            Refresh();
        }

        public void AddNewProfile()
        {
            AddNewProfileAsync();
        }

        public async Task AddNewProfileAsync()
        {
            //await FindObjectOfType<UserProfileService>().CreateNewProfile(m_NameInput.text, m_SurnameInput.text);
            Refresh();
        }

        public void ExportData()
        {
            string profilesCsv;
            string tangramResultsCsv;

            FindObjectOfType<UserProfileService>().GetDataExport(
                out profilesCsv,
                out tangramResultsCsv
            );

            Debug.Log("Exporting data to: " + Application.persistentDataPath);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "profiles.csv"), profilesCsv);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "tangramResults.csv"), tangramResultsCsv);
        }

        private void Start()
        {
            StartAsync();
        }

        private async Task StartAsync()
        {
            var profileService = FindObjectOfType<UserProfileService>();
            await profileService.WaitUntilReady();

            List<UserProfile> allprofiles = profileService.GetAllProfiles().ToList();

            m_GenderDropdown.options = _DefaultGender;
            m_LocationDropdown.options = _DefaultLocations;
            m_LanguagesDropdown.options = _DefaultLanguages;
            m_YearDropdown.options = _years;
            m_MonthDropdown.options = _MonthOfBirth;

            int numdays = DateTime.DaysInMonth(int.Parse(_years[m_YearDropdown.value].text), int.Parse(_MonthOfBirth[m_MonthDropdown.value].text));
            var days = new List<Dropdown.OptionData>();

            for (int i = 1; i <= numdays; i++)
            {
                days.Add(new Dropdown.OptionData { text = i.ToString() });
            }

            m_Daydropdown.options = days;
            Refresh();
        }

        private void Subscribe()
        {
            m_ProfilesDropdown.onValueChanged.AddListener(OnProfilesDropdownChanged);
            m_GenderDropdown.onValueChanged.AddListener(OnGenderChangedHandler);
            m_LocationDropdown.onValueChanged.AddListener(OnLocationChangedHandler);
            m_LanguagesDropdown.onValueChanged.AddListener(OnLanguageChangedHandler);
            m_YearDropdown.onValueChanged.AddListener(OnYearChangedHandler);
            m_MonthDropdown.onValueChanged.AddListener(OnMonthChangedHandler);
            m_Daydropdown.onValueChanged.AddListener(OnDayChangedHandler);
        }

        private void Unsubscribe()
        {
            m_ProfilesDropdown.onValueChanged.RemoveListener(OnProfilesDropdownChanged);
            m_GenderDropdown.onValueChanged.RemoveListener(OnGenderChangedHandler);
            m_LocationDropdown.onValueChanged.RemoveListener(OnLocationChangedHandler);
            m_LanguagesDropdown.onValueChanged.RemoveListener(OnLanguageChangedHandler);
            m_YearDropdown.onValueChanged.RemoveListener(OnYearChangedHandler);
            m_MonthDropdown.onValueChanged.RemoveListener(OnMonthChangedHandler);
            m_Daydropdown.onValueChanged.RemoveListener(OnDayChangedHandler);
        }

        private void Refresh()
        {
            Unsubscribe();

            var profileService = FindObjectOfType<UserProfileService>();

            if (profileService == null)
            {
                return;
            }

            List<UserProfile> allprofiles = profileService.GetAllProfiles().ToList();
            m_ProfilesDropdown.options = allprofiles.Select(up => 
            {
                return new Dropdown.OptionData(up.FirstName + " " + up.LastName);
            }).ToList();

            m_ProfilesDropdown.value = allprofiles.FindIndex(p => p.Id == profileService.ActiveProfile.Id);
            m_GenderDropdown.value = _DefaultGender.FindIndex(v => v.text == profileService.ActiveProfile.Gender);
            m_LocationDropdown.value = _DefaultLocations.FindIndex(v => v.text == profileService.ActiveProfile.Location);
            m_LanguagesDropdown.value = _DefaultLanguages.FindIndex(v => v.text == profileService.ActiveProfile.Language);
            m_YearDropdown.value = _years.FindIndex(v => v.text == profileService.ActiveProfile.YearOfBirth);
            m_MonthDropdown.value = _MonthOfBirth.FindIndex(v => v.text == profileService.ActiveProfile.MonthOfBirth);
            m_Daydropdown.value = int.Parse(profileService.ActiveProfile.DayOfBirth);
            m_DeleteProfileButton.SetActive(!profileService.ActiveProfile.IsDefaultProfile);
            m_LocalizeAudio.Initialize();

            Subscribe();
        }

        private void OnProfilesDropdownChanged(int arg0)
        {
            OnProfilesDropdownChangedAsync(arg0);
        }

        private async Task OnProfilesDropdownChangedAsync(int arg0)
        {
            throw new NotImplementedException();
            //var profileService = FindObjectOfType<UserProfileService>();
            //await profileService.SetActiveProfile(profileService.GetAllProfiles()[arg0].Id);
            Refresh();
        }

        private void OnDayChangedHandler(int arg0)
        {
            var profileService = FindObjectOfType<UserProfileService>();
            profileService.ActiveProfile.DayOfBirth = m_Daydropdown.options[m_Daydropdown.value].text;
            profileService.SaveProfile(profileService.ActiveProfile);
        }

        private void OnMonthChangedHandler(int arg0)
        {
            var profileService = FindObjectOfType<UserProfileService>();
            profileService.ActiveProfile.MonthOfBirth = m_MonthDropdown.options[m_MonthDropdown.value].text;
            profileService.SaveProfile(profileService.ActiveProfile);
        }

        private void OnYearChangedHandler(int arg0)
        {
            var profileService = FindObjectOfType<UserProfileService>();
            profileService.ActiveProfile.YearOfBirth = m_YearDropdown.options[m_YearDropdown.value].text;
            profileService.SaveProfile(profileService.ActiveProfile);
        }

        private void OnLanguageChangedHandler(int arg0)
        {
            var profileService = FindObjectOfType<UserProfileService>();
            profileService.ActiveProfile.Language = m_LanguagesDropdown.options[m_LanguagesDropdown.value].text;
            profileService.SaveProfile(profileService.ActiveProfile);
        }

        private void OnGenderChangedHandler(int arg0)
        {
            var profileService = FindObjectOfType<UserProfileService>();
            profileService.ActiveProfile.Gender = m_GenderDropdown.options[m_GenderDropdown.value].text;
            profileService.SaveProfile(profileService.ActiveProfile);
        }

        private void OnLocationChangedHandler(int arg0)
        {
            var profileService = FindObjectOfType<UserProfileService>();
            profileService.ActiveProfile.Location = m_LocationDropdown.options[m_LocationDropdown.value].text;
            profileService.SaveProfile(profileService.ActiveProfile);
        }
    }
}