using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BirdTracks.Game.Core
{
    public sealed class UserProfileService : MonoBehaviour
    {
        private static readonly string _ProfileIdsCollectionKey = "UserProfile.ProfileIds";
        private static readonly string _ProfilePrefix = "UserProfile.Profile.";
        private static readonly string _TangramResultIdsCollectionKey = "UserProfile.TangramResultIds";
        private static readonly string _TangramResultPrefix = "UserProfile.TangramResult.";
        [SerializeField] private bool m_Delete = default;
        [SerializeField] private GameObject m_ConnectingContainer = default;
        private List<TaskCompletionSource<bool>> _whenReadyTasks = new List<TaskCompletionSource<bool>>(10);
        private UserProfile _activeProfile;
        private bool _initialized;
        private int _connectingLock;
        private bool _isReady;


        public UserProfile ActiveProfile
        {
            get
            {
                return _activeProfile;
            }

            set
            {
                if (string.IsNullOrEmpty(value.Id))
                {
                    throw new ArgumentException();
                }

                _activeProfile = value;
                PlayerPrefs.SetString("UserProfile.ActiveProfile", _activeProfile.Id);
            }
        }


        private void IncrementConnectingLock()
        {
            _connectingLock++;
            m_ConnectingContainer.SetActive(_connectingLock != 0);
        }
        private void DecrementConnectingLock()
        {
            _connectingLock--;
            m_ConnectingContainer.SetActive(_connectingLock != 0);
        }

        private void OnEnable()
        {
            m_ConnectingContainer.SetActive(false);
            
            Debug.Log("Running user profile");
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await Initialize();
        }

        private void OnDisable()
        {
            _initialized = false;
        }

        private async Task Initialize()
        {
            if (_initialized)
            {
                return;
            }

            try
            {
                Debug.Log("Initializing User profile service!");
                IncrementConnectingLock();

                if (true)
                {
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                }

                var activeProfile = PlayerPrefs.GetString("UserProfile.ActiveProfile", string.Empty);
                var profile = default(UserProfile);

                if (!string.IsNullOrEmpty(activeProfile))
                {
                    profile = GetProfile(activeProfile);
                }

                if (string.IsNullOrEmpty(activeProfile) ||
                    string.IsNullOrEmpty(profile.UserName) ||
                    string.IsNullOrEmpty(profile.Password))
                {
                    var userProfile = new UserProfile
                    {
                        Gender = UserProfileDialog._DefaultGender.First().text,
                        Location = UserProfileDialog._DefaultLocations.First().text,
                        Language = UserProfileDialog._DefaultLanguages.First().text,
                        YearOfBirth = UserProfileDialog._START_YEAR.ToString(),
                        MonthOfBirth = UserProfileDialog._MonthOfBirth.First().text,
                        DayOfBirth = "1",
                        IsDefaultProfile = true
                    };

                    userProfile.FirstName = "Developer";
                    userProfile.LastName = "Trackosaurus";
                    userProfile.UserName = "developer@trackosaurus.education";
                    userProfile.Password = "developerP1!";

                    //var exists = await AwsService.CheckIfUserExists("developer@trackosaurus.education");
                    var exists = true;

                    if (exists)
                    {
                        // await AwsService.WrapCall(
                        //     AwsService.SignInUser,
                        //     userProfile);
                    }
                    else
                    {
                        // await AwsService.WrapCall(
                        //     AwsService.CreateAndSignInUser,
                        //     userProfile);
                    }

                    SaveProfile(userProfile);
                    ActiveProfile = userProfile;
                }
                else
                {
                    await SetActiveProfile(activeProfile);
                }

                DecrementConnectingLock();
                _isReady = true;

                for (int i = 0; i < _whenReadyTasks.Count; i++)
                {
                    _whenReadyTasks[i].SetResult(true);
                }

                _whenReadyTasks.Clear();
                _initialized = true;
            }
            catch (Exception e)
            {
                DecrementConnectingLock();
                Debug.LogException(e);
                UnityEngine.Object.FindObjectOfType<ErrorDialog>().OverrideOkayClickedCallback(InitializeAsync);
            }
        }

        public Task WaitUntilReady()
        {
            if (_isReady)
            {
                return Task.CompletedTask;
            }

            var resultTask = new TaskCompletionSource<bool>();
            _whenReadyTasks.Add(resultTask);
            return resultTask.Task;
        }

        private IEnumerable<string> GetExistingProfileIds()
        {
            return PlayerPrefs.GetString(_ProfileIdsCollectionKey).Split(',').Where(s => !string.IsNullOrEmpty(s));
        }

        public void LogTangramResult(string sceneName, int numPuzzlePieces, int numCorrectPieces, float timeElapsed, float activeTime, float inactiveTime)
        {

            if (ActiveProfile is null)
            {
                Debug.Log($"Active profile is {ActiveProfile}");
            }
            var result = new TangramResult
            {
                Id = Guid.NewGuid().ToString(),
                UserId = ActiveProfile.Id,
                SceneName = sceneName,
                NumPuzzlePieces = numPuzzlePieces,
                NumCorrectPieces = numCorrectPieces,
                TimeElapsed = timeElapsed,
                ActiveTime = activeTime,
                InactiveTime = inactiveTime,
            };

            SaveResult(result, _TangramResultIdsCollectionKey, _TangramResultPrefix);
        }

        private void SaveResult<T>(T result, string resultIdsKey, string resultPrefix)
            where T : IResult
        {
            List<string> ids = GetResultIds(resultIdsKey).ToList();
            ids.Add(result.Id);
            PlayerPrefs.SetString(resultIdsKey, string.Join(",", ids));
            PlayerPrefs.SetString(resultPrefix + result.Id, JsonUtility.ToJson(result));
            PlayerPrefs.Save();
        }


        public UserProfile[] GetAllProfiles()
        {
            return GetExistingProfileIds().Select(s => GetProfile(s)).ToArray();
        }

        public void SaveProfile(UserProfile userProfile)
        {
            if (string.IsNullOrEmpty(userProfile.Id))
            {
                List<string> profileIds = GetExistingProfileIds().ToList();

                userProfile.Id = Guid.NewGuid().ToString();
                profileIds.Add(userProfile.Id);

                PlayerPrefs.SetString(_ProfileIdsCollectionKey, string.Join(",", profileIds));
            }

            PlayerPrefs.SetString(_ProfilePrefix + userProfile.Id, JsonUtility.ToJson(userProfile));
            PlayerPrefs.Save();
        }

        public UserProfile GetProfile(string profileId)
        {
            string json = PlayerPrefs.GetString(_ProfilePrefix + profileId, "{}");
            return JsonUtility.FromJson<UserProfile>(json);
        }

        /*public async Task DeleteCurrentProfile()
        {
            IncrementConnectingLock();
            if (ActiveProfile.IsDefaultProfile)
            {
                return;
            }

            List<string> profileIds = GetExistingProfileIds().ToList();
            profileIds.Remove(ActiveProfile.Id);
            PlayerPrefs.SetString(_ProfileIdsCollectionKey, string.Join(",", profileIds));
            PlayerPrefs.DeleteKey(_ProfilePrefix + ActiveProfile.Id);
            await SetActiveProfile(profileIds[0]);
            DecrementConnectingLock();
        }*/

        public void GetDataExport(out string profilesCsv, out string tangramResultsCsv)
        {
            var stringBuilder = new StringBuilder(20000);

            stringBuilder.AppendLine("Id,FirstName,LastName,IdNumber,DateOfBirth,Location,Language,Gender");
            UserProfile[] profiles = GetAllProfiles();

            foreach (var profile in profiles)
            {
                stringBuilder.AppendLine(
                    $"{profile.Id},{profile.FirstName},{profile.LastName},{profile.IdNumber},{profile.DayOfBirth}/{profile.MonthOfBirth}/{profile.YearOfBirth},{profile.Location},{profile.Language},{profile.Gender}");
            }

            profilesCsv = stringBuilder.ToString();
            stringBuilder.Clear();

            stringBuilder.AppendLine("Id,UserId,SceneName,NumPuzzlePieces,NumCorrectPieces,TimeElapsed,ActiveTime,InactiveTime");
            TangramResult[] tangramResults = GetAllTangramResults();

            foreach (var result in tangramResults)
            {
                stringBuilder.AppendLine(
                    $"{result.Id},{result.UserId},{result.SceneName},{result.NumPuzzlePieces},{result.NumCorrectPieces},{result.TimeElapsed},{result.ActiveTime},{result.InactiveTime}");
            }

            tangramResultsCsv = stringBuilder.ToString();
        }

        public TangramResult[] GetAllTangramResults()
        {
            return GetResultIds(_TangramResultIdsCollectionKey).Select(s => GetTangramResult(s)).ToArray();
        }

        private IEnumerable<string> GetResultIds(string key)
        {
            return PlayerPrefs.GetString(key).Split(',').Where(s => !string.IsNullOrEmpty(s));
        }

        public TangramResult GetTangramResult(string id)
        {
            string json = PlayerPrefs.GetString(_TangramResultPrefix + id, "{}");
            return JsonUtility.FromJson<TangramResult>(json);
        }


        public async Task CreateNewProfile(string email, string password, string firstName, string lastName)
        {
            IncrementConnectingLock();
            var userProfile = new UserProfile
            {

                Gender = UserProfileDialog._DefaultGender.First().text,
                Location = UserProfileDialog._DefaultLocations.First().text,
                Language = UserProfileDialog._DefaultLanguages.First().text,
                YearOfBirth = UserProfileDialog._START_YEAR.ToString(),
                MonthOfBirth = UserProfileDialog._MonthOfBirth.First().text,
                DayOfBirth = "1",
                IsDefaultProfile = false,
            };

            userProfile.FirstName = firstName;
            userProfile.LastName = lastName;
            userProfile.UserName = email;

            //await AwsService.CreateAndSignInUser(
            //        userProfile, password);

            SaveProfile(userProfile);
            ActiveProfile = userProfile;
            DecrementConnectingLock();
        }

        public async Task SetActiveProfile(string profileId)
        {
            // IncrementConnectingLock();
            // var userProfile = GetProfile(profileId);

            // try
            // {
            //     await AwsService.WrapCall(
            //         AwsService.SignInUser,
            //         userProfile
            //     );
            // }
            // catch (Exception e)
            // {
            //     DecrementConnectingLock();
            //     throw e;
            // }

            // AwsService.User.Attributes["given_name"] = userProfile.FirstName;
            // AwsService.User.Attributes["family_name"] = userProfile.LastName;

            // SaveProfile(userProfile);
            // ActiveProfile = userProfile;
            // DecrementConnectingLock();
        }
    }


    public interface IResult
    {
        string Id { get; }
    }
}