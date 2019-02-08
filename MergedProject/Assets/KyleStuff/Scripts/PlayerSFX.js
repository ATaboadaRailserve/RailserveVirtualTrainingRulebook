#pragma strict

public var footSteps : AudioSource;

function Update () {
	if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")) {
		if (!footSteps.isPlaying) {
			footSteps.Play();
		}
	} else
		footSteps.Stop();
	
	if (Input.GetKey("s"))
		footSteps.pitch = 0.8f;
	else
		footSteps.pitch = 1.0f;
}