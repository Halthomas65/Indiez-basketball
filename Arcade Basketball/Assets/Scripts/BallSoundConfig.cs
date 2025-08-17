using UnityEngine;

[CreateAssetMenu(fileName = "BallSoundConfig", menuName = "Basketball/Ball Sound Config")]
public class BallSoundConfig : ScriptableObject
{
    [Header("Ball Collision Sounds")]
    public AudioClip[] ballCollisionSounds;
    public AudioClip[] rimCollisionSounds;
    public AudioClip[] backboardCollisionSounds;
    
    [Header("Scoring Sounds")]
    public AudioClip swishSound;        // Clean basket
    public AudioClip bounceInSound;     // Bounce then score
    public AudioClip dunkSound;         // Perfect dunk
}