using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Input;
using UnityEngine;

namespace Game.Character.Implementations.Player
{
    public class BasePlayerCharacter : MonoBehaviour, IPlayer
    {
        public event Action Dead;
        public event Action Grounded;

        [Header("Character")]
        [SerializeField] [Min(0)] private float _StartHp = 100;
        [SerializeField] private Transform _BodyRoot;
        
        [Header("Movement")]
        [SerializeField] private BasePlayerController _PlayerController;
        [SerializeField] private CharacterController _Controller;

        [Header("Crouch")] 
        [SerializeField] private float _DefaultHeight = 2;
        [SerializeField] private float _CrouchHeight = 1.25f;
        [SerializeField] private Vector3 _CrouchCharacterCenter = new Vector3(0, -0.375f, 0);
        [SerializeField] private float _CrouchSpeed = 2.65f;
        
        [Header("Camera")]
        [SerializeField] private Camera _Camera;
        [SerializeField] private float _CameraNormalHeight = 0.85f;
        [SerializeField] private float _CameraCrouchHeight = 0.1f;
        [SerializeField] private float _CameraAngleUpLimit;
        [SerializeField] private float _CameraAngleDownLimit;
        [SerializeField] private Vector3 _CameraDownLookPosition;

        [Space]
        [SerializeField] private List<AddTransform> _AddTransforms = new List<AddTransform>();
        
        public Transform RootTransform { get; set; }
        public Vector3 GravityValue { get; set; }
        public Vector2 MovementVelocity { get; set; }
        public bool IsCrouch { get; set; }
        public bool InJump { get; set; }
        public float CurrentHp { get; set; }
        public bool InMovement { get; set; }
        public Vector3 LastDamagerPosition { get; set; }
        public bool IsDeath { get; set; }
        public List<AddTransform> AddTransforms { get => _AddTransforms; set => _AddTransforms = value; }
        public Camera Camera { get => _Camera; set => _Camera = value; }
        public BasePlayerController PlayerController => _PlayerController;
        public CharacterController CharacterController => _Controller;
        public Vector3 JumpVelocity => _jumpVelocity;

        public float CameraDownLookFactor
        {
            get
            {
                if (_CameraAngleDownLimit >= 0 || _cameraVerticalAngle >= 0) return 0;
                return _cameraVerticalAngle / _CameraAngleDownLimit;
            }
        }
        public bool IsGrounded => _Controller.isGrounded;

        private Vector3 _cameraPosition;
        private Vector3 _jumpVelocity;
        private Vector2 _cameraRotateVelocity;
        
        private float _cameraVerticalAngle;
        private float _cameraHorizontalAngle;
        private float _crouchValue;

        public async UniTask Initialize()
        {
            CurrentHp = _StartHp;
            RootTransform = transform;

            _crouchValue = 0;
            _cameraVerticalAngle = 0;
            _cameraHorizontalAngle = 0;
            _jumpVelocity = Vector3.zero;
            _Controller.height = _DefaultHeight;
            _cameraPosition = Vector3.up * _CameraNormalHeight;
            
            Camera = _Camera;
            var cameraTransform = _Camera.transform;
            cameraTransform.localPosition = _cameraPosition;
            cameraTransform.localRotation = Quaternion.identity;
            
            _BodyRoot.localRotation = Quaternion.identity;
        }
        public void SetPosition(Vector3 position)
        {
            RootTransform.position = position;
        }
        public void SetRotation(Vector2 rotation)
        {
            _cameraVerticalAngle = rotation.x;
            _cameraHorizontalAngle = rotation.y;
        }
        public Vector2 GetRotation() => new Vector2(_cameraVerticalAngle, _cameraHorizontalAngle);
        public void RotateCamera(Vector3 velocity)
        {
            _cameraRotateVelocity = velocity * InputManager.ResolutionDifference;
        }
        public void Jump(float force)
        {
            if (_Controller.isGrounded == false) return;
            InJump = true;
            _jumpVelocity.y = force;
        }
        public void Crouch(bool value)
        {
            IsCrouch = value;
        }
        public void Move(Vector2 velocity)
        {
            MovementVelocity = velocity;
        }
        public void StopMove()
        {
            MovementVelocity = Vector2.zero;
        }
        public void Damaged(float damageValue, IBaseCharacter damager)
        {
            if (IsDeath) return;
            
            CurrentHp -= damageValue;

            if (CurrentHp <= 0)
            {
                CurrentHp = 0;
                Death();
            }
            
            Debug.Log($"<color=yellow>{damager.GetType().Name}</color> dealt " +
                      $"<color=red>{damageValue}</color> damage to " +
                      $"<color=cyan>{GetType().Name} ({CurrentHp}HP)</color>");
        }
        public void Death()
        {
            IsDeath = true;
            Dead?.Invoke();
        }

