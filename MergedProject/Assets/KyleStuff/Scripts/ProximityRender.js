#pragma strict

var camObj : GameObject;
var renderDistance = 5.0f;
var isText = true;
var text : UI.Text;
var curve = new AnimationCurve(Keyframe(0, 0), Keyframe(1, 1));

private var color : Color;

function Start () {
	if (isText) {
		color = text.color;
	} else {
		color = gameObject.GetComponent.<Renderer>().material.color;
	}
}

function Update () {
	var distance = (transform.position - camObj.transform.position).magnitude;
	var normalizeDistance = (renderDistance-distance)/renderDistance;
	normalizeDistance = Mathf.Clamp(normalizeDistance, 0.0f, 1.0f);
	color.a = curve.Evaluate(normalizeDistance);
	if (isText) {
		text.color = color;
	} else {
		gameObject.GetComponent.<Renderer>().material.color = color;
	}
}