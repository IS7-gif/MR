using DG.Tweening;
using UnityEngine;

namespace Project.Scripts.Services.Announcements
{
    public class BoardAnnouncementParams
    {
        public Color? TextColor { get; set; }
        public float? DisplayDuration { get; set; }
        public float? FadeOutDuration { get; set; }
        public float? FlyDistance { get; set; }
        public Ease? FadeOutEase { get; set; }
    }
}