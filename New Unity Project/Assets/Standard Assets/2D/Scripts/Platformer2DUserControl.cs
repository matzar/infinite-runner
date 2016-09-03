using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets {
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D m_Character;
		private MenuController menuController;
		private bool m_Jump = false;
		bool crouch = false;
		bool slide = false;
		bool grounded;
		[SerializeField] private float slideTime = 1.5f;

		[SerializeField] private AudioClip jumpSound;
		[SerializeField] private AudioClip deadSound;
		[SerializeField] private AudioClip slideSound;
		private AudioSource audioSource;

		public Canvas gameOverCanvas;
		public Text gameOverText;
		public Text restartText;

		private int jumpCounter = 0;

        private void Awake()
        {
			gameOverCanvas = gameOverCanvas.GetComponent<Canvas> ();
			audioSource = GetComponent<AudioSource> ();
            m_Character = GetComponent<PlatformerCharacter2D>();
			menuController = GetComponent<MenuController> ();
			gameOverText.text = "";
			restartText.text = "";
			gameOverCanvas.enabled = false;
        }


        private void Update()
		{
			if (jumpCounter >= 2) {
				jumpCounter = 0;
			}
			if (!m_Jump && jumpCounter < 2) {
				// Read the jump input in Update so button presses aren't missed.
				m_Jump = CrossPlatformInputManager.GetButtonDown ("Jump");
				if (m_Jump) {
					audioSource.clip = jumpSound;
					audioSource.Play();
					jumpCounter++;
				}
			}
			if (audioSource.clip == slideSound && !m_Character.GetGrounded()) {
				audioSource.Stop ();
			}
        }


        private void FixedUpdate()
		{
			bool slide = Input.GetButton ("Fire1");
			if (slide && m_Character.GetGrounded()) {
				StartCoroutine (Slide ());
			}
			if (!m_Character.GetGrounded()) {
				crouch = false;
			}

			bool dodge = Input.GetKey (KeyCode.LeftAlt); 	 // If left alt pressed, dodge = true
//            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
		if (m_Character.GetGameOver ()) {
			gameOverCanvas.enabled = true;
			m_Character.Move (0f, false, false, false);
			gameOverText.text = "GAME OVER";
			restartText.text = "Press 'R' to restart";
			if (Input.GetKey (KeyCode.R)) {
				Application.LoadLevel (0);
			}
			if (Input.GetKey ("escape")) {
				Application.Quit ();
			}
		}
		if (!m_Character.GetGameOver()) {
				m_Character.Move (1f, crouch, m_Jump, dodge);
			}
            m_Jump = false;
        }

		IEnumerator Slide() {
			crouch = true;
			audioSource.clip = slideSound;
			audioSource.Play ();
			yield return new WaitForSeconds(slideTime);
			audioSource.Stop ();
			crouch = false;
		}
    }
}