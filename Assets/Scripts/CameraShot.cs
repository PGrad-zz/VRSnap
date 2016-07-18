/**
 * Adapted from TakeHiResScreenShot here: http://answers.unity3d.com/questions/22954/how-to-save-a-picture-take-screenshot-from-a-camer.html
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Image))]
public class CameraShot : MonoBehaviour {
	private int resWidth = Screen.width, 
				resHeight = Screen.height,
				quarterWidth,
				quarterHeight;
	//private int screenReticleSize;
	private int reticleWidth;
	new private Camera camera;
	//public Image image;
	public Material frameMat;
	private Texture initTxtr;
	private bool takeShot = false;
	//private int x, y;
	private int[] dims;
	private const int UNITY_TO_SCREEN_CONVERSION = 75; //prev 500
	private Color[] pixels;
	//For weighting "warmness" of colors.
	private int[] warmness;
	void Start() {
		camera = gameObject.GetComponent<Camera> ();
		quarterWidth = resWidth / 2;
		quarterHeight = resHeight / 2;
		initTxtr = frameMat.mainTexture;
	}
	/**public static string ScreenShotName(int width, int height) {
		return string.Format("{0}/Screenshots/screen_{1}x{2}_{3}.png", 
			Application.dataPath, 
			width, height, 
			System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}*/

	public void ClearFrame () {
		frameMat.mainTexture = initTxtr;
	}

	public void TakeCameraShot(float reticleWidth) {
		//this.screenReticleSize = (int) unityReticleSize * UNITY_TO_SCREEN_CONVERSION;
		takeShot = true;
		this.reticleWidth = (int) (reticleWidth * UNITY_TO_SCREEN_CONVERSION);
		//x = (resWidth - this.screenReticleSize) / 2;
		//y = (resHeight - this.screenReticleSize) / 2;
	}

	void LateUpdate() {
		if (frameMat != null && takeShot) {
			//image.GetComponent<CanvasRenderer> ().SetAlpha (1f);
			RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
			camera.targetTexture = rt;
			//Texture2D screenShot = new Texture2D(screenReticleSize, screenReticleSize, TextureFormat.RGB24, false);
			Texture2D screen = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
			camera.Render();
			RenderTexture.active = rt;
			screen.ReadPixels (new Rect(0,0,resWidth,resHeight), 0, 0);
			screen.Apply ();
			dims = findReticleDims (screen);
			if (dims != null) {
				Texture2D screenShot = new Texture2D (dims [2], dims [2], TextureFormat.RGB24, false);
				screenShot.ReadPixels (new Rect (dims [0], dims [1], dims [2], dims [2]), 0, 0);
				screenShot.Apply ();
				frameMat.mainTexture = screenShot;
			}
			//image.sprite = Sprite.Create (screenShot, new Rect(0,0,screenShot.width, screenShot.width), new Vector2(0.5f,0.5f));
			//image.CrossFadeAlpha(255f,2f,true);
			camera.targetTexture = null;
			RenderTexture.active = null; // JC: added to avoid errors
			Destroy(rt);
			/**byte[] bytes = screenShot.EncodeToPNG();
			string filename = ScreenShotName(resWidth, resHeight);
			System.IO.File.WriteAllBytes(filename, bytes);
			Debug.Log(string.Format("Took screenshot to: {0}", filename));*/
			takeShot = false;
		}
	}

	//Return int array of {x,y,dim} where x and y are the coordinates of the (inner) bottom left corner of the reticle, and dim is the length of the reticle.
	int[] findReticleDims (Texture2D screen) {
		int cur = 0,
			lastPixel,
			x = 0;
		bool foundRed,
			 isLine;
		pixels = screen.GetPixels (0, 0, quarterWidth, quarterHeight);
		for (int row = 0; row < quarterHeight; row++) {
			cur = row * quarterWidth;
			foundRed = false;
			isLine = true;
			lastPixel = 0;
			for (int col = 0; col < quarterWidth; col++) {
				if (pixels [cur + col].Equals (Color.red)) {
					if (!foundRed) {
						x = col;
						lastPixel = cur + col;
						foundRed = true;
					} else if (lastPixel + 1 != cur + col) {
							isLine = false;
							break;
					} else 
						lastPixel++;
				} 
			}
			isLine &= foundRed;
			if (isLine) {
				row += reticleWidth;
				if ((quarterHeight - row) * 2 > 1)
					return new int[]{ x + reticleWidth, row, (quarterHeight - row) * 2 };
				else
					Debug.Log ("failed"); //What do I tell the player when this happens?
			}
		}
		return null;
	}
}