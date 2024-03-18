using System;
using Character.Monster;
using UnityEngine;

namespace Character
{
    public class Movement : MonoBehaviour
    {
        [field: SerializeField] public float MaxSpeed { get; protected set; }
        [field: SerializeField] public float knockBackDampingTime { get; protected set; }

        #region KnockBack

        protected Vector2 currentKnockBack;
        protected Vector2 knockBackDampingVelocity;

        #endregion

        public Vector2 ControlDirection { get; protected set; }

        #region Dash

        protected Vector3 dashStart;
        protected Vector3 dashEnd;
        protected float dashTime;
        protected bool isDash;
        protected Vector3 velocityDamping;
        protected float passedTime;

        #endregion

        private Rigidbody2D rigid;
        private Action onDashEnd;
        public void Awake()
        {
            rigid = gameObject.GetComponent<Rigidbody2D>();
        }

        public void InitData(PlayerData data)
        {
            data.status.onMovementSpeedChange += ChangeMaxSpeed;
            data.controller.onHit += GetKnockBack;
            data.controller.onMove += RunToward;
            data.controller.onDashStart += DashStart;
            onDashEnd += data.controller.CallDashEnd;

            MaxSpeed = data.status.currentMovementSpeed;
        }

        public void ChangeMaxSpeed(float value)
        {
            MaxSpeed = value;
        }

        public void InitData(MonsterData data)
        {
            data.controller.onHit += GetKnockBack;
            data.controller.onMove += RunToward;
        }

        private void FixedUpdate()
        {
            if (isDash)
            {
                rigid.position = Vector3.SmoothDamp(rigid.position, dashEnd, ref velocityDamping, dashTime);
                passedTime += Time.fixedDeltaTime;
                if (passedTime >= dashTime)
                {
                    isDash = false;
                    passedTime = 0.0f;
                    onDashEnd?.Invoke();
                }
            }
            else
            {
                SmoothKnockBack();
                var rigidPos = rigid.position;
                rigid.position = rigidPos + (MaxSpeed * ControlDirection + currentKnockBack) * Time.fixedDeltaTime;
            }
        }

        public void DashStart(Vector2 start, Vector2 end, float time)
        {
            if (!isDash)
            {
                dashStart = start;
                dashEnd = end;
                dashTime = time;
                isDash = true;
                StopMove();
                StopKnockBack();
            }
        }

        public void GetKnockBack(Vector2 shock, float knockback)
        {
            currentKnockBack = shock.normalized * knockback;
        }

        public void StopKnockBack()
        {
            currentKnockBack = Vector2.zero;
        }

        public void RunToward(Vector2 direction)
        {
            if (!isDash)
                ControlDirection = direction.normalized;
        }

        protected void SmoothKnockBack()
        {
            currentKnockBack = Vector2.SmoothDamp(currentKnockBack, Vector2.zero, ref knockBackDampingVelocity,
                knockBackDampingTime);
        }

        public void StopMove()
        {
            ControlDirection = Vector2.zero;
        }
    }
}