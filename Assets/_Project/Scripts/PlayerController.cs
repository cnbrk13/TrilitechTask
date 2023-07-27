#region

using System;
using _Project.Scripts.Enums;
using _Project.Scripts.Structs;
using DG.Tweening;
using UnityEngine;

#endregion

namespace _Project.Scripts
{

	public class PlayerController : MonoBehaviour
	{
		#region Serialized Fields

		[SerializeField] private float airAccel = 1f;
		[SerializeField] private float airDecel = 10f;
		[SerializeField] private float airMaxSpeed = 10f;

		[Tooltip("Accel multiplier wheen boost key is down")] [SerializeField]
		private float boostMultipAccel = 2f;

		[Header("BOOST")] [Tooltip("Max speed multiplier wheen boost key is down, higher value = faster when boosting")]
		[SerializeField]
		private float boostMultipMaxVel = 2f;

		[Header("MISC")] [Tooltip("Window of time to allow jumping after leaving ground surface")] [SerializeField]
		private float coyoteTime = 0.1f;

		[Tooltip("Downwards speed clamp when falling with jump key pressed")] [SerializeField]
		private float fallLimitMinVel = -30f;

		[Header("ACCEL")] [SerializeField]
		private float groundAccel = 10f;

		[SerializeField] private float groundDecel = 10f;
		[SerializeField] private float groundMaxSpeed = 10f;

		[Tooltip("Window of time to remember wanting to jump before hitting ground. " +
		         "So we can jump even at an early key press.")] [SerializeField]
		private float jumpBuffer = 0.1f;

		[Header("JUMPING")] [SerializeField]
		private float jumpPower = 30;

		[Tooltip("Gravity applied when jump key is not down")] [SerializeField]
		private float maxGravity = -2;

		[Header("GRAVITY")] [Tooltip("Gravity applied when jump key is down")] [SerializeField]
		private float minGravity = -1;

		public JumpState JumpState;
		public Vector3 Punch = Vector3.one * .8f;
		public float Duration = .3f;
		public int Vibrato = 5;
		public float Elasticity;

		#endregion

		#region Constants and Fields

		private bool coyoteUsable;
		private float currentBoostMultipAccel = 1f;
		private float currentBoostMultipMaxVel = 1f;
		private bool endedJumpEarly = true;
		private bool hasBufferedJump;
		private FrameInput inputs;
		private bool isMoving;
		private float lastJumpPressed = float.MinValue;
		private Rigidbody2D rb;
		private Vector3 targetJumpVel = Vector3.zero;
		private float ungroundedAt;

		#endregion

		#region Public Events and Delegates

		public event Action<float, SpriteRenderer> HitGround;
		public event Action StartMove;
		public event Action StopMove;

		#endregion

		#region Public Properties

		public float FallSpeed
		{
			get => rb.velocity.y;
		}

		#endregion

		#region Private Properties

		private bool CanUseCoyote
		{
			get => coyoteUsable && JumpState != JumpState.Grounded && ungroundedAt + coyoteTime > Time.time;
		}

		#endregion

		#region Unity Methods

		private void Awake()
		{
			rb = GetComponent<Rigidbody2D>();
		}

		private void Start()
		{
			SetState(JumpState.Falling);
		}

		private void Update()
		{
			if (GameManager.Instance.GameState != GameState.Playing)
			{
				return;
			}
			
			ProcessInput();
			var final_velocity = ComputeFinalVelocity();

			rb.velocity = final_velocity;
			CheckAndHandleMovementStatus(final_velocity);
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			if (!other.gameObject.CompareTag("JumpSurface"))
			{
				return;
			}

			OnHitGround(other.collider);
		}

		private void OnCollisionExit2D(Collision2D other)
		{
			if (!other.gameObject.CompareTag("JumpSurface"))
			{
				return;
			}

			SetState(JumpState.Jumping);
			ungroundedAt = Time.time;
		}

		// private void OnTriggerEnter2D(Collider2D collision)
		// {
		// 	if (!collision.CompareTag("JumpSurface"))
		// 	{
		// 		return;
		// 	}
		//
		// 	OnHitGround(collision);
		// }

		// private void OnTriggerExit2D(Collider2D collision)
		// {
		// 	if (!collision.CompareTag("JumpSurface"))
		// 	{
		// 		return;
		// 	}
		//
		// 	SetState(JumpState.Jumping);
		// 	ungroundedAt = Time.time;
		// }

		private void OnCollisionStay2D(Collision2D other)
		{
			if (!other.gameObject.CompareTag("JumpSurface"))
			{
				return;
			}

			SetState(JumpState.Grounded);
		}

		#endregion

		#region Private Methods

		private Vector2 CalculateGravity()
		{
			if (JumpState == JumpState.Grounded)
			{
				return Vector3.zero;
			}

			var gravity = maxGravity;

			if (inputs.JumpDown && rb.velocity.y > 0)
			{
				gravity = minGravity;
			}

			var magnitude = gravity * Time.deltaTime;

			// if pressing jump but falling too fast for slow jump speed
			if (rb.velocity.y < fallLimitMinVel && inputs.JumpDown)
			{
				magnitude = -maxGravity * Time.deltaTime;
			}

			return Vector3.up * magnitude;
		}