        private void UpdateJump()
        {
            if (_Controller.isGrounded == false)
            {
                _jumpVelocity += GravityValue * Time.deltaTime;
            }
            else if (InJump && _jumpVelocity.y <= 0)
            {
                InJump = false;
                Grounded?.Invoke();
            }
        }
        private void UpdateCrouch()
        {
            if (IsCrouch)
            {
                if (_crouchValue < 1) _crouchValue += Time.deltaTime * _CrouchSpeed;
                if (_crouchValue > 1) _crouchValue = 1;
                
            }
            else
            {
                if (_crouchValue > 0) _crouchValue -= Time.deltaTime * _CrouchSpeed;
                if (_crouchValue < 0) _crouchValue = 0;
            }

            _Controller.height = Mathf.Lerp(_DefaultHeight, _CrouchHeight, _crouchValue);
            _Controller.center = Vector3.Lerp(Vector3.zero, _CrouchCharacterCenter, _crouchValue);
            _cameraPosition = Vector3.Lerp(Vector3.up * _CameraNormalHeight, Vector3.up * _CameraCrouchHeight, _crouchValue);
        }
        private void UpdateCameraAndRotate()
        {
            _cameraVerticalAngle +=  Time.deltaTime * _cameraRotateVelocity.y;
            _cameraHorizontalAngle += Time.deltaTime * _cameraRotateVelocity.x;
            
            _cameraRotateVelocity = Vector2.zero;

            if (_cameraVerticalAngle > _CameraAngleUpLimit) _cameraVerticalAngle = _CameraAngleUpLimit; 
            if (_cameraVerticalAngle < _CameraAngleDownLimit) _cameraVerticalAngle = _CameraAngleDownLimit;

            var addPosition = Vector3.zero;
            var addRotation = Quaternion.identity;
            _AddTransforms.ForEach(x =>
            {
                if (x.Transform == null) return;
                switch (x.UseParameters)
                {
                    case UseTransformParameters.Everything:
                        addPosition += x.Transform.localPosition + x.OffsetPosition; 
                        addRotation *= x.Transform.localRotation * Quaternion.Euler(x.OffsetRotation); break;
                    case UseTransformParameters.Position:
                        addPosition += x.Transform.localPosition + x.OffsetPosition; break;
                    case UseTransformParameters.Rotation:
                        addRotation *= x.Transform.localRotation * Quaternion.Euler(x.OffsetRotation); break;
                }
            });
            
            var camPos = _cameraPosition + _CameraDownLookPosition * CameraDownLookFactor + addPosition;
            _Camera.transform.localPosition -= (_Camera.transform.localPosition - camPos) * Time.deltaTime * 24;
            
            var camRot = Quaternion.Euler(-_cameraVerticalAngle, 0, 0) * addRotation;
            _Camera.transform.localRotation =  Quaternion.LerpUnclamped(_Camera.transform.localRotation, camRot, Time.deltaTime * 24);
            
            _BodyRoot.localRotation = Quaternion.LerpUnclamped(_BodyRoot.localRotation, Quaternion.Euler(Vector3.up * _cameraHorizontalAngle), Time.deltaTime * 24);
        }
        private void UpdateMovement()
        {
            var vertical = _BodyRoot.TransformDirection(Vector3.forward) * MovementVelocity.y;
            var horizontal = _BodyRoot.TransformDirection(Vector3.right) * MovementVelocity.x;
            var movementVelocity = _jumpVelocity + vertical + horizontal;
            
            _Controller.Move(movementVelocity * Time.deltaTime);
        }

        private void Update()
        {
            UpdateCrouch();
            UpdateMovement();
            UpdateCameraAndRotate();
            UpdateJump();
        }
    }
}