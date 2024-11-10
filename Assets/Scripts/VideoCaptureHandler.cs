using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows.WebCam;
using System.IO;

public class VideoCaptureHandler : MonoBehaviour
{
    private VideoCapture videoCapture = null;

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

            this.videoCapture.StartVideoModeAsync(cameraParameters,
                                                VideoCapture.AudioState.None,
                                                OnStartedVideoCaptureMode);
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
            string filename = string.Format("WebView_{0}.mp4", Time.time);
            string filepath = Path.Combine(Application.persistentDataPath, filename);
            Debug.Log("Saving Video to: " + filepath);

            videoCapture.StartRecordingAsync(filepath, OnStartedRecordingVideo);
        }
    }

    private void OnStartedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Started Recording Video");
    }

    public void StopRecordingVideo()
    {
        Debug.Log("Stopping Video Recording");
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
