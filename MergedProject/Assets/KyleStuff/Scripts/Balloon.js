var riseRate = 1.0f;
var life = 7.0f;

function Start () {
	Destroy(gameObject, life);
}

function Update () {
	transform.localPosition += new Vector3(0,riseRate,0) * Time.deltaTime;
}