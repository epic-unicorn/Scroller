using System;
using CoreAudioApi;

namespace Scroller
{
    public class AudioVolumeNotificationEventArgs : EventArgs
    {
        private readonly AudioVolumeNotificationData m_AudioVolumeNotificationData;

        public AudioVolumeNotificationEventArgs(AudioVolumeNotificationData data)
        {
            m_AudioVolumeNotificationData = data;
        }

        public AudioVolumeNotificationData Data
        {
            get { return m_AudioVolumeNotificationData; }
        }
    }
}
