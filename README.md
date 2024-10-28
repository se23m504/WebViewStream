# WebView

WebView is a Unity-based HoloLens 2 application for high-quality, low-latency WebRTC streaming from multiple endpoints, optimized with Microsoft's Mixed Reality Toolkit (MRTK) and WebView package. It offers an extensible, high-performance streaming setup that can scale to multiple streams, limited only by the HoloLens 2 hardware.

## Features

- MRTK-Native: Built on MRTK for optimized HoloLens 2 functionality.
- Optimized performance: Configured and fine-tuned for smooth, low-latency streaming on the HoloLens 2.
- WebRTC-based streaming: Uses WebRTC for low-latency, high-quality streaming.
- Dynamic endpoint management: Reads streaming endpoints from a REST API (GetEndpoints)[https://github.com/se23m504/GetEndpoints], simplifying endpoint updates without typing long URLs directly on the HoloLens.
- Extendable streaming capacity: Supports two streams by default and can scale up, limited only by HoloLens 2's hardware.
- Hand menu and speech commands: Offers an intuitive `HandMenu` and voice control for hands-free operation.

## Installation

1. Clone the repository.
2. Configure the API host for `GetEndpoints` to dynamically retrieve streaming endpoints.
3. Build and sideload the application to the HoloLens 2.

## Technical details

- Platform: HoloLens 2 (UWP)
- Language: C# (Unity)
- Required Packages:
    - Mixed Reality Toolkit (MRTK2)
    - Microsoft WebView for Unity

Note: Install dependencies from my (MixedReality)[https://github.com/se23m504/mixedreality] repository. Only use the specific libraries and versions hosted there, as they have been patched for this project's requirements. Using different versions may lead to errors and compatibility issues.

## API requirements

The app requires an external API for endpoint retrieval (`GetEndpoints`), expected to be accessible within the HoloLens 2's network environment.

The application supports local service discovery using UDP broadcasting. This is particularly useful in scenarios where DNS is down or when setting up a DNS record is not feasible. 