		private Vector2 CalculateGroundVelocity()
		{
			var current_vel = rb.velocity;

			var current_x = Vector3.Dot(current_vel, transform.right);

			var acceleration = JumpState == JumpState.Grounded ? groundAccel : airAccel;
			var deceleration = JumpState == JumpState.Grounded ? groundDecel : airDecel;

			var new_x = current_x;

			if (inputs.InputVector.x != 0)
			{
				var a = acceleration * currentBoostMultipAccel;
				var s = JumpState == JumpState.Grounded ? groundMaxSpeed : airMaxSpeed;

				var target_vel = inputs.InputVector.x * s * currentBoostMultipMaxVel;

				new_x = Mathf.MoveTowards(current_x, target_vel, a * Time.deltaTime);
			}

			new_x = Mathf.MoveTowards(new_x, 0, deceleration * Time.deltaTime);

			// Calculate the velocities in the direction we are looking
			var x_vel = transform.right * (new_x - current_x);

			//Vector3 zVel = transform.forward * (newZ - currentZ);

			return x_vel;
		}

		private void CheckAndHandleMovementStatus(Vector2 velocity)
		{
			var currently_moving = velocity.x != 0;

			if (currently_moving)
			{
				if (!isMoving && JumpState == JumpState.Grounded)
				{
					StartedMove();
				}
			}
			else if (isMoving)
			{
				StoppedMove();
			}

			isMoving = currently_moving;
		}

		private Vector2 ComputeFinalVelocity()
		{
			var velocity = rb.velocity;
			velocity += CalculateGroundVelocity();
			velocity += GetJumpVector();
			velocity += CalculateGravity();

			ProcessJumpState(velocity);
			return velocity;
		}

		private Vector2 DoJump()
		{
			endedJumpEarly = false;
			coyoteUsable = false;
			ungroundedAt = float.MinValue;

			var velocity = rb.velocity;
			var to_return = Vector2.up * (jumpPower + -velocity.y);

			targetJumpVel = velocity + to_return;
			hasBufferedJump = false;
			PlayJumpEffects();

			return to_return;
		}

		private void DoPunchScale()
		{
			Transform transform1;
			(transform1 = transform).DOKill();
			transform1.localScale = Vector3.one;

			transform.DOPunchScale(Punch, Duration, Vibrato, Elasticity);
		}

		private void DoPunchScaleTransform(float multiplier = 1f)
		{
			Transform transform1;
			(transform1 = transform).DOKill();
			transform1.localScale = Vector3.one;

			transform1.DOPunchScale(-Punch * multiplier, Duration, Vibrato, Elasticity);
		}

		private void GatherInput()
		{
			inputs = new FrameInput
			{
				JumpPressed = Input.GetButtonDown("Jump"),
				JumpReleased = Input.GetButtonUp("Jump"),
				JumpDown = Input.GetButton("Jump"),
				BoostDown = Input.GetKey(KeyCode.LeftShift),
				X = Input.GetAxisRaw("Horizontal"),
				Z = Input.GetAxisRaw("Vertical")
			};

			if (inputs.JumpPressed)
			{
				lastJumpPressed = Time.time;
			}
		}

		private Vector2 GetJumpVector()
		{
			var jump_vector = Vector2.zero;

			// Jump if: grounded or within coyote threshold || sufficient jump buffer
			if ((inputs.JumpPressed || hasBufferedJump) && (JumpState == JumpState.Grounded || CanUseCoyote))
			{
				jump_vector = DoJump();
			}

			// End the jump early if button released
			if (JumpState != JumpState.Grounded && inputs.JumpReleased && !endedJumpEarly &&
			    rb.velocity.y > targetJumpVel.y * 0.75f)
			{
				endedJumpEarly = true;
			}

			return jump_vector;
		}

		private void OnHitGround(Collider2D collision)
		{
			if (JumpState == JumpState.Grounded)
			{
				return;
			}

			SetState(JumpState.Grounded);
			coyoteUsable = true;
			hasBufferedJump = lastJumpPressed + jumpBuffer > Time.time;
			PlayLandAnim();
			HitGround?.Invoke(rb.velocity.y, collision.gameObject.GetComponent<SpriteRenderer>());
		}

		private void PlayJumpEffects()
		{
			DoPunchScale();
		}

		private void PlayLandAnim()
		{
			DoPunchScaleTransform();
		}

		private void ProcessInput()
		{
			GatherInput();

			if (inputs.BoostDown)
			{
				currentBoostMultipAccel = boostMultipAccel;
				currentBoostMultipMaxVel = boostMultipMaxVel;
			}
			else
			{
				currentBoostMultipAccel = 1f;
				currentBoostMultipMaxVel = 1f;
			}
		}

		private void ProcessJumpState(Vector2 velocity)
		{
			if (JumpState != JumpState.Grounded)
			{
				SetState(velocity.y > 0 ? JumpState.Jumping : JumpState.Falling);
			}
		}

		private void SetState(JumpState new_state)
		{
			if (JumpState == new_state)
			{
				return;
			}

			JumpState = new_state;

			switch (new_state)
			{
				case JumpState.Jumping:
					StoppedMove();

					break;
				case JumpState.Falling:
					StoppedMove();

					break;
				case JumpState.Grounded:
					break;
			}
		}

		private void StartedMove()
		{
			StartMove?.Invoke();
			isMoving = true;
		}

		private void StoppedMove()
		{
			StopMove?.Invoke();
			isMoving = false;
		}

		#endregion

		// private void OnTriggerStay2D(Collider2D other)
		// {
		// 	if (!other.CompareTag("JumpSurface"))
		// 	{
		// 		return;
		// 	}
		//
		// 	SetState(JumpState.Grounded);
		// }
	}

}