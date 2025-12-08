using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CinematicPlayer : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer videoPlayer;

    // 스킵 포인트 시간 (초)
    public double[] skipTimes = { 3.0, 5.0, 8.0, 10.0, 12.0, 14.0, 16.0 };

    private void Start()
    {
        rawImage.texture = videoPlayer.targetTexture;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            SkipCinematic();
        }
    }

    private void SkipCinematic()
    {
        double current = videoPlayer.time;

        // 현재 시간 이후의 가장 가까운 스킵 포인트 찾기
        double nextSkipTime = -1;

        foreach (double t in skipTimes)
        {
            if (t > current)
            {
                nextSkipTime = t;
                break;
            }
        }

        // 점프할 포인트 없으면 종료
        if (nextSkipTime < 0)
        {
            LoadNextScene();
            return;
        }

        videoPlayer.time = nextSkipTime;

        Debug.Log($"[Cinematic] Skip → {nextSkipTime} sec");
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("NewTitleScene");
    }
}