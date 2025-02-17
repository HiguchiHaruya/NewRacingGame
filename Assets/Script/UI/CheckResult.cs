using Cysharp.Threading.Tasks;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
/// <summary>
/// ゴール後のイベント
/// </summary>
public class CheckResult : MonoBehaviour
{
    [Header("UI関連")]
    [SerializeField] TMP_Text _text1;
    [SerializeField] TMP_Text _text2;
    [SerializeField] TMP_Text _highScoreText;
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] GameObject _panel;
    [SerializeField] Button _spectatorButton;
    Camera _camera;
    int _highScore;
    int _minute;
    int _second;
    bool _isAlone;
    string _errorMessage = "スコア記録されていません";
    private PostProcessVolume _processVolume;
    ReactiveProperty<bool> _spectatorMode = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> SpectatorMode => _spectatorMode;
    private async void Start()
    {
        _isAlone = PhotonNetwork.PlayerList.Length < 2;
        _panel.gameObject.SetActive(false);
        GameManager.Instance.IsGameEnd
            .Where(e => e)
            .Subscribe(_ => SetResultText())
            .AddTo(this);
        if (await EnsureLoggin())
        {
            GetHighScore();
        }
    }
    private void SetResultText()
    {

        _panel.gameObject.SetActive(true);
        if (_isAlone)
        {
            _spectatorButton.gameObject.SetActive(false);
        }
        else
        {
            _spectatorButton.onClick.AddListener(() => DestroyCamera());
        }
        TextAnimation.Instance.LTextAnimation("歴代最速記録", _text1);
        TextAnimation.Instance.LTextAnimation("今回のタイム", _text2);
        TextAnimation.Instance.LTextAnimation(ConvertSecondsToTime(_highScore).ToString(), _highScoreText);
    }
    public void GetCamera(Camera camera)
    {
        _camera = camera;
    }
    private void DestroyCamera()
    {
        if (!_isAlone)
        {
            var s = gameObject.GetComponent<SpectatorCamera>();
            s.SwitchCamera(0);
            _spectatorMode.Value = true;
            _panel.gameObject.SetActive(false);
            BackPostEffect();
        }
    }

    public void ChangePostEffect(PostProcessVolume postProcessVolume)
    {
        _processVolume = postProcessVolume;
        if (_processVolume.profile.TryGetSettings<DepthOfField>(out var data))
        {
            data.focusDistance.value = 0.1f;
        }
        if (_processVolume.profile.TryGetSettings<LensDistortion>(out var lens))
        {
            lens.intensity.value = -100;
        }
    }

    public void BackPostEffect()
    {
        if (_processVolume.profile.TryGetSettings<DepthOfField>(out var data))
        {
            data.focusDistance.value = 10f;
        }
        if (_processVolume.profile.TryGetSettings<LensDistortion>(out var lens))
        {
            lens.intensity.value = 10;
        }

    }
    public void SetSocre(int m, int s)
    {
        TextAnimation.Instance.LTextAnimation($"{m:D2} : {s:D2}", _scoreText);
    }
    private void GetHighScore()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "RaceTime",
        };
        PlayFabClientAPI.GetLeaderboard(request, result =>
        {
            if (result.Leaderboard.Count > 0)
            {
                var h = result.Leaderboard.OrderBy(x => x.StatValue).ToList();
                _highScore = h[0].StatValue;
            }
        },
        error =>
        {
            Debug.Log("スコア取得失敗");
        });
    }

    private async UniTask<bool> EnsureLoggin()
    {
        if (PlayFabClientAPI.IsClientLoggedIn()) return true; //既にログイン済みならreturn
        var request = new PlayFab.ClientModels.LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
        };
        var tcs = new UniTaskCompletionSource<bool>();
        PlayFabClientAPI.LoginWithCustomID(request,
            result => tcs.TrySetResult(true),
            error =>
            {
                Debug.Log("ログイン失敗");
                tcs.TrySetResult(false);
            }
            );

        var success = await tcs.Task;
        if (success == true) { return true; }
        return false;
    }
    private string ConvertSecondsToTime(int seconds)
    {
        int m = seconds / 60;
        int s = seconds % 60;
        return $"{m:D2} : {s:D2}";
    }
}
