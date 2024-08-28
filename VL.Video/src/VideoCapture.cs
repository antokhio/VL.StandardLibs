﻿#nullable enable
using Stride.Core.Mathematics;
using System;
using System.Reactive.Subjects;
using System.Threading;
using VL.Lib.Basics.Resources;
using VL.Lib.Basics.Video;
using VL.Video.CaptureControl;

namespace VL.Video
{
    public sealed partial class VideoCapture : IVideoSource2
    {
        private readonly object syncRoot = new();

        private string? deviceLink;
        private Int2 preferredSize;
        private float preferredFps;
        private int changeTicket;
        private VideoCaptureImpl? currentCapture;
        private bool enabled;

        public VideoCaptureDeviceEnumEntry? Device
        {
            set
            {
                var deviceLink = value?.Tag as string;
                if (deviceLink != this.deviceLink)
                {
                    this.deviceLink = deviceLink;
                    changeTicket++;
                }
            }
        }

        public Int2 PreferredSize 
        { 
            set
            {
                if (value != preferredSize)
                {
                    preferredSize = value;
                    changeTicket++;
                }
            }
        }

        public float PreferredFps 
        { 
            set
            {
                if (value != preferredFps)
                {
                    preferredFps = value;
                    changeTicket++;
                }
            }
        }

        public CameraControls CameraControls
        {
            set
            {
                var v = value ?? CameraControls.Default;
                if (v != cameraControls.Value)
                    cameraControls.OnNext(v);
            }
        }
        readonly BehaviorSubject<CameraControls> cameraControls = new BehaviorSubject<CameraControls>(CameraControls.Default);

        public VideoControls VideoControls
        {
            set
            {
                var v = value ?? VideoControls.Default;
                if (v != videoControls.Value)
                    videoControls.OnNext(v);
            }
        }
        readonly BehaviorSubject<VideoControls> videoControls = new BehaviorSubject<VideoControls>(VideoControls.Default);

        public bool Enabled
        {
            get => enabled;
            set
            {
                if (value != enabled)
                {
                    enabled = value;
                    changeTicket++;
                }
            }
        }

        public float ActualFPS => currentCapture?.ActualFPS ?? default;

        public string SupportedFormats => currentCapture?.SupportedFormats ?? string.Empty;

        int IVideoSource2.ChangedTicket => changeTicket;

        IVideoPlayer? IVideoSource2.Start(VideoPlaybackContext ctx)
        {
            lock (syncRoot)
            {
                if (currentCapture != null && !enabled)
                    return null;

                var config = new VideoCaptureConfig(deviceLink, preferredSize, preferredFps, cameraControls, videoControls);
                if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                {
                    var device = ctx.GraphicsDeviceType == GraphicsDeviceType.Direct3D11 ? ctx.GraphicsDevice : default;
                    var capture = MF.MFVideoCaptureImpl.Create(config, device);
                    if (capture is null)
                        return null;

                    capture.VideoCapture = this;
                    capture.DisposeAction = () =>
                    {
                        currentCapture = null;
                        // Let other sinks try to resubscribe
                        changeTicket++;
                    };
                    return currentCapture = capture;
                }

                throw new PlatformNotSupportedException();
            }
        }
    }
}
