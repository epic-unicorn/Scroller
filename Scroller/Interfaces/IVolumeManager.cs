using System;
using Scroller.ViewModels;

namespace Scroller.Interfaces
{
    public interface IVolumeManager
    {
        /// <summary>
        /// Get the current volume
        /// </summary>
        /// <returns></returns>
        float CurrentVolume();
        
        /// <summary>
        /// Volumes up.
        /// </summary>
        /// <param name="unmuteOption">The unmute option.</param>
        /// <param name="preferredVolume"> </param>
        /// <param name="value">The value.</param>
        void VolumeUp(UnmuteOptions unmuteOption, float preferredVolume, float value = 1);

        /// <summary>
        /// Volumes down.
        /// </summary>
        /// <param name="unmuteOption">The unmute option.</param>
        /// <param name="preferredVolume"> </param>
        /// <param name="value">The value.</param>
        void VolumeDown(UnmuteOptions unmuteOption, float preferredVolume, float value = 1);

        /// <summary>
        /// Mute the audio
        /// </summary>
        void Mute();

        /// <summary>
        /// Unmute the audio
        /// </summary>
        void UnMute();

        /// <summary>
        /// Check if the audio is muted
        /// </summary>
        /// <returns>
        ///   <c>true</c> if audio is muted; otherwise, <c>false</c>.
        /// </returns>
        bool IsMuted();

        /// <summary>
        /// Occurs on audio notification <see cref="AudioVolumeNotificationEventArgs"/>
        /// </summary>
        event EventHandler<AudioVolumeNotificationEventArgs> OnAudioNotification;
    }
}