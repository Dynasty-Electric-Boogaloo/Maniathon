using System;
using UnityEngine;

namespace Microgames.Greet.Scripts
{
    public class GuyEmotion : MonoBehaviour
    {
        [Serializable]
        public enum Emotion
        {
            Miffed,
            Happy,
            Sad
        }
        
        [SerializeField] private SpriteRenderer miffedSprite;
        [SerializeField] private SpriteRenderer happySprite;
        [SerializeField] private SpriteRenderer sadSprite;

        private void Start()
        {
            SetEmotion(Emotion.Miffed);
        }
        
        public void SetEmotion(int emotion)
        {
            miffedSprite.enabled = emotion == (int)Emotion.Miffed;
            happySprite.enabled = emotion == (int)Emotion.Happy;
            sadSprite.enabled = emotion == (int)Emotion.Sad;
        }

        public void SetEmotion(Emotion emotion)
        {
            miffedSprite.enabled = emotion == Emotion.Miffed;
            happySprite.enabled = emotion == Emotion.Happy;
            sadSprite.enabled = emotion == Emotion.Sad;
        }
    }
}