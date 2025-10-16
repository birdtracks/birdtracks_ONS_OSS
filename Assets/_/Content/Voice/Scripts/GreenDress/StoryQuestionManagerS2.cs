using System;
using System.Collections;
using System.Collections.Generic;
using BirdTracks.Game.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

namespace Birdtracks.Game.ONS.GreenDress
{
    public class StoryQuestionManagerS2 : MonoBehaviour, INotificationReceiver
    {
        [SerializeField] private LoadingScreen m_LoadingScreen = default;
        [SerializeField] private PlayableDirector m_PlayableDirector;
        public GameEvent gameEvent;
        [SerializeField] private string _gameName;
        [SerializeField] private BoolGameVariable _isSeswati;
        [SerializeField] private List<StoryQuestionData> _questionData = new List<StoryQuestionData>();
        [SerializeField] private List<Image> _anwserImages = new List<Image>();
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _answerSound;
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private Animator TortoiseAnimator;
        [SerializeField] private ONS_CharacterLipSync _lipSync;
        [SerializeField] private Canvas _answerCanvas;
        
        [SerializeField] private float _tintDuration = 0.5f;
        [SerializeField] private Color _tintColor = new Color(1f, 1f, 1f, 0.5f);
        [SerializeField] private bool _randomizeAnswerPositions = false;
        [SerializeField] private float _animationDuration = 0.5f;
        
        [SerializeField] private float totalListenTime = 30f;
        [SerializeField] private float stageInterval = 5f;
        
        private int _currentStage = 0;
        
        private Color[] _originalColors;
        private Coroutine[] _tintCoroutines;

        public bool _isAwaitingAnswer = false;
        public bool _runInEditor = true;
        private bool _inTap = false;
        private List<QuestionData> _currentAnswerOrder = new List<QuestionData>();
        private Random _random = new Random();
        private Queue<StoryQuestionData> _waitingQuestions = new Queue<StoryQuestionData>();
        private StoryQuestionData _currentQuestion;
        
        private Coroutine _delayNextQuestion;
        private Coroutine _timer;
        private Coroutine _hideShowCoroutine;
        private Coroutine _acknowledgeCoroutine;
        private bool _isFirstQuestion = true;

        private Language _language;

