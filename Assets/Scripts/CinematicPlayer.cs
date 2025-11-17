using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CinematicPlayer : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer videoPlayer;

    private void Start()
    {
        // RenderTexture 영상 출력 연결
        rawImage.texture = videoPlayer.targetTexture;

        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    private void Update()
    {
        // 스킵 기능
        if (Input.anyKeyDown)
        {
            LoadNextScene();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("TitleScene");
    }
}