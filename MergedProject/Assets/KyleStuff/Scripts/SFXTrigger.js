#pragma strict

public var sound : AudioSource;
public var playOnce : boolean;

function OnTriggerEnter (col : Collider) {
	if (col.gameObject.name == "Player") {
		if (!sound.isPlaying) {
			print ("Playing: " + sound.clip.name);
			sound.Play();
		}
		if (playOnce) {
			Destroy(gameObject);
		}
	}
}