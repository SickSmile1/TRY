using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TRY.Kampfmodus.Abilities;
using TRY.Kampfmodus.Collision;
using TRY.Kampfmodus.Commands;

namespace TRY.Kampfmodus.Characters
{
    interface ICharacter : ICommandObserver, IDynamicCollider
    {
        // Properties of a Character
        string Texture { get; set; }
        Vector2 Position { get; set; }
        Rectangle CharacterArea { get; }

        Animation CharacterAnimation { get; set; }
        Vector2? Destination { get; set; }
        Vector2 MidPoint { get; set; }
        int Health { get; set; }
        int MaxHealth { get; set; }
        float DeathTimer { get; set; }
        Weapons.IWeapon Weapon { get; set; }
        IAbility Ability { get; set; }
        IAbility SupportAbility { get; set; }
        bool IsBeingRevived { get; set; }
        int PlayerLevel { get; set; }
        bool Player { get; set; }
        bool Active { get; set; }
        int Vision { get; }
        string Id { get; set; }
        Pathfinding.Pathfinding Pathfinding { get; set; }


        // Methods of a Character
        void Draw(SpriteBatch sb,TextureManager textureManager);
        void Update(GameTime gameTime);
    }
}
