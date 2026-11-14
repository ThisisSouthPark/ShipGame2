using UnityEngine;
namespace PirateGame.Juice
{
    public sealed class CombatAudioFeedback : MonoBehaviour
    {
        // 실습 TODO: ShotFired/Hit/WaterMiss/Sunk 이벤트별로 AudioSource.PlayOneShot을 호출한다. 볼륨/피치 변화, 연속 소리 제한, HitStop 중 소리 유지 정책을 구현한다.
        // 구현은 Docs/03_JuiceTutorial.md를 따라 직접 추가합니다.
        public AudioSource source;
        public AudioClip shot, hit, splash, sink;
        [Range(0, 1)] public float volume = 0.55f;
        public float minimumInterval = 0.035f;
        CombatFeedbackEvents events;
        // Each sound family has its own limit; a shot never suppresses a sinking sound.
        readonly float[] last = { -100, -100, -100, -100 };
        void OnEnable()
        {
            events = FindFirstObjectByType<CombatFeedbackEvents>();
            if (events)
            {
                events.ShotFired += OnShot; events.Hit += OnHit;
                events.WaterMiss += OnMiss; events.Sunk += OnSink;
            }
        }
        void Play(AudioClip clip, int channel)
        {
            if (!clip || !source || Time.unscaledTime - last[channel] < minimumInterval) return;
            last[channel] = Time.unscaledTime;
            source.pitch = Random.Range(0.95f, 1.05f);
            source.PlayOneShot(clip, volume);
        }
        void OnShot(ShotFeedback value) => Play(shot, 0);
        void OnHit(HitFeedback value) => Play(hit, 1);
        void OnMiss(Vector2 value) => Play(splash, 2);
        void OnSink(SinkFeedback value) => Play(sink, 3);
        void OnDisable()
        {
            if (events)
            {
                events.ShotFired -= OnShot; events.Hit -= OnHit;
                events.WaterMiss -= OnMiss; events.Sunk -= OnSink;
            }
            if (source) source.Stop();
        }
    }
}
