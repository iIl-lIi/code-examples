using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BattleSystem.Character.EventHandlers;
using Data.Character;
using Data.Character.Upgrade;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BattleSystem.Character
{
    public sealed class CharacterModel
    {
        public event Action Wined;
        public event Action<float> Attacked;
        public event Action<CharacterModel> ReceivedDamage;
        public event Action<float, float> ChangedHitPoints;
        public event Action Dead;
        
        private readonly List<AnimationEventHandler> idle           = new ();
        private readonly List<AnimationEventHandler> stepTowards    = new ();
        private readonly List<MoveBackEventHandler>  moveBack       = new ();
        private readonly List<AttackEventHandler>    attack         = new ();
        private readonly List<AnimationEventHandler> receivedDamage = new ();
        private readonly List<AnimationEventHandler> die            = new ();
        private readonly List<AnimationEventHandler> win            = new ();
        private readonly CharacterData data;
        private readonly UpgradesData upgrades;

        public CharacterParametersData AttackActionParameters { get; set; }
        public CharacterParametersData ReceivedDamageActionParameters { get; set; }

        public float HitPoints
        {
            get => hitPoints;
            private set
            {
                hitPoints = value;
                ChangedHitPoints?.Invoke(HitPoints, MaxHitPoints);
            }
        }
        public float MaxHitPoints => data.Parameters.HitPoints + upgrades.HitPoints;
        public Transform RootTransform { get; private set; }
        public bool IsDead { get; private set; }

        private float hitPoints;
        private bool receivingDamageProcess;
        private bool attackProcess;
        private bool isWin;

        public CharacterModel(CharacterData data, UpgradesData upgrades)
        {
            this.data = data;
            this.upgrades = upgrades;
            HitPoints = data.Parameters.HitPoints + upgrades.HitPoints;
        }
        public async Task Initialize(CharacterModel enemy, Vector3 position, Vector3 scale)
        {
            var gameObject = await CharacterLoader.Load(data.ModelId);
            var listener = gameObject.GetComponent<AnimationsEventsListener>();
            
            RootTransform = gameObject.transform;
            RootTransform.position = position;
            RootTransform.localScale = scale;

            var allowedAttackAnimations = data.AttackAnimations.Where(x => x.Cost <= upgrades.WinsCount).ToList();
            var allowedReceivedDamageAnimations = data.ReceivedDamageAnimations.Where(x => x.Cost <= upgrades.WinsCount).ToList();
            var allowedWinAnimations = data.WinAnimations.Where(x => x.Cost <= upgrades.WinsCount).ToList();
            allowedAttackAnimations.ForEach(x => attack.Add(new AttackEventHandler(x, listener, OnAttacked)));
            allowedReceivedDamageAnimations.ForEach(x => receivedDamage.Add(new AnimationEventHandler(x, listener)));
            allowedWinAnimations.ForEach(x => win.Add(new AnimationEventHandler(x, listener)));

            data.IdleAnimations.ForEach(x => idle.Add(new AnimationEventHandler(x, listener)));
            data.StepTowardsAnimations.ForEach(x => stepTowards.Add(new StepTowardsEventHandler(x, listener, RootTransform)));
            data.MoveBackAnimations.ForEach(x => moveBack.Add(new MoveBackEventHandler(x, listener, RootTransform)));
            data.DieAnimations.ForEach(x => die.Add(new AnimationEventHandler(x, listener)));
            
            enemy.Attacked += OnReceivedDamage;
            enemy.Dead += Win;
        }
        public async Task Execute()
        {
            if (attackProcess) return;
            attackProcess = true;
            while (receivingDamageProcess || IsDead || isWin) await Task.Yield();

            var idleHandler        = GetAnimationWithChance(idle);
            var stepTowardsHandler = GetAnimationWithChance(stepTowards);
            var attackHandler      = GetAnimationWithChance(attack);
            var moveBackHandler    = GetAnimationWithChance(moveBack);

            if (attackHandler.isLongRangeAttack == false) await stepTowardsHandler.Execute();
            await attackHandler.Execute();
            if (attackHandler.isLongRangeAttack == false) await moveBackHandler.Execute();
            await idleHandler.Execute();
            
            if (isWin)
            {
                await GetAnimationWithChance(win).Execute();
                await idleHandler.Execute();
                Wined?.Invoke();
            }
            attackProcess = false;
        }
        public void Heal(float value)
        {
            HitPoints += Mathf.Abs(value);
            if (HitPoints > MaxHitPoints) HitPoints = MaxHitPoints;
        }

        private void Win() => isWin = true;
        private void OnAttacked(float animationFactor)
        {
            var damageValue = data.Parameters.AttackValue * data.Parameters.AttackFactor;
            var randomCriticalAttackChance = Random.Range(0, 1f);
            var criticalFactor = randomCriticalAttackChance < data.Parameters.CriticalAttackChance ? data.Parameters.CriticalAttackFactor : 1;

            var value = damageValue + upgrades.AttackValue;
            var actionCriticalFactor = 1f;
            var actionAttackFactor = 1f;
            if (AttackActionParameters != null)
            {
                value += AttackActionParameters.AttackValue;
                actionAttackFactor = AttackActionParameters.AttackFactor;
                actionCriticalFactor = AttackActionParameters.CriticalAttackFactor;
            }
            Attacked?.Invoke(value * animationFactor * actionAttackFactor * criticalFactor * actionCriticalFactor);
            AttackActionParameters = null;
        }
        private async void OnReceivedDamage(float damageValue)
        {
            if (attackProcess || receivingDamageProcess || IsDead || isWin) return;
            receivingDamageProcess = true;
            var actionFactor = ReceivedDamageActionParameters?.ReceivedDamageFactor ?? 1f;
            HitPoints -= damageValue * data.Parameters.ReceivedDamageFactor * actionFactor;
            ReceivedDamageActionParameters = null;
            if (HitPoints <= 0)
            {
                HitPoints = 0;
                IsDead = true;
                ReceivedDamage?.Invoke(this);
                Dead?.Invoke();
                await GetAnimationWithChance(die).Execute();
            }
            else
            {
                ReceivedDamage?.Invoke(this);
                await GetAnimationWithChance(receivedDamage).Execute();
                await GetAnimationWithChance(idle).Execute();
            }
            receivingDamageProcess = false;
        }
        private static THandler GetAnimationWithChance<THandler>(in List<THandler> handlers) where THandler : AnimationEventHandler
        {
            var sortByChances = handlers.OrderBy(x => x.animationData.ActivateChance);
            var randomChance = Random.Range(0, 1f);
            return sortByChances.FirstOrDefault(handler => handler.animationData.ActivateChance >= randomChance);
        }
    }
}