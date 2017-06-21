using UnityEngine;
using System.Collections;

public class SmoothCamera2D : MonoBehaviour {

    public float cameraOffsetMaxAllowed = 6.0f;
	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;
	Camera myCamera;
	float speed = 10;

	Builder GM;

	public float zoomSpeed = 1;
	public float targetOrtho;
	public float smoothSpeed = 5.0f;
	public float minOrtho = 1.0f;
	public float maxOrtho = 30.0f;

    float maxOrthoGame;
    float maxOrthoBuilding = 10f;

	//Debug
	bool moveUp = false;
	bool moveDown = false;
	bool moveRight = false;
	bool moveLeft = false;

	bool resetingPosition = false;

    Vector3 old_position;
    float old_ortho;

    float x_pos_building_mode = 1.5f;
	void Awake()
	{
		myCamera = GetComponent<Camera> ();
		GM = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<Builder> ();
	}

	void Start()
	{
        maxOrthoGame = maxOrtho;
		targetOrtho = myCamera.orthographicSize;
	}
	void Update () 
	{
		if (target)
		{
			Vector3 point = myCamera.WorldToViewportPoint(target.position);
			Vector3 delta = target.position - myCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}

		if (GM.buildMode) {
			if (!resetingPosition) {
				if (Input.GetKey (KeyCode.D) || moveRight) {
                    if (transform.position.x <= cameraOffsetMaxAllowed)
					transform.Translate (new Vector3 (speed * Time.deltaTime, 0, 0));
				}
                if (Input.GetKey(KeyCode.A) || moveLeft)
                {
                    if (transform.position.x >= -cameraOffsetMaxAllowed)
					transform.Translate (new Vector3 (-speed * Time.deltaTime, 0, 0));
				}
                if (Input.GetKey(KeyCode.S) || moveDown)
                {
                    if (transform.position.y >= -cameraOffsetMaxAllowed-1)
					transform.Translate (new Vector3 (0, -speed * Time.deltaTime, 0));
				}
                if (Input.GetKey(KeyCode.W) || moveUp)
                {
                    if (transform.position.y <= cameraOffsetMaxAllowed+1)
					transform.Translate (new Vector3 (0, speed * Time.deltaTime, 0));
				}
			}
		}

		float scroll = Input.GetAxis ("Mouse ScrollWheel");
		if (scroll != 0.0f) {
			targetOrtho -= scroll * zoomSpeed;
			targetOrtho = Mathf.Clamp (targetOrtho, minOrtho, maxOrtho);
		}

		myCamera.orthographicSize = Mathf.MoveTowards (myCamera.orthographicSize, targetOrtho, smoothSpeed * Time.deltaTime);

		if (resetingPosition) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(x_pos_building_mode, 0, -10), 5 * Time.deltaTime);
            if (transform.position.x <= x_pos_building_mode + 0.02f && transform.position.x >= x_pos_building_mode - 0.02f && transform.position.y <= 0.02f && transform.position.y >= -0.02f)
            {
                transform.position = new Vector3(x_pos_building_mode, 0, -10);
				targetOrtho = 6;
				resetingPosition = false;
			}
		}

	}

	public void ZoomIn()
	{
		targetOrtho -= 1 * 2.5f;
		targetOrtho = Mathf.Clamp (targetOrtho, minOrtho, maxOrtho);
	}
	public void ZoomOut()
	{
		targetOrtho -= -1 * 2.5f;
		targetOrtho = Mathf.Clamp (targetOrtho, minOrtho, maxOrtho);
	}

	public void MoveUp(bool condition)
	{
		moveUp = condition;
	}
	public void MoveDown(bool condition)
	{
		moveDown = condition;
	}
	public void MoveRight(bool condition)
	{
		moveRight = condition;
	}
	public void MoveLeft(bool condition)
	{
		moveLeft = condition;
	}

	public void ResetPosition()
	{
		resetingPosition = true;
	}

    public void SetCameraBuildingMode()
    {
        old_ortho = myCamera.orthographicSize;
        old_position = transform.position;

        maxOrtho = maxOrthoBuilding;
        targetOrtho = 6;
        transform.position = new Vector3(x_pos_building_mode, 0, -10);
        myCamera.orthographicSize = 6;
    }

    public void SetCameraGameMode()
    {
        resetingPosition = false;
        targetOrtho = old_ortho;
        transform.position = old_position;
        myCamera.orthographicSize = old_ortho;
        maxOrtho = maxOrthoGame;
    }

    public void OpenShop()
    {
        x_pos_building_mode = 1.5f;
        ResetPosition();
    }

    public void ClosedShop()
    {
        x_pos_building_mode = 0;
        ResetPosition();
    }
}