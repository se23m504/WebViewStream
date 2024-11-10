using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;
#if WINDOWS_UWP && !UNITY_EDITOR
using Windows.Storage;
#endif

public class VideoCaptureHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject recordingStatus = null;

    private Coroutine blinkCoroutine = null;
    private Image recordingImage = null;
    private VideoCapture videoCapture = null;

    private IEnumerator BlinkLightRed()
    {
        if (recordingImage == null)
        {
            recordingImage = recordingStatus.GetComponentInChildren<Image>();
        }

        Color lightRed = new Color(1, 0.3f, 0.3f, 0.6f);
        Color transparent = new Color(1, 0.3f, 0.3f, 0);

        while (true)
        {
            recordingImage.color = lightRed;
            yield return new WaitForSeconds(0.5f);

            recordingImage.color = transparent;
            yield return new WaitForSeconds(0.5f);
        }
    }

#if WINDOWS_UWP && !UNITY_EDITOR
    private const string freeSpace = "System.FreeSpace";
    private const UInt64 minAvailableSpace = 5UL * 1024 * 1024 * 1024; // 5GB

    private IEnumerator CheckAvailableStorageSpace()
    {
        while (videoCapture != null && videoCapture.IsRecording)
        {
            yield return CheckSpaceAndHandleRecording();
            yield return new WaitForSeconds(5);
        }
    }

    private async Task CheckSpaceAndHandleRecording()
    {
        try
        {
            StorageFolder folder = ApplicationData.Current.TemporaryFolder;
            var props = await folder.Properties.RetrievePropertiesAsync(new string[] { freeSpace });
            UInt64 availableSpace = (UInt64)props[freeSpace];

            if (availableSpace < minAvailableSpace)
            {
                Debug.LogWarning("Not enough storage space to continue recording. Saving video.");
                videoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error checking storage space: " + ex.Message);
        }
    }
#endif

    public void StartRecordingVideo()
    {
#if WINDOWS_UWP && !UNITY_EDITOR
        VideoCapture.CreateAsync(true, OnVideoCaptureCreated);
#else
        VideoCapture.CreateAsync(false, OnVideoCaptureCreated);
#endif
    }

    public void ToggleRecordingVideo()
    {
        if (videoCapture == null)
        {
            StartRecordingVideo();
        }
        else if (videoCapture.IsRecording)
        {
            StopRecordingVideo();
        }
    }

    private void OnVideoCaptureCreated(VideoCapture videoCapture)
    {
        if (videoCapture != null)
        {
            this.videoCapture = videoCapture;

            Resolution cameraResolution = new Resolution();
            foreach (Resolution resolution in VideoCapture.SupportedResolutions)
            {
                if (resolution.width * resolution.height > cameraResolution.width * cameraResolution.height)
                {
                    cameraResolution = resolution;
                }
            }

            float cameraFramerate = 0.0f;
            foreach (float framerate in VideoCapture.GetSupportedFrameRatesForResolution(cameraResolution))
            {
                if (framerate > cameraFramerate)
                {
                    cameraFramerate = framerate;
                }
            }

            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.75f;
            cameraParameters.frameRate = cameraFramerate;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            this.videoCapture.StartVideoModeAsync(
                cameraParameters,
                VideoCapture.AudioState.ApplicationAndMicAudio,
                OnStartedVideoCaptureMode
            );
        }
        else
        {
            Debug.LogError("Failed to create VideoCapture Instance");
        }
    }

    private void OnStartedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        if (result.success)
        {
            string filename = string.Format("WebView_{0}.mp4", DateTime.UtcNow.ToString("yyyy-MM-ddTHHmmssZ"));
            string filepath = Path.Combine(Application.persistentDataPath, filename);
            Debug.Log("Saving Video to: " + filepath);

            videoCapture.StartRecordingAsync(filepath, OnStartedRecordingVideo);
        }
    }

    private void OnStartedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Started Recording Video");
#if WINDOWS_UWP && !UNITY_EDITOR
        StartCoroutine(CheckAvailableStorageSpace());
#endif
        recordingStatus.SetActive(true);
        blinkCoroutine = StartCoroutine(BlinkLightRed());
    }

    public void StopRecordingVideo()
    {
        Debug.Log("Stopping Video Recording");
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        recordingStatus.SetActive(false);
        videoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
    }

    private void OnStoppedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Stopped Recording Video");
        videoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
    }

    private void OnStoppedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        videoCapture.Dispose();
        videoCapture = null;
    }
}
