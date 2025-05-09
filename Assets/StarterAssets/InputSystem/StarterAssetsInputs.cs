﻿using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

        [Header("Weapon Input Values")]
        public bool shoot;
        public bool reload;

        [Header("Grapple Input Values")]
        public bool grapple;

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
		public bool lookingLocked = false;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnShoot(InputValue value)
        {
            ShootInput(value.isPressed);
        }

        public void OnReload(InputValue value)
        {
            ReloadInput(value.isPressed);
        }
        public void OnGrapple(InputValue value)
        {
            GrappleInput(value.isPressed);
        }
#endif


        public void MoveInput(Vector2 newMoveDirection)
		{
            if (!lookingLocked)
            {
                move = newMoveDirection;
            }
            else
            {
                move = new Vector2(0, 0);
            }
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			if (!lookingLocked)
			{
                look = newLookDirection;
            }
			else
			{
				look = new Vector2 (0,0);
			}
		}

		

		public void SprintInput(bool newSprintState)
		{
            if (!lookingLocked)
            {
                sprint = newSprintState;
            }
            else
            {
                sprint = false;
            }
            
		}

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }

        public void ReloadInput(bool newReloadState)
        {
            reload = newReloadState;
        }

        public void GrappleInput(bool newReloadState)
        {
            grapple = newReloadState;
        }

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}