        private void Awake()
        {
            m_LoadingScreen.FadeIn();
            
            _originalColors = new Color[_anwserImages.Count];
            _tintCoroutines = new Coroutine[_anwserImages.Count];
            for (int i = 0; i < _anwserImages.Count; i++)
            {
                _originalColors[i] = _anwserImages[i].color;
            }
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void Start()
        {
            _language = _isSeswati.Value ? Language.Seswati : Language.English;
            
             m_PlayableDirector.Play();
        }

        private void SetupAnswers()
        {
            if (_timer != null)
            {
                StopCoroutine(_timer);
                _timer = null;
            }
            
            ClearAllSubscriptions();
            
            _currentAnswerOrder.Clear();
            
            //if we have no more questions left we have completed this game.
            if (_waitingQuestions.Count == 0)
            {
                OnGameComplete();
                return;
            }
            
            _isFirstQuestion = false;
            _currentQuestion = _waitingQuestions.Dequeue();

            Debug.Log("questions remaining = " + _waitingQuestions.Count);
            if (_currentQuestion.questions.Count < _anwserImages.Count)
            {
                Debug.Log("not enough answer data for the number of possible answers in UI");
                return;
            }

            List<QuestionData> answersToPlace = _randomizeAnswerPositions ? 
                RandomizeQuestions(_currentQuestion.questions) : 
                new List<QuestionData>(_currentQuestion.questions);

            for (int i = 0; i < answersToPlace.Count; i++)
            {
                _anwserImages[i].sprite = answersToPlace[i].Image;
                _anwserImages[i].GetComponent<StoryQuestionDataHolder>().SetQuestionData(answersToPlace[i]);
                _currentAnswerOrder.Add(answersToPlace[i]);
                _anwserImages[i].GetComponent<StoryQuestionDataHolder>().OnClick += OnAnswerSelected;
                
                // For first question, start images invisible
                // if (_isFirstQuestion)
                // {
                //     var color = _anwserImages[i].color;
                //     color.a = 0f;
                //     _anwserImages[i].color = color;
                // }
            }
            
            SetQuestionAudio();
            
            // For subsequent questions (after hide/show transition), fade images back in
            if (!_isFirstQuestion)
            {
                StartCoroutine(AnimateImagesBackIn());
            }
            
            _timer = StartCoroutine(ListenTimerCoroutine());
        }

        private void OnGameComplete()
        {
            SaveTapDataUtil.CreateNewFile(_gameName);
            SaveTapDataUtil.SaveAllData();
            _answerCanvas.enabled = false;
                
            gameEvent.Invoke();
        }

        private void ClearAllSubscriptions()
        {
            foreach (var image in _anwserImages)
            {
                var holder = image.GetComponent<StoryQuestionDataHolder>();
                if (holder != null)
                {
                    holder.OnClick -= OnAnswerSelected;
                }
            }
        }
        
        private List<QuestionData> RandomizeQuestions(List<QuestionData> questions)
        {
            List<QuestionData> randomized = new List<QuestionData>();
            List<QuestionData> tempPool = new List<QuestionData>(questions);

            while (tempPool.Count > 0)
            {
                int randomIndex = _random.Next(0, tempPool.Count);
                randomized.Add(tempPool[randomIndex]);
                tempPool.RemoveAt(randomIndex);
            }

            return randomized;
        }

        private void OnAnswerSelected(GameObject touchedObj)
        {
            if (_inTap) return;

            if (_currentQuestion == null) return;

            _inTap = true;
            
            int imageIndex = _anwserImages.FindIndex(img => img.gameObject == touchedObj);
    
            if (imageIndex >= 0)
            {
                if (_tintCoroutines[imageIndex] != null)
                    StopCoroutine(_tintCoroutines[imageIndex]);
                _tintCoroutines[imageIndex] = StartCoroutine(TintEffect(imageIndex));
            }

            var dataHolder = touchedObj.GetComponent<StoryQuestionDataHolder>();
            
            if (dataHolder == null)
            {
                _inTap = false;
                return;
            }

            var newGameDataEntry = new GameData
            {
                gameName = _gameName,
                question = _currentQuestion.questionText,
                selectedAnswer = touchedObj.name,
                answeredCorrectly = dataHolder.IsCorrectAnswer()
            };

            SaveTapDataUtil.AddGameData(newGameDataEntry);
            _inTap = false;
            if (_delayNextQuestion != null)
            {
                StopCoroutine(_delayNextQuestion);
                _delayNextQuestion = null;
            }
            _delayNextQuestion = StartCoroutine(DelayNextQuestion());
        }
        
        //this simulated click pretends to be a real interaction and so triggers the usual process
        public void SimulateClick(GameObject targetObject)
        {
            Debug.Log("simulate click");

            OnAnswerSelected(targetObject);
        }

        //this fake click does not trigger the answer selected system, only tints the answered question
        public void FakeClick(int imageIndex)
        {
            if (_tintCoroutines[imageIndex] != null)
                StopCoroutine(_tintCoroutines[imageIndex]);
            _tintCoroutines[imageIndex] = StartCoroutine(TintEffect(imageIndex));
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            
        }

        public void InitializeQuestions()
        {
            Debug.Log("initialized questions");
            _answerCanvas.enabled = true;
            PrepareData();
            SetupAnswers();
        }

        private void PrepareData()
        {
            Debug.Log("preparing data");

            if (_waitingQuestions.Count > 0)
            {
                Debug.Log("enough in queue");
                return;
            }

            foreach (var q in _questionData)
            {
                _waitingQuestions.Enqueue(q);
            }
        }
        
        private IEnumerator TintEffect(int imageIndex)
        {
            Image image = _anwserImages[imageIndex];
            float elapsed = 0;
    
            image.color = _tintColor;

            PlayAnswerAudio();
    
            while (elapsed < _tintDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _tintDuration;
                image.color = Color.Lerp(_tintColor, _originalColors[imageIndex], t);
                yield return null;
            }
    
            image.color = _originalColors[imageIndex];
            
            // After tint completes, start the hide/show transition for all questions after initial setup
            StartHideShowTransition();
        }

        private void PlayAnswerAudio()
        {
            _audioSource.clip = _answerSound;
            _audioSource.Play();
        }

        private IEnumerator DelayNextQuestion()
        {
            TortoiseAnimator.SetTrigger("Answer");
            
            yield return new WaitForSecondsRealtime(2f);
            
            // All questions now use the hide/show transition handled by TintEffect
        }
        
        private void FirstNudge()
        {
            Debug.Log("first nudge (animation)");
            TortoiseAnimator.SetTrigger("Prompt");

        }

        private void SecondNudge()
        {
            Debug.Log("second nudge (animation & audio)");

            TortoiseAnimator.SetTrigger("Prompt");
            SetQuestionNudgeAudio();
            SetQuestionNudgeText();
        }
        
        private void SetQuestionAudio()
        {
            AudioClip clip = GetQuestionClip();
            _audioSource.clip = clip;
            _audioSource.Play();
            if (_lipSync.gameObject.activeInHierarchy)
            {
                _lipSync.PlayLipSync(clip.length);
            }

            StartCoroutine(QuestionCompletion(clip.length));
        }
        
        private AudioClip GetQuestionClip()
        {
            return _language == Language.English
                ? _currentQuestion.ENGQuestionAudioClip
                : _currentQuestion.SSWQuestionAudioClip;
        }
        
        private void SetQuestionNudgeAudio()
        {
            AudioClip clip = GetPromptClip();
            _audioSource.clip = clip;
            _audioSource.Play();
            _lipSync.PlayLipSync(clip.length);
        }
        
        private AudioClip GetPromptClip()
        {
            return _language == Language.English
                ? _currentQuestion.ENGPromptQuestionAudioClip
                : _currentQuestion.SSWPromptQuestionAudioClip;
        }

        private void SetQuestionNudgeText()
        {
            //_subtitleText.text = _currentQuestionData.NudgeQuestionSubtitle;
        }
        
        private IEnumerator ListenTimerCoroutine()
        {
            float elapsedTime = 0f;
            _currentStage = 0;

            while (elapsedTime < totalListenTime)
            {
                if (!_runInEditor)
                {
                    Debug.Log("run in editor flag is false");
                    yield break;
                }
                // Update time and invoke time update event
                elapsedTime += Time.deltaTime;

                // Check stages and trigger appropriate actions
                if (_currentStage == 0 && elapsedTime >= stageInterval)
                {
                    _currentStage++;
                    FirstNudge();
                    Debug.Log("First stage passed - First nudge");
                }
                else if (_currentStage == 1 && elapsedTime >= stageInterval * 2)
                {
                    _currentStage++;
                    SecondNudge();
                    Debug.Log("Second stage passed - Second nudge");
                }

                yield return null;
            }

            // Final timeout
            _isAwaitingAnswer = false;
            StartCoroutine(DelayNextQuestion());
        }

        private IEnumerator QuestionCompletion(float waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            
            TortoiseAnimator.SetTrigger("QuestionComplete");

            SetAnswerImagesInteractable();
        }

        private void SetAnswerImagesInteractable()
        {
            for (int i = 0; i < _anwserImages.Count; i++)
            {
                if (_anwserImages[i] != null)
                {
                    var holder = _anwserImages[i].GetComponent<StoryQuestionDataHolder>();
                    if (holder != null)
                    {
                        holder.SetHolderInteractable();
                    }
                }
            }
        }
        
        private IEnumerator AnimateImagesIn()
        {
            float elapsed = 0;
            Color[] targetColors = new Color[_anwserImages.Count];
            
            // Store target colors (original colors with full alpha)
            for (int i = 0; i < _anwserImages.Count; i++)
            {
                targetColors[i] = _originalColors[i];
            }
            
            while (elapsed < _animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _animationDuration;
                
                for (int i = 0; i < _anwserImages.Count; i++)
                {
                    Color currentColor = _anwserImages[i].color;
                    currentColor.a = Mathf.Lerp(0f, targetColors[i].a, t);
                    _anwserImages[i].color = currentColor;
                }
                
                yield return null;
            }
            
            // Ensure final alpha values are correct
            for (int i = 0; i < _anwserImages.Count; i++)
            {
                _anwserImages[i].color = targetColors[i];
            }
        }
        
        private void StartHideShowTransition()
        {
            if (_hideShowCoroutine != null)
            {
                StopCoroutine(_hideShowCoroutine);
            }
            _hideShowCoroutine = StartCoroutine(HideShowTransition());
        }
        
        private IEnumerator HideShowTransition()
        {
            yield return StartCoroutine(AnimateImagesOut());
            
            yield return new WaitForSecondsRealtime(1f);
            
            if (!_isFirstQuestion)
            {
                SetupAnswers();
            }

            //yield return StartCoroutine(AcknowledgePlayerResponse());
        }


        private IEnumerator AcknowledgePlayerResponse()
        {
            if (_language == Language.Seswati)
            {
                _audioSource.clip = _currentQuestion.SSWAcknowledgeAnswerAudioClipP1;
                _audioSource.Play();
            }
            else
            {
                _audioSource.clip = _currentQuestion.ENGAcknowledgeAnswerAudioClipP1;
                _audioSource.Play();
            }
            
            
            if (_language == Language.Seswati)
            {
                yield return new WaitForSecondsRealtime(_currentQuestion.SSWAcknowledgeAnswerAudioClipP1.length);
            }
            else
            {
                yield return new WaitForSecondsRealtime(_currentQuestion.ENGAcknowledgeAnswerAudioClipP1.length);
            }

            if (_language == Language.Seswati)
            {
                _audioSource.clip = _currentQuestion.SSWAcknowledgeAnswerAudioClipP2;
                _audioSource.Play();
            }
            else
            {
                _audioSource.clip = _currentQuestion.ENGAcknowledgeAnswerAudioClipP2;
                _audioSource.Play();
            }
            
            _currentQuestion = null;
        }

        private IEnumerator AnimateImagesOut()
        {
            float elapsed = 0;
            Color[] startColors = new Color[_anwserImages.Count];
            
            // Store current colors
            for (int i = 0; i < _anwserImages.Count; i++)
            {
                startColors[i] = _anwserImages[i].color;
            }
            
            while (elapsed < _animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _animationDuration;
                
                for (int i = 0; i < _anwserImages.Count; i++)
                {
                    Color currentColor = startColors[i];
                    currentColor.a = Mathf.Lerp(startColors[i].a, 0f, t);
                    _anwserImages[i].color = currentColor;
                }
                
                yield return null;
            }
            
            // Ensure final alpha is 0
            for (int i = 0; i < _anwserImages.Count; i++)
            {
                Color currentColor = _anwserImages[i].color;
                currentColor.a = 0f;
                _anwserImages[i].color = currentColor;
            }
        }
        
        private IEnumerator AnimateImagesBackIn()
        {
            float elapsed = 0;
            
            while (elapsed < _animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _animationDuration;
                
                for (int i = 0; i < _anwserImages.Count; i++)
                {
                    Color currentColor = _anwserImages[i].color;
                    currentColor.a = Mathf.Lerp(0f, _originalColors[i].a, t);
                    _anwserImages[i].color = currentColor;
                }
                
                yield return null;
            }
            
            // Ensure final alpha values are correct
            for (int i = 0; i < _anwserImages.Count; i++)
            {
                _anwserImages[i].color = _originalColors[i];
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _anwserImages.Count; i++)
            {
                if (_anwserImages[i] != null)
                {
                    var holder = _anwserImages[i].GetComponent<StoryQuestionDataHolder>();
                    if (holder != null)
                    {
                        holder.OnClick -= OnAnswerSelected;
                    }
                }
            }
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }
}
