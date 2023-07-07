using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.Character
{
    [Flags] public enum UseTransformParameters : sbyte
    {
        Everything = -1,
        Position = 1, 
        Rotation = 2
    }

    [Serializable] public class AddOffset
    {
        public UseTransformParameters UseParameters;
        public Vector3 OffsetPosition;
        public Vector3 OffsetRotation;
        public Func<Vector3> GetPosition = () => Vector3.zero;
        public Func<Quaternion> GetRotation = () => Quaternion.identity;
    }
    public interface ICameraRotate
    {
        Camera Camera { get; set; }
        List<AddOffset> AddOffsets { get; set; }
        void RotateCamera(Vector3 velocity);
    }
    public interface IMovable
    {
        bool InMovement { get; set; }
        Vector2 MovementVelocity { get; set; }

        void Move(Vector2 velocity);
        void StopMove();
    }
    public interface IDeathable
    {
        event Action Dead;

        bool IsDeath { get; set; }

        void Death();
    }
    public interface IDamageable
    { 
        Vector3 LastDamagerPosition { get; set; }
        float CurrentHp { get; set; }

        void Damaged(float damageValue, IBaseCharacter damager);
    }
    public interface IJumpable
    {
        event Action Grounded;
        bool InJump { get; set; }
        Vector3 GravityValue { get; set; }

        void Jump(float force);
    }
    public interface IBaseCharacter : IJumpable, IMovable, IDamageable, IDeathable
    {
        Transform RootTransform { get; set; }
    }
    public interface ICrouch
    {
        bool IsCrouch { get; set; }
        void Crouch(bool value);
    }
    public interface IPlayer : IBaseCharacter, ICameraRotate, ICrouch
    {
        UniTask Initialize();
        void SetPosition(Vector3 position);
        
        void SetRotation(Vector2 rotation);
        Vector2 GetRotation();
    }
}