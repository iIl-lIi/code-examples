using Game.Weapon;

namespace Game.Item.Implementations
{
    public static class FPSObjectsInitializator
    {
        public static void Initialize(IFPSObject fpsObject) 
        {
            switch (fpsObject.Index)
            {
                case ItemIndex.BaseFPSObject: fpsObject?.Initialize();         break;
                case ItemIndex.BaseWeapon   : InitializeBaseWeapon(fpsObject); break;
                case ItemIndex.Glock17      : InitializeGlock17(fpsObject);    break;
            }
        }

        private static void InitializeBaseWeapon(IFPSObject fpsObject)
        {
            if(fpsObject == null) return;
            fpsObject.Initialize();
            
            var weapon = (IWeapon)fpsObject;
            var parameters = WeaponParameters.GetParameters(fpsObject.Index);

            weapon.SetStates(new WeaponIdleState  (weapon, 1), 
                             new WeaponAttackState(weapon, 0, parameters));

            weapon.SwitchState(0);
        }
        private static void InitializeGlock17(IFPSObject fpsObject)
        {
            if(fpsObject == null) return;
            fpsObject.Initialize();
            
            var weapon = (IWeapon)fpsObject;
            var parameters = WeaponParameters.GetParameters(fpsObject.Index);

            weapon.SetStates(new FirearmIdleState      (weapon, 1, 2, 5), 
                             new WeaponAttackState     (weapon, 0, parameters), 
                             new WeaponAimingState     (weapon, 0, 3), 
                             new WeaponAimedIdleState  (weapon, 2, 4), 
                             new WeaponAimedAttackState(weapon, 3, parameters), 
                             new WeaponReloadingState  (weapon));

            weapon.SwitchState(0);
        }
    }
}