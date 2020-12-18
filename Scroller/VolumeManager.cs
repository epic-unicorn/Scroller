using System;
using CoreAudioApi;
using Scroller.Interfaces;
using Scroller.ViewModels;

namespace Scroller
{
    public class VolumeManager : IVolumeManager
    {
        private readonly MMDevice m_Device;

        public float Volume
        {
            get { return m_Device.AudioEndpointVolume.MasterVolumeLevelScalar; }
            private set { 
                if(value != m_Device.AudioEndpointVolume.MasterVolumeLevelScalar)
                {
                    m_Device.AudioEndpointVolume.MasterVolumeLevelScalar = value;
                }                
            }
        }

        public VolumeManager()
        {
            var devEnum = new MMDeviceEnumerator();
            m_Device = devEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            m_Device.AudioEndpointVolume.OnVolumeNotification += OnVolumeNotification;

            // do not remove
            var leaveThis = (int)(m_Device.AudioMeterInformation.MasterPeakValue * 100);
        }

        ~VolumeManager()
        {
            m_Device.AudioEndpointVolume.OnVolumeNotification -= OnVolumeNotification;
        }

        void OnVolumeNotification(AudioVolumeNotificationData data)
        {
            if (OnAudioNotification != null)
            {
                OnAudioNotification.Invoke(this, new AudioVolumeNotificationEventArgs(data));
            }
        }

        float IVolumeManager.CurrentVolume()
        {
            return m_Device.AudioEndpointVolume.MasterVolumeLevelScalar;
        }

        bool IVolumeManager.IsMuted()
        {
            return m_Device.AudioEndpointVolume.Mute;
        }

        /// <summary>
        /// Occurs on audio notification <see cref="AudioVolumeNotificationEventArgs" />
        /// </summary>
        public event EventHandler<AudioVolumeNotificationEventArgs> OnAudioNotification;

        void IVolumeManager.Mute()
        {
            m_Device.AudioEndpointVolume.Mute = true;
        }

        void IVolumeManager.UnMute()
        {
            m_Device.AudioEndpointVolume.Mute = false;
        }

        void IVolumeManager.VolumeUp(UnmuteOptions unmuteOption, float preferredVolume, float value)
        {
            if (!m_Device.AudioEndpointVolume.Mute)
            {
                if ((Volume + value / 100) > 1)
                {
                    Volume = 1;
                }
                else
                {
                    Volume += (value / 100.0f);
                }
            }
            else
            {
                switch (unmuteOption)
                {
                    case UnmuteOptions.DisableScroll:
                        break;
                    case UnmuteOptions.UnmuteScroll:
                        m_Device.AudioEndpointVolume.Mute = false;
                        break;
                    case UnmuteOptions.UnmuteWithPreferredVolume:
                        Volume = preferredVolume / 100;
                        m_Device.AudioEndpointVolume.Mute = false;
                        break;
                }                
            }
        }

        void IVolumeManager.VolumeDown(UnmuteOptions unmuteOption, float preferredVolume, float value)
        {
            if (!m_Device.AudioEndpointVolume.Mute)
            {
                if ((Volume - value/100) < 0)
                {
                    Volume = 0;
                    m_Device.AudioEndpointVolume.Mute = true;
                }
                else
                {
                    Volume -= (value/100.0f);
                }
            }
            else
            {
                switch (unmuteOption)
                {
                    case UnmuteOptions.DisableScroll:
                        break;
                    case UnmuteOptions.UnmuteScroll:
                        if (Volume > 0)
                        {
                            m_Device.AudioEndpointVolume.Mute = false;
                        }
                        break;
                    case UnmuteOptions.UnmuteWithPreferredVolume:
                        Volume = preferredVolume / 100;
                        m_Device.AudioEndpointVolume.Mute = false;
                        break;
                }
            }
        }
    }
}
