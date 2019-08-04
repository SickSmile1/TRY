using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TRY.Kampfmodus.Characters;
using TRY.Kampfmodus.Collision;
using TRY.Kampfmodus.Weapons;

namespace TRY.Kampfmodus
{
    internal sealed class AddEnemy
    {
        private static Pathfinding.Pathfinding sPathfinding;
        private static BattleModeState sBattleModeState;
        private static CollisionManager sCollisionManager;
        private static int[] sAnimation;
        public AddEnemy(BattleModeState bms,Pathfinding.Pathfinding pathfinding, CollisionManager collisionManager)
        {
            sCollisionManager = collisionManager;
            sPathfinding = pathfinding;
            sBattleModeState = bms;
            sAnimation = new[] { 6, 4, 6, 6, 4 };
        }

        public void AddExplosive(Point position, int damage=0)
        {
            const string texture = "Explosiv";
            var enemy = new Character(texture, position, new Point(32, 32), sPathfinding, 0.175f, player: false, health: 75, vision: 300)
            {
                CharacterAnimation = new Animation(texture, new Vector2(32, 32), new List<int>(sAnimation))
            };
            enemy.Weapon = new Explode(sBattleModeState.FindCharactersInRadius, sBattleModeState.AddExplodedCharacter, enemy);
            enemy.Weapon.Damage += damage;
            sBattleModeState.AddNpc(enemy);
        }

        public void AddMeleeEnemy(Point position, int damage=0) 
        {
            const string texture = "MeleeEnemy";
            var enemy = new Character(texture, position, new Point(32, 32), sPathfinding, 0.2f, player: false, health: 120, vision: 200)
            {
                CharacterAnimation = new Animation(texture, new Vector2(32, 32), new List<int>(sAnimation)),
                Weapon = new Melee(sBattleModeState.FindNearestCharacter, false, 30)
            };
            enemy.Weapon.Damage += damage;
            sBattleModeState.AddNpc(enemy);
        }

        public void AddRangedEnemy(Point position, int damage=0)
        {
            const string texture = "RangedEnemy";
            var enemy = new Character(texture, position, new Point(32, 32), sPathfinding, player: false, vision: 300)
            {
                CharacterAnimation = new Animation(texture, new Vector2(32, 32), new List<int>(sAnimation)),
                Weapon = new Rifle("Projectile", false, sBattleModeState.FindNearestCharacterInRadius,
                    sBattleModeState.AddProjectile, fireRate: 0.2f)
            };
            enemy.Weapon.Damage += damage;
            sBattleModeState.AddNpc(enemy);
        }

        public void AddSupervisor(Point position, int damage=0)
        {
            const string texture = "Supervisor";
            var enemy = new Character(texture, position, new Point(32, 32), sPathfinding, 0.2f, player: false, health: 120, vision: 400)
            {
                CharacterAnimation = new Animation(texture, new Vector2(32, 32), new List<int>(sAnimation)),
                Weapon = new Melee(sBattleModeState.FindNearestCharacter, false, 0)
            };
            enemy.Weapon.Damage += damage;
            sBattleModeState.AddNpc(enemy);
        }


    }
}